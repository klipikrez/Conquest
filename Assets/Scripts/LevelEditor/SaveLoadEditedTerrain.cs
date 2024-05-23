using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GLTFast;
using GLTFast.Addons;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


///////////////////////////////////////////////////////////////////
//                                                               //
//     Storage classes - for temp storage of unity classes,      //
//                 to be able to read/write them                 //
//                                                               //
//     Compiled classes -  for easier manipulation of arrays     //
//                                                               //
///////////////////////////////////////////////////////////////////


public class SaveLoadEditedTerrain : MonoBehaviour
{
    public Terrain terrain;
    public GameObject towerDefaultPrefab;
    public GameObject editorTowerPrefab;
    //public Terrain terrain2;

    // Start is called before the first frame update
    void Start()
    {

        //SaveTerrain("bababoj", terrain1.terrainData);
        LoadLevelEditor("brki");
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SaveTerrain()
    {
        Debug.Log(EditorManager.Instance.playerSpawn + " : " + Vector3.negativeInfinity + "   ----   " + (EditorManager.Instance.playerSpawn == Vector3.negativeInfinity));
        string levelName = EditorOptions.Instance.terrainNameInput.text;
        if (levelName == "" || levelName == null) { ErrorManager.Instance.SendError("Name your level, bruh..."); return; }
        if (!levelName.All(char.IsLetterOrDigit)) { ErrorManager.Instance.SendError("Only letters and numbers are allowed in level name >:V"); return; }
        if (char.IsDigit(levelName[0])) { ErrorManager.Instance.SendError("The first character in the save name can't be a number :("); return; }
        if (EditorManager.Instance.playerSpawn.x < float.MinValue) { ErrorManager.Instance.SendError("Player spawn location not set :P"); return; }
        //if (levelName == "brki") { ErrorManager.Instance.SendError("Nuh Uh ;) change your level name."); return; }

        Debug.Log("" + levelName);
        CheckLevelFolder(levelName);
        SaveTerrainHeight(levelName, terrain.terrainData);
        SaveTerrainAlpha(levelName, terrain.terrainData);
        SaveTerrainTrees(levelName, terrain.terrainData);
        SaveTerrainDetails(levelName, terrain.terrainData);
        SaveTerrainTowers(levelName);
        SaveLevelOptionsEditor(levelName);
        ErrorManager.Instance.SendSucsess("Oops...\n we successfully saved your level ;D"); return;
    }

    public void LoadLevelEditor(string levelName)
    {
        CheckLevelFolder(levelName);
        LoadTerrainHeight(levelName, terrain.terrainData);
        LoadTerrainAlpha(levelName, terrain.terrainData);
        LoadTerrainTrees(levelName, terrain.terrainData);
        LoadTerrainDetails(levelName, terrain.terrainData);
        LoadTerrainTowersEditor(levelName);
        LoadLevelOptionsEditor(levelName);
        if (levelName != "brki")
        {
            EditorOptions.Instance.terrainNameInput.text = levelName;
        }

    }

    public void LoadLevel(string levelName)
    {
        CheckLevelFolder(levelName);
        LoadTerrainHeight(levelName, terrain.terrainData);
        LoadTerrainAlpha(levelName, terrain.terrainData);
        LoadTerrainTrees(levelName, terrain.terrainData);
        LoadTerrainDetails(levelName, terrain.terrainData);
        LoadTerrainTowers(levelName);
        LoadLevelOptions(levelName);
    }

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
    public struct TwerOverrideStorage
    {
        public string overrideName;
        public object value;
    }
    [System.Serializable]
    public class TowerStorage
    {
        public int id;
        public TowerPresetData preset;
        public string modelPath;
        public string presetName;
        public TwerOverrideStorage[] towerOverrides;
        public int[] connections;
        public Vector3 position;
        public quaternion rotation;
        public float scale;
        public TowerStorage(int id, TowerPresetData preset, Dictionary<string, object> towerOverrides, int[] connections, Vector3 pos, quaternion rotation, string modelPath, string presetName)
        {
            this.id = id;
            this.preset = preset;
            this.towerOverrides = new TwerOverrideStorage[towerOverrides.Count];
            this.connections = connections;
            this.position = pos;
            this.modelPath = modelPath;
            this.rotation = rotation;
            this.presetName = presetName;

            int i = 0;
            foreach (KeyValuePair<string, object> kvp in towerOverrides)
            {
                this.towerOverrides[i].overrideName = kvp.Key;
                this.towerOverrides[i++].value = kvp.Value;
            }
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
                Debug.Log(editorTowers[i].meshName);
                towers[i] = new TowerStorage(editorTowers[i].selfID, editorTowers[i].preset, editorTowers[i].towerOverrides, conn, editorTowers[i].transform.position, editorTowers[i].transform.rotation, editorTowers[i].meshName, editorTowers[i].presetName);

            }
        }

        public void InstantiateInLevel(GameObject towerDefaultPrefab, bool editorMode = false)
        {
            if (editorMode)
            {
                Dictionary<int, EditorTower> tempTowers = new Dictionary<int, EditorTower>();
                foreach (TowerStorage t in towers)
                {
                    EditorTower tower = Instantiate(towerDefaultPrefab, t.position, t.rotation).GetComponent<EditorTower>();
                    foreach (TwerOverrideStorage tOverride in t.towerOverrides)
                    {
                        tower.towerOverrides.Add(tOverride.overrideName, tOverride.value);
                    }

                    tower.gameObject.transform.position = t.position;
                    tower.gameObject.transform.rotation = t.rotation;
                    tower.SetId(t.id);
                    tempTowers.Add(t.id, tower);
                    EditorManager.Instance.editorTowers.Add(tower);

                    string file = Application.dataPath + "/StreamingAssets/TowerPresets/" + t.presetName + "/" + t.modelPath;
                    Debug.Log(file);


                    AsyncSetTowerEditor(file, tower, t);
                    //ImportGLTFAsync(dirName + "/" + path, b);


                }

                foreach (TowerStorage t in towers)
                {
                    EditorTower tower = tempTowers[t.id];

                    foreach (int conn in t.connections)
                        tower.AddConnection(tempTowers[conn], 1);
                }
            }
            else
            {
                foreach (TowerStorage t in towers)
                {
                    UnitController tower = Instantiate(towerDefaultPrefab, t.position, t.rotation).GetComponent<UnitController>();
                    Dictionary<string, object> map = new Dictionary<string, object>();
                    foreach (TwerOverrideStorage tOverride in t.towerOverrides)
                    {
                        map.Add(tOverride.overrideName, tOverride.value);
                    }

                    tower.GetProduction().SetProduct(map.ContainsKey("Starting units") ? (float)map["Starting units"] : t.preset.product);
                    tower.GetProduction().maxUnits = (int)(map.ContainsKey("Max units") ? (float)map["Max units"] : t.preset.maxUnits);
                    tower.GetProduction().productProduction = (map.ContainsKey("Unit production") ? (float)map["Unit production"] : t.preset.productProduction);
                    //tower.GetProduction().SetProduct(t.towerOverrides.ContainsKey("Cost as an upgrade") ? (float)t.towerOverrides["Cost as an upgrade"] : t.preset.cost);
                    tower.GetTeam().vulnerability = (map.ContainsKey("Vulnerability") ? (float)map["Vulnerability"] : t.preset.vulnerability);


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
                tower.SetPreset(t.preset, t.presetName, new meshAndName { mesh = gltf.GetMeshes()[0], name = t.modelPath });
            }
            else
            {
                Debug.Log("nevalja::" + file);
            }

        }

        private async void AsyncSetTower(string file, UnitController tower, TowerStorage t)
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
                tower.GetTeam().SetMesh(gltf.GetMeshes()[0]);
            }
            else
            {
                Debug.Log("nevalja::" + file);
            }

        }
    }

    void SaveTerrainTowers(string levelName)
    {

        TowerCompiled towers = new TowerCompiled(EditorManager.Instance.editorTowers.ToArray());


        string folderPath = Application.dataPath + "/StreamingAssets/Levels/" + levelName;
        string filePath = Path.Combine(folderPath, levelName + "_Towers.rez");
        File.WriteAllText(filePath, JsonUtility.ToJson(towers, true));//update setings json


    }

    void LoadTerrainTowers(string levelName)
    {
        string folderPath = Application.dataPath + "/StreamingAssets/Levels/" + levelName;
        string filePath = Path.Combine(folderPath, levelName + "_Towers.rez");

        TowerCompiled towers = JsonUtility.FromJson<TowerCompiled>(File.ReadAllText(filePath));//update setings json

        towers.InstantiateInLevel(towerDefaultPrefab);
    }

    void LoadTerrainTowersEditor(string levelName)
    {
        string folderPath = Application.dataPath + "/StreamingAssets/Levels/" + levelName;
        string filePath = Path.Combine(folderPath, levelName + "_Towers.rez");

        TowerCompiled towers = JsonUtility.FromJson<TowerCompiled>(File.ReadAllText(filePath));//update setings json

        towers.InstantiateInLevel(editorTowerPrefab, true);
    }

    void SaveLevelOptionsEditor(string levelName)
    {
        LevelOptions options = new LevelOptions
        {
            playerPos = levelName != "brki" ? EditorManager.Instance.playerSpawn : Vector3.negativeInfinity
        };
        string folderPath = Application.dataPath + "/StreamingAssets/Levels/" + levelName;
        string filePath = Path.Combine(folderPath, levelName + "_Options.rez");
        File.WriteAllText(filePath, JsonUtility.ToJson(options));//update setings json
    }

    void LoadLevelOptionsEditor(string levelName)
    {

        string folderPath = Application.dataPath + "/StreamingAssets/Levels/" + levelName;
        string filePath = Path.Combine(folderPath, levelName + "_Options.rez");

        LevelOptions options = JsonUtility.FromJson<LevelOptions>(File.ReadAllText(filePath));//update setings json


        EditorManager.Instance.playerSpawn = options.playerPos;
    }

    void LoadLevelOptions(string levelName)
    {
        string folderPath = Application.dataPath + "/StreamingAssets/Levels/" + levelName;
        string filePath = Path.Combine(folderPath, levelName + "_Options.rez");

        LevelOptions options = JsonUtility.FromJson<LevelOptions>(File.ReadAllText(filePath));//update setings json

        GameObject player = GameObject.FindWithTag("Player");
        player.transform.position = options.playerPos + Vector3.up * 22;
    }

    void SaveTerrainDetails(string levelName, TerrainData terrain)
    {
        DetailPrototype[] workPrototypes = new DetailPrototype[terrain.detailPrototypes.Length];

        for (int dp = 0; dp < workPrototypes.Length; dp++)
        {

            DetailPrototype clonedPrototype = new DetailPrototype();

            // prototype
            clonedPrototype.prototype = terrain.detailPrototypes[dp].prototype;
            // prototypeTexture
            /*ovo ne koristimo :)*/
            clonedPrototype.prototypeTexture = terrain.detailPrototypes[dp].prototypeTexture;
            // minWidth
            clonedPrototype.minWidth = terrain.detailPrototypes[dp].minWidth;
            // maxWidth
            clonedPrototype.maxWidth = terrain.detailPrototypes[dp].maxWidth;
            // minHeight
            clonedPrototype.minHeight = terrain.detailPrototypes[dp].minHeight;
            // maxHeight
            clonedPrototype.maxHeight = terrain.detailPrototypes[dp].maxHeight;
            // noiseSpread
            clonedPrototype.noiseSpread = terrain.detailPrototypes[dp].noiseSpread;
            // healthyColor
            clonedPrototype.healthyColor = terrain.detailPrototypes[dp].healthyColor;
            // dryColor
            clonedPrototype.dryColor = terrain.detailPrototypes[dp].dryColor;
            // renderMode
            clonedPrototype.renderMode = terrain.detailPrototypes[dp].renderMode;

            workPrototypes[dp] = clonedPrototype;
        }

        TerrainDetailsCompiled details = new TerrainDetailsCompiled(workPrototypes);

        string folderPath = Application.dataPath + "/StreamingAssets/Levels/" + levelName;
        string filePath = Path.Combine(folderPath, levelName + "_Details.rez");
        File.WriteAllText(filePath, JsonUtility.ToJson(details));//update setings json


        int numDetailLayers = terrain.detailPrototypes.Length;
        for (int layNum = 0; layNum < numDetailLayers; layNum++)
        {
            filePath = Path.Combine(folderPath, levelName + "_DetailMap" + layNum + ".rez");
            float[,] thisDetailLayer = EditorManager.Instance.folage;
            Debug.Log(filePath);
            FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            BinaryWriter bw = new BinaryWriter(fs);
            for (int i = 0; i < terrain.detailHeight; i++)
            {
                for (int j = 0; j < terrain.detailWidth; j++)
                {
                    bw.Write((float)thisDetailLayer[i, j]);
                }
            }
            bw.Close();
        }



    }

    void LoadTerrainDetails(string levelName, TerrainData terrain)
    {

        string folderPath = Application.dataPath + "/StreamingAssets/Levels/" + levelName;
        string filePath = Path.Combine(folderPath, levelName + "_Details.rez");

        TerrainDetailsCompiled details = JsonUtility.FromJson<TerrainDetailsCompiled>(File.ReadAllText(filePath));//update setings json

        List<DetailPrototype> clonedDetails = new List<DetailPrototype>();
        for (int ti = 0; ti < details.details.Length; ti++)
        {
            clonedDetails.Add(details.details[ti].Copy());

        }
        terrain.detailPrototypes = clonedDetails.ToArray();


        int layNum = 0;
        while (true)
        {


            string path = Application.dataPath + "/StreamingAssets/Levels/" + levelName + "/" + levelName + "_DetailMap" + layNum++ + ".rez";


            if (!File.Exists(path)) break;

            float[,] dat = new float[terrain.detailWidth, terrain.detailHeight];
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            BinaryReader br = new BinaryReader(fs);

            for (int i = 0; i < terrain.detailHeight; i++)
            {
                for (int j = 0; j < terrain.detailWidth; j++)
                {
                    dat[i, j] = (float)br.ReadSingle();
                }

            }
            br.Close();

            EditorManager.Instance.RefreshDetailTerrain(dat);


            //terrain.SetDetailLayer(0, 0, layNum++, dat);


        }
    }

    void SaveTerrainDetailsOld(string levelName, TerrainData terrain)
    {
        DetailPrototype[] workPrototypes = new DetailPrototype[terrain.detailPrototypes.Length];

        for (int dp = 0; dp < workPrototypes.Length; dp++)
        {

            DetailPrototype clonedPrototype = new DetailPrototype();

            // prototype
            clonedPrototype.prototype = terrain.detailPrototypes[dp].prototype;
            // prototypeTexture
            /*ovo ne koristimo :)*/
            clonedPrototype.prototypeTexture = terrain.detailPrototypes[dp].prototypeTexture;
            // minWidth
            clonedPrototype.minWidth = terrain.detailPrototypes[dp].minWidth;
            // maxWidth
            clonedPrototype.maxWidth = terrain.detailPrototypes[dp].maxWidth;
            // minHeight
            clonedPrototype.minHeight = terrain.detailPrototypes[dp].minHeight;
            // maxHeight
            clonedPrototype.maxHeight = terrain.detailPrototypes[dp].maxHeight;
            // noiseSpread
            clonedPrototype.noiseSpread = terrain.detailPrototypes[dp].noiseSpread;
            // healthyColor
            clonedPrototype.healthyColor = terrain.detailPrototypes[dp].healthyColor;
            // dryColor
            clonedPrototype.dryColor = terrain.detailPrototypes[dp].dryColor;
            // renderMode
            clonedPrototype.renderMode = terrain.detailPrototypes[dp].renderMode;

            workPrototypes[dp] = clonedPrototype;
        }

        TerrainDetailsCompiled details = new TerrainDetailsCompiled(workPrototypes);

        string folderPath = Application.dataPath + "/StreamingAssets/Levels/" + levelName;
        string filePath = Path.Combine(folderPath, levelName + "_Details.rez");
        File.WriteAllText(filePath, JsonUtility.ToJson(details));//update setings json


        int numDetailLayers = terrain.detailPrototypes.Length;
        for (int layNum = 0; layNum < numDetailLayers; layNum++)
        {
            filePath = Path.Combine(folderPath, levelName + "_DetailMap" + layNum + ".rez");
            int[,] thisDetailLayer = terrain.GetDetailLayer(0, 0, terrain.detailWidth, terrain.detailHeight, layNum);

            FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            BinaryWriter bw = new BinaryWriter(fs);
            for (int i = 0; i < terrain.detailHeight; i++)
            {
                for (int j = 0; j < terrain.detailWidth; j++)
                {
                    bw.Write((int)thisDetailLayer[i, j]);
                }
            }
            bw.Close();
        }



    }

    void LoadTerrainDetailsOld(string levelName, TerrainData terrain)
    {

        string folderPath = Application.dataPath + "/StreamingAssets/Levels/" + levelName;
        string filePath = Path.Combine(folderPath, levelName + "_Details.rez");

        TerrainDetailsCompiled details = JsonUtility.FromJson<TerrainDetailsCompiled>(File.ReadAllText(filePath));//update setings json

        List<DetailPrototype> clonedDetails = new List<DetailPrototype>();
        for (int ti = 0; ti < details.details.Length; ti++)
        {
            clonedDetails.Add(details.details[ti].Copy());

        }
        terrain.detailPrototypes = clonedDetails.ToArray();


        int layNum = 0;
        while (true)
        {


            string path = Application.dataPath + "/StreamingAssets/Levels/" + levelName + "/" + levelName + "_DetailMap" + layNum + ".rez";


            if (!File.Exists(path)) break;

            int[,] dat = terrain.GetDetailLayer(0, 0, terrain.detailWidth, terrain.detailHeight, layNum);
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            BinaryReader br = new BinaryReader(fs);

            for (int i = 0; i < terrain.detailHeight; i++)
            {
                for (int j = 0; j < terrain.detailWidth; j++)
                {
                    dat[i, j] = (int)br.ReadInt32();
                }

            }
            br.Close();
            terrain.SetDetailLayer(0, 0, layNum++, dat);


        }
    }

    void SaveTerrainTrees(string levelName, TerrainData terrain)
    {
        /*  terrain.treeInstances = terrain.treeInstances;
          terrain.treePrototypes = terrain.treePrototypes;*/

        // Tree Instances = TreeInstance[]
        TreeInstance[] workTrees = new TreeInstance[terrain.treeInstances.Length];

        for (int ti = 0; ti < workTrees.Length; ti++)
        {
            TreeInstance clonedTree = new TreeInstance();

            // position
            clonedTree.position = terrain.treeInstances[ti].position;
            // widthScale
            clonedTree.widthScale = terrain.treeInstances[ti].widthScale;
            // heightScale
            clonedTree.heightScale = terrain.treeInstances[ti].heightScale;
            // color
            clonedTree.color = terrain.treeInstances[ti].color;
            // lightmapColor
            clonedTree.lightmapColor = terrain.treeInstances[ti].lightmapColor;
            // prototypeIndex
            clonedTree.prototypeIndex = terrain.treeInstances[ti].prototypeIndex;

            workTrees[ti] = clonedTree;
        }
        //workData.treeInstances = workTrees;

        // Tree Prototypes = TreePrototype[]
        TreePrototype[] workTreePrototypes = new TreePrototype[terrain.treePrototypes.Length];

        for (int tp = 0; tp < workTreePrototypes.Length; tp++)
        {
            TreePrototype clonedTreePrototype = new TreePrototype();

            // prefab
            clonedTreePrototype.prefab = terrain.treePrototypes[tp].prefab;
            // bendFactor
            clonedTreePrototype.bendFactor = terrain.treePrototypes[tp].bendFactor;

            workTreePrototypes[tp] = clonedTreePrototype;
        }

        //workData.treePrototypes = workTreePrototypes;


        TerrainTreesCompiled trees = new TerrainTreesCompiled(workTrees, workTreePrototypes);

        string folderPath = Application.dataPath + "/StreamingAssets/Levels/" + levelName;
        string filePath = Path.Combine(folderPath, levelName + "_Trees.rez");
        File.WriteAllText(filePath, JsonUtility.ToJson(trees));//update setings json

    }

    void LoadTerrainTrees(string levelName, TerrainData terrain)
    {



        string folderPath = Application.dataPath + "/StreamingAssets/Levels/" + levelName;
        string filePath = Path.Combine(folderPath, levelName + "_Trees.rez");
        TerrainTreesCompiled trees = JsonUtility.FromJson<TerrainTreesCompiled>(File.ReadAllText(filePath));//update setings json



        List<TreeInstance> clonedTree = new List<TreeInstance>();
        for (int ti = 0; ti < trees.workTrees.Length; ti++)
        {
            clonedTree.Add(trees.workTrees[ti].Copy());
        }
        terrain.treeInstances = clonedTree.ToArray();

        //ovo ti je ako oces u buducce da das koisniku da doda svoje drvece
        /*List<TreePrototype> workTreePrototypes = new List<TreePrototype>();
        for (int tp = 0; tp < trees.workTreePrototypes.Length; tp++)
        {
            workTreePrototypes.Add(trees.workTreePrototypes[tp].Copy());
        }
        terrain.treePrototypes = workTreePrototypes.ToArray();*/



    }

    void SaveTerrainAlpha(string levelName, TerrainData terrain)
    {

        string folderPath = Application.dataPath + "/StreamingAssets/Levels/" + levelName;
        string filePath = Path.Combine(folderPath, levelName + "_Splat.txt");


        float[,,] numArray = terrain.GetAlphamaps(0, 0, terrain.alphamapWidth, terrain.alphamapHeight);
        BinaryWriter writer = new BinaryWriter(new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None));
        int num = 0;
        while (num < terrain.alphamapWidth)
        {
            int num2 = 0;
            while (true)
            {
                if (num2 >= terrain.alphamapHeight)
                {
                    num++;
                    break;
                }

                int index = 0;
                while (true)
                {
                    if (index >= numArray.GetLength(2))
                    {
                        num2++;
                        break;
                    }

                    writer.Write(numArray[num, num2, index]);
                    index++;
                }
            }
        }
        writer.Close();
    }

    void LoadTerrainAlpha(string levelName, TerrainData terrain)
    {

        string folderPath = Application.dataPath + "/StreamingAssets/Levels/" + levelName;
        string filePath = Path.Combine(folderPath, levelName + "_Splat.txt");


        float[,,] numArray = terrain.GetAlphamaps(0, 0, terrain.alphamapWidth, terrain.alphamapHeight);
        BinaryReader reader = new BinaryReader(new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None));
        //reader.BaseStream.Seek(0, SeekOrigin.Begin);

        int num = 0;
        while (num < terrain.alphamapWidth)
        {
            int num2 = 0;
            while (true)
            {
                if (num2 >= terrain.alphamapHeight)
                {
                    num++;
                    break;
                }

                int index = 0;
                while (true)
                {
                    if (index >= numArray.GetLength(2))
                    {
                        num2++;
                        break;
                    }

                    numArray[num, num2, index] = (float)reader.ReadSingle();
                    index++;
                }
            }
        }

        terrain.SetAlphamaps(0, 0, numArray);
        reader.Close();
    }

    void SaveTerrainHeight(string levelName, TerrainData terrain)
    {
        string folderPath = Application.dataPath + "/StreamingAssets/Levels/" + levelName;
        string filePath = Path.Combine(folderPath, levelName + "_Heightmap.txt");

        float[,] dat = terrain.GetHeights(0, 0, terrain.heightmapResolution, terrain.heightmapResolution);
        FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
        BinaryWriter bw = new BinaryWriter(fs);
        for (int i = 0; i < terrain.heightmapResolution; i++)
        {
            for (int j = 0; j < terrain.heightmapResolution; j++)
            {
                bw.Write(dat[i, j]);
            }
        }
        bw.Close();
    }

    void LoadTerrainHeight(string levelName, TerrainData terrain)
    {
        CheckLevelFolder(levelName);
        string folderPath = Application.dataPath + "/StreamingAssets/Levels/" + levelName;
        string filePath = Path.Combine(folderPath, levelName + "_Heightmap.txt");
        float[,] dat = terrain.GetHeights(0, 0, terrain.heightmapResolution, terrain.heightmapResolution);
        FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        BinaryReader br = new BinaryReader(fs);
        br.BaseStream.Seek(0, SeekOrigin.Begin);
        for (int i = 0; i < terrain.heightmapResolution; i++)
        {
            for (int j = 0; j < terrain.heightmapResolution; j++)
            {
                dat[i, j] = (float)br.ReadSingle();
            }
        }
        br.Close();
        terrain.SetHeights(0, 0, dat);
        // heights = terrain.GetHeights(50, 50, 100, 100);
    }

    void CheckLevelFolder(string levelName)
    {
        if (!System.IO.Directory.Exists(Application.dataPath + "/StreamingAssets/Levels"))
        {
            System.IO.Directory.CreateDirectory(Application.dataPath + "/StreamingAssets/Levels");


        }
        if (!System.IO.Directory.Exists(Application.dataPath + "/StreamingAssets/Levels/" + levelName))
        {
            System.IO.Directory.CreateDirectory(Application.dataPath + "/StreamingAssets/levels/" + levelName);


        }
    }

    async void LoadGltfBinaryFromMemory()
    {
        var filePath = "/path/to/file.glb";
        byte[] data = File.ReadAllBytes(filePath);
        var gltf = new GltfImport();
        bool success = await gltf.LoadGltfBinary(
            data,
            // The URI of the original data is important for resolving relative URIs within the glTF
            new Uri(filePath)
            );
        if (success)
        {
            success = await gltf.InstantiateMainSceneAsync(transform);
        }
    }

}
