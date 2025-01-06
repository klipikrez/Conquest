using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GLTFast;
using GLTFast.Addons;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


[System.Serializable]
public class TerrainTreesCompiled
{
    public TerrainTreesCompiled(TreeInstance[] instances, TreePrototype[] prototypes)
    {

        List<TreeInstanceStorage> bob = new List<TreeInstanceStorage>();
        List<TreePrototypeStorage> sajba = new List<TreePrototypeStorage>();

        foreach (TreeInstance instance in instances)
        {

            bob.Add(new TreeInstanceStorage(instance));
        }
        workTrees = bob.ToArray();
        foreach (TreePrototype prototype in prototypes)
        {

            sajba.Add(new TreePrototypeStorage(prototype));
        }

        workTreePrototypes = sajba.ToArray();
    }
    public TreeInstanceStorage[] workTrees;
    public TreePrototypeStorage[] workTreePrototypes;
}

[System.Serializable]
public class TreeInstanceStorage
{
    public Vector3 position;
    public float widthScale;
    public float heightScale;
    public float rotation;
    public Color32 color;
    public Color32 lightmapColor;
    public int prototypeIndex;

    public TreeInstanceStorage(TreeInstance treeInstance)
    {
        this.position = treeInstance.position;
        this.widthScale = treeInstance.widthScale;
        this.heightScale = treeInstance.heightScale;
        this.rotation = treeInstance.rotation;
        this.color = treeInstance.color;
        this.lightmapColor = treeInstance.lightmapColor;
        this.prototypeIndex = treeInstance.prototypeIndex;
    }

    public TreeInstance Copy()
    {
        TreeInstance treeInstance = new TreeInstance();

        treeInstance.position = this.position;
        treeInstance.widthScale = this.widthScale;
        treeInstance.heightScale = this.heightScale;
        treeInstance.rotation = this.rotation;
        treeInstance.color = this.color;
        treeInstance.lightmapColor = this.lightmapColor;
        treeInstance.prototypeIndex = this.prototypeIndex;

        return treeInstance;
    }
}

[System.Serializable]
public class TreePrototypeStorage
{

    public GameObject m_Prefab;
    public float m_BendFactor;
    public int m_NavMeshLod;

    public TreePrototypeStorage(TreePrototype treePrototype)
    {

        this.m_Prefab = treePrototype.prefab;
        this.m_BendFactor = treePrototype.bendFactor;
        this.m_NavMeshLod = treePrototype.navMeshLod;
    }

    public TreePrototype Copy()
    {
        TreePrototype treePrototype = new TreePrototype();

        treePrototype.prefab = this.m_Prefab;
        treePrototype.bendFactor = this.m_BendFactor;
        treePrototype.navMeshLod = this.m_NavMeshLod;

        return treePrototype;
    }
}

[System.Serializable]
public class TerrainDetailsCompiled
{
    public TerrainDetailsCompiled(DetailPrototype[] instances)
    {

        List<DetailStorage> bob = new List<DetailStorage>();

        foreach (DetailPrototype instance in instances)
        {
            bob.Add(new DetailStorage(instance));
        }

        details = bob.ToArray();
    }
    public DetailStorage[] details;
}

[System.Serializable]
public class DetailStorage
{

    //public GameObject prototype;
    public string prototypeName;
    /*ovo ne koristimo :)*/
    public Texture2D PrototypeTexture;
    public float MinWidth;
    public float MaxWidth;
    public float MinHeight;
    public float MaxHeight;
    public float NoiseSpread;
    public Color HealthyColor;
    public Color DryColor;
    public DetailRenderMode RenderMode;

    public DetailStorage(DetailPrototype treePrototype)
    {

        this.prototypeName = treePrototype.prototype.name;
        this.PrototypeTexture = treePrototype.prototypeTexture;
        this.MinWidth = treePrototype.minWidth;
        this.MaxWidth = treePrototype.maxWidth;
        this.MinHeight = treePrototype.minHeight;
        this.MaxHeight = treePrototype.maxHeight;
        this.NoiseSpread = treePrototype.noiseSpread;
        this.HealthyColor = treePrototype.healthyColor;
        this.DryColor = treePrototype.dryColor;
        this.RenderMode = treePrototype.renderMode;
    }

    public DetailPrototype Copy()
    {
        DetailPrototype detailPrototype = new DetailPrototype();
        detailPrototype.prototype = Resources.Load("Folage/" + this.prototypeName) as GameObject;
        detailPrototype.prototypeTexture = this.PrototypeTexture;
        detailPrototype.minWidth = this.MinWidth;
        detailPrototype.maxWidth = this.MaxWidth;
        detailPrototype.minHeight = this.MinHeight;
        detailPrototype.maxHeight = this.MaxHeight;
        detailPrototype.noiseSpread = this.NoiseSpread;
        detailPrototype.healthyColor = this.HealthyColor;
        detailPrototype.dryColor = this.DryColor;
        detailPrototype.renderMode = this.RenderMode;
        detailPrototype.usePrototypeMesh = true; // uvek koristimo mesh
        detailPrototype.useInstancing = true;
        return detailPrototype;
    }
}
[System.Serializable]
public class LayerStorage
{
    public string textureName;
    public float scale = 2;
    public string LayerName;
    public LayerStorage(string tex, float scale, string name)
    {
        this.textureName = tex;
        this.scale = scale;
        this.LayerName = name;
    }
}
[System.Serializable]
public class LayersCompiled
{
    public LayersCompiled(TerrainLayer[] instances)
    {

        List<LayerStorage> bob = new List<LayerStorage>();

        foreach (TerrainLayer instance in instances)
        {
            Debug.Log("--" + instance.diffuseTexture.name);
            bob.Add(new LayerStorage(instance.diffuseTexture.name, instance.tileSize.x, instance.name));
        }

        layers = bob.ToArray();
        Debug.Log(layers.Length);
    }
    public LayerStorage[] layers;
    public Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

    void LoadTextures()
    {
        if (textures == null) textures = new Dictionary<string, Texture2D>();
        if (textures.Count != 0) { return; }

        string folderPath = Application.dataPath + "/StreamingAssets/TerrainTextures";

        string[] Files = Directory.GetFiles(folderPath); //Getting Text files

        int i = 0;

        foreach (string file in Files)
        {
            if (IsImage(file))
            {

                Byte[] pngBytes = System.IO.File.ReadAllBytes(file);
                Texture2D tt = new Texture2D(52, 52);
                tt.LoadImage(pngBytes);//moguce je ede da dovo treba da se sacuva negde na disky
                                       //tt.alphaIsTransparency = true;
                tt.name = Path.GetFileName(file);

                textures.Add(tt.name, tt);
                Debug.Log(tt.name);
                i++;
            }

        }

    }

    bool IsImage(string fileName)
    {
        string extension = Path.GetExtension(fileName).ToLower();
        return extension == ".png" || extension == ".jpg" || extension == ".jpeg" || extension == ".bmp";
    }

    public void Load(TerrainData terrain, bool isEditor = true)
    {
        LoadTextures();

        foreach (LayerStorage layer in layers)
        {
            if (isEditor)
            {
                if (!textures.ContainsKey(layer.textureName)) continue;
                Debug.Log(layer.LayerName);
                TerrainLayers.Instance.AddNewLayer(textures[layer.textureName], layer.scale, layer.LayerName);
            }
            else
            {
                List<TerrainLayer> terrainLayersList = new List<TerrainLayer>();

                //set new layer in terrain
                terrainLayersList.AddRange(terrain.terrainLayers);
                TerrainLayer newLayer = new TerrainLayer();
                if (textures[layer.textureName] != null) { newLayer.diffuseTexture = textures[layer.textureName]; }
                newLayer.tileSize = new Vector2(layer.scale, layer.scale);
                newLayer.name = layer.LayerName;
                terrainLayersList.Add(newLayer);

                terrain.terrainLayers = terrainLayersList.ToArray();
            }
        }
    }
}

[System.Serializable]
public struct TwerOverrideStorage
{
    public string overrideName;
    public float value;
}

[System.Serializable]
struct BuildingAndItsConnections
{
    public BuildingMain main;
    public int[] connections;
}
[System.Serializable]
public class TowerStorage
{
    public int id;
    public int team = 0;
    [System.NonSerialized]
    public TowerPresetData preset;
    public string modelPath;
    public string presetName;
    public TwerOverrideStorage[] towerOverrides;
    public int[] connections;
    public Vector3 position;
    public quaternion rotation;
    public float scale = 1;
    public TowerStorage(int id, int team/*, TowerPresetData preset*/, Dictionary<string, float> towerOverrides, int[] connections, Vector3 pos, quaternion rotation, string modelPath, string presetName)
    {
        this.id = id;
        this.team = team;
        //this.preset = preset;
        this.towerOverrides = new TwerOverrideStorage[towerOverrides.Count];
        this.connections = connections;
        this.position = pos;
        this.modelPath = modelPath;
        this.rotation = rotation;
        this.presetName = presetName;

        int i = 0;
        foreach (KeyValuePair<string, float> kvp in towerOverrides)
        {
            this.towerOverrides[i].overrideName = kvp.Key;
            this.towerOverrides[i++].value = kvp.Value;
        }
    }

    public void LoadPreset()
    {
        string folderPath = Application.dataPath + "/StreamingAssets/TowerPresets/" + presetName;
        string filePath = Path.Combine(folderPath, "data.rez");

        if (!File.Exists(filePath))
        {
            Debug.Log("Failed to load tower preset for: " + filePath);
            return;
        }

        TowerPresetData TowerPresetData = JsonUtility.FromJson<TowerPresetData>(File.ReadAllText(filePath));//update setings json

        if (TowerPresetData == null)
        {
            Debug.Log("Failed to load tower preset for(content mismatch): " + filePath);
            return;
        }

        preset = TowerPresetData;
    }

}

public class TowerCompiled
{
    public TowerStorage[] towers;
    public TowerCompiled(EditorTower[] editorTowers)
    {
        towers = new TowerStorage[editorTowers.Length];
        for (int i = 0; i < editorTowers.Length; i++)
        {
            int[] conn = new int[editorTowers[i].connections.Count];
            int j = 0;
            foreach (TowerConnection twr in editorTowers[i].connections)
            {
                if (editorTowers[i] == twr.tower1) conn[j] = twr.tower2.selfID;
                if (editorTowers[i] == twr.tower2) conn[j] = twr.tower1.selfID;
                /*                    if (twr.line1.enabled && twr.line2.enabled)
                                    {
                                        if (editorTowers[i] == twr.tower1) conn[j] = twr.tower2.selfID;
                                        if (editorTowers[i] == twr.tower2) conn[j] = twr.tower1.selfID;

                                    }
                                    else
                                    if (!twr.line1.enabled && twr.line2.enabled)
                                    {
                                        if (editorTowers[i] == twr.tower1) conn[j] = twr.tower2.selfID;
                                    }
                                    else
                                        if (editorTowers[i] == twr.tower2) conn[j] = twr.tower1.selfID;*/
                j++;

            }
            towers[i] = new TowerStorage(editorTowers[i].selfID, editorTowers[i].team/*, editorTowers[i].preset*/, editorTowers[i].towerOverrides, conn, editorTowers[i].transform.position, editorTowers[i].transform.rotation, editorTowers[i].meshName, editorTowers[i].presetName);

        }
    }

    public void LoadPresets()
    {
        foreach (TowerStorage tower in towers)
        {
            tower.LoadPreset();
        }
    }

    public void InstantiateInLevel(GameObject towerDefaultPrefab, bool editorMode = false)
    {
        if (editorMode)
        {
            Dictionary<int, EditorTower> tempTowers = new Dictionary<int, EditorTower>();
            foreach (TowerStorage t in towers)
            {
                EditorTower tower = GameObject.Instantiate(towerDefaultPrefab, t.position, t.rotation).GetComponent<EditorTower>();

                tower.gameObject.transform.position = t.position;
                tower.gameObject.transform.rotation = t.rotation;
                tower.SetId(t.id);
                tower.team = t.team;
                tempTowers.Add(t.id, tower);
                EditorManager.Instance.editorTowers.Add(tower);



                string file = Application.dataPath + "/StreamingAssets/TowerPresets/" + t.presetName + "/" + t.modelPath;


                AsyncSetTowerEditor(file, tower, t);
                //ImportGLTFAsync(dirName + "/" + path, b);


            }

            foreach (TowerStorage t in towers)
            {
                EditorTower tower = tempTowers[t.id];

                foreach (int conn in t.connections)
                {
                    tower.AddConnection(tempTowers[conn], 1);

                }
            }
        }
        else
        {



            Dictionary<int, BuildingAndItsConnections> spawnedBuildings = new Dictionary<int, BuildingAndItsConnections>(); // building as key, the first value is the id of rhe building itself, the rest connections
            foreach (TowerStorage t in towers)
            {
                BuildingMain tower = GameObject.Instantiate(towerDefaultPrefab, t.position, t.rotation).GetComponent<BuildingMain>();
                tower.Inicialize();
                BuildingAndItsConnections buildingAndItsConnections = new BuildingAndItsConnections();
                buildingAndItsConnections.main = tower;
                buildingAndItsConnections.connections = t.connections;
                spawnedBuildings.Add(t.id, buildingAndItsConnections);

                Dictionary<string, object> map = new Dictionary<string, object>();
                foreach (TwerOverrideStorage tOverride in t.towerOverrides)
                {
                    map.Add(tOverride.overrideName, tOverride.value);
                }
                TowerPresetData preset = GetBuildingPresetByName(t.presetName);
                tower.production.SetProduct(map.ContainsKey("Starting units") ? (float)map["Starting units"] : preset.product);
                tower.production.maxUnits = (int)(map.ContainsKey("Max units") ? (float)map["Max units"] : preset.maxUnits);
                tower.production.productProduction = (map.ContainsKey("Unit production") ? (float)map["Unit production"] : preset.productProduction);
                //tower.GetProduction().SetProduct(t.towerOverrides.ContainsKey("Cost as an upgrade") ? (float)t.towerOverrides["Cost as an upgrade"] : preset.cost);
                tower.team.vulnerability = (map.ContainsKey("Vulnerability") ? (float)map["Vulnerability"] : preset.vulnerability);
                tower.id = t.id;
                Debug.Log(tower.id);
                tower.team.teamid = t.team;

                string file = Application.dataPath + "/StreamingAssets/TowerPresets/" + t.presetName + "/" + t.modelPath;

                // Load the GLTF file
                if (t.preset.meshPath != null && t.preset.meshPath.Length > 0 && t.preset.meshPath[0] != "")
                {

                    AsyncSetTower(file, tower, t);

                }
                else
                {
                    Debug.LogError("it no exist :::: " + file);
                }


            }

            //setup connections
            foreach (KeyValuePair<int, BuildingAndItsConnections> building in spawnedBuildings)
            {


                foreach (int connectionId in building.Value.connections)
                {
                    if (connectionId != building.Key)
                    {
                        building.Value.main.neighbours.Add(spawnedBuildings[connectionId].main);
                    }
                }


            }

        }
    }

    private async void AsyncSetTowerEditor(string file, EditorTower tower, TowerStorage t)
    {

        // Load the GLTF file
        byte[] data = File.ReadAllBytes(file);
        var gltf = new GltfImport();
        bool success = await gltf.LoadGltfBinary(
            data,
            // The URI of the original data is important for resolving relative URIs within the glTF
            new Uri(file)
            );

        if (success)
        {

            tower.SetPreset(GetBuildingPresetByName(t.presetName), t.presetName, new meshAndName { mesh = gltf.GetMeshes()[0], name = t.modelPath });
            foreach (TwerOverrideStorage tOverride in t.towerOverrides)
            {
                Debug.Log(t.id + " -- " + tOverride.overrideName + " -- " + tOverride.value);
                tower.towerOverrides.Add(tOverride.overrideName, tOverride.value);
            }
        }
        else
        {
            Debug.Log("Failed to load:" + file);
        }

    }

    private async void AsyncSetTower(string file, BuildingMain tower, TowerStorage t)
    {


        // Load the GLTF file
        byte[] data = File.ReadAllBytes(file);
        var gltf = new GltfImport();
        bool success = await gltf.LoadGltfBinary(
            data,
            // The URI of the original data is important for resolving relative URIs within the glTF
            new Uri(file)
            );

        if (success)
        {
            tower.team.SetMesh(gltf.GetMeshes()[0]);
        }
        else
        {
            Debug.Log("Failed to load:" + file);
        }

    }

    TowerPresetData GetBuildingPresetByName(string name)
    {
        if (!System.IO.Directory.Exists(Application.dataPath + "/StreamingAssets/TowerPresets"))
        {
            System.IO.Directory.CreateDirectory(Application.dataPath + "/StreamingAssets/TowerPresets");
        }

        string folderPath = Application.dataPath + "/StreamingAssets/TowerPresets";


        TowerPresetData presetData;

        if (!File.Exists(folderPath + "/" + name + "/data.rez"))
        {
            presetData = new TowerPresetData();
            Debug.Log("Failed to find tower preset data for: " + folderPath + "/" + name + "/data.rez");
            return presetData;
        }
        else
        {
            FileStream fileStream = new FileStream(folderPath + "/" + name + "/data.rez", FileMode.Open, FileAccess.Read, FileShare.Read, 64 * 1024,
       (FileOptions)0x20000000 | FileOptions.WriteThrough & FileOptions.SequentialScan);

            string fileContents;
            using (StreamReader reader = new StreamReader(fileStream))
            {
                fileContents = reader.ReadToEnd();
            }


            presetData = JsonUtility.FromJson<TowerPresetData>(fileContents);//get tower

        }

        if (presetData == null)
        {
            presetData = new TowerPresetData();
            Debug.Log("Failed to load tower preset data for: " + folderPath + "/" + name + "/data.rez");
        }

        return presetData;
    }

}




