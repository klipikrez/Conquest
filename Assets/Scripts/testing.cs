using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class testing : MonoBehaviour
{
    public Terrain terrain1;
    public Terrain terrain2;

    // Start is called before the first frame update
    void Start()
    {

        SaveTerrain("bababoj", terrain1.terrainData);
        LoadTerrain("bababoj", terrain2.terrainData);
    }

    // Update is called once per frame
    void Update()
    {

    }
    void SaveTerrain(string levelName, TerrainData terrain)
    {

        CheckLevelFolder(levelName);
        SaveTerrainHeight(levelName, terrain);
        SaveTerrainAlpha(levelName, terrain);
        SaveTerrainTrees(levelName, terrain);
        SaveTerrainDetails(levelName, terrain);
    }

    void LoadTerrain(string levelName, TerrainData terrain)
    {
        CheckLevelFolder(levelName);
        LoadTerrainDetails(levelName, terrain);
        LoadTerrainHeight(levelName, terrain);
        LoadTerrainAlpha(levelName, terrain);
        LoadTerrainTrees(levelName, terrain);

    }

    [System.Serializable]
    public class TerrainTrees
    {
        public TerrainTrees(TreeInstance[] instances, TreePrototype[] prototypes)
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
    public class TerrainDetails
    {
        public TerrainDetails(DetailPrototype[] instances)
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
            Debug.Log(Resources.Load("Folage/" + this.prototypeName) as GameObject);
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
            return detailPrototype;
        }
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

        TerrainDetails details = new TerrainDetails(workPrototypes);

        string folderPath = "Assets\\StreamingAssets\\Levels\\" + levelName;
        string filePath = Path.Combine(folderPath, levelName + "_Details.rez");
        File.WriteAllText(filePath, JsonUtility.ToJson(details));//update setings json

        //ovde je deo sto ne valja
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
                    bw.Write(thisDetailLayer[i, j]);
                }
            }
            bw.Close();
        }



    }

    void LoadTerrainDetails(string levelName, TerrainData terrain)
    {

        string folderPath = "Assets\\StreamingAssets\\Levels\\" + levelName;
        string filePath = Path.Combine(folderPath, levelName + "_Details.rez");

        TerrainDetails details = JsonUtility.FromJson<TerrainDetails>(File.ReadAllText(filePath));//update setings json

        List<DetailPrototype> clonedDetails = new List<DetailPrototype>();
        for (int ti = 0; ti < details.details.Length; ti++)
        {
            clonedDetails.Add(details.details[ti].Copy());

        }
        terrain.detailPrototypes = clonedDetails.ToArray();


        renderTexture = new RenderTexture(renderTexture.width, renderTexture.height, 24);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();
        Color[] pixels = new Color[renderTexture.width * renderTexture.height];
        //tex = new Texture2D(renderTexture.width, renderTexture.width);
        //renderer.material.mainTexture = texture;
        //RenderTexture.active = renderTexture;
        //don't forget that you need to specify rendertexture before you call readpixels
        //otherwise it will read screen pixels.
        //texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        int layNum = 0;
        while (true)
        {


            string path = Application.streamingAssetsPath + "/Levels/" + levelName + "/" + levelName + "_DetailMap" + layNum + ".rez";
            Debug.Log(path);


            if (!File.Exists(path)) break;

            int[,] dat = terrain.GetDetailLayer(0, 0, terrain.detailWidth, terrain.detailHeight, layNum);
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            BinaryReader br = new BinaryReader(fs);
            //br.BaseStream.Seek(0, SeekOrigin.Begin);
            Debug.Log(terrain.detailHeight + " " + terrain.detailWidth);
            for (int i = 0; i < terrain.detailHeight; i++)
            {
                int au = 1;

                for (int j = 0; j < terrain.detailWidth; j++)
                {
                    au = (int)br.Read();
                    pixels[i * terrain.detailWidth + j] = new Color(0, au > 0 ? 1 : 0, 0, 1);
                    //texture.SetPixel(i, j, new Color(1, 0, 0));

                    dat[i, j] = au;
                }

                Debug.Log(au);
            }
            br.Close();
            terrain.SetDetailLayer(0, 0, layNum++, dat);


        }        //tex.SetPixels(pixels);
                 //tex.Apply();
        tex.SetPixels(pixels);
        tex.Apply();
    }

    int at = 0;

    public RenderTexture renderTexture; // renderTextuer that you will be rendering stuff on
                                        // public Renderer renderer; // renderer in which you will apply changed texture
    public Texture2D tex;

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


        TerrainTrees trees = new TerrainTrees(workTrees, workTreePrototypes);

        string folderPath = "Assets\\StreamingAssets\\Levels\\" + levelName;
        string filePath = Path.Combine(folderPath, levelName + "_Trees.rez");
        File.WriteAllText(filePath, JsonUtility.ToJson(trees));//update setings json

    }

    void LoadTerrainTrees(string levelName, TerrainData terrain)
    {



        string folderPath = "Assets\\StreamingAssets\\Levels\\" + levelName;
        string filePath = Path.Combine(folderPath, levelName + "_Trees.rez");
        TerrainTrees trees = JsonUtility.FromJson<TerrainTrees>(File.ReadAllText(filePath));//update setings json



        List<TreeInstance> clonedTree = new List<TreeInstance>();
        for (int ti = 0; ti < trees.workTrees.Length; ti++)
        {
            clonedTree.Add(trees.workTrees[ti].Copy());
        }
        terrain.treeInstances = clonedTree.ToArray();


        List<TreePrototype> workTreePrototypes = new List<TreePrototype>();
        for (int tp = 0; tp < trees.workTreePrototypes.Length; tp++)
        {
            workTreePrototypes.Add(trees.workTreePrototypes[tp].Copy());
        }
        terrain.treePrototypes = workTreePrototypes.ToArray();



    }

    private void SaveTerrainAlpha(string levelName, TerrainData terrainData)
    {

        string folderPath = "Assets\\StreamingAssets\\Levels\\" + levelName;
        string filePath = Path.Combine(folderPath, levelName + "_Splat.txt");


        float[,,] numArray = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
        BinaryWriter writer = new BinaryWriter(new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None));
        int num = 0;
        while (num < terrainData.alphamapWidth)
        {
            int num2 = 0;
            while (true)
            {
                if (num2 >= terrainData.alphamapHeight)
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

    private void LoadTerrainAlpha(string levelName, TerrainData terrainData)
    {

        string folderPath = "Assets\\StreamingAssets\\Levels\\" + levelName;
        string filePath = Path.Combine(folderPath, levelName + "_Splat.txt");


        float[,,] numArray = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
        BinaryReader reader = new BinaryReader(new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None));
        //reader.BaseStream.Seek(0, SeekOrigin.Begin);

        int num = 0;
        while (num < terrainData.alphamapWidth)
        {
            int num2 = 0;
            while (true)
            {
                if (num2 >= terrainData.alphamapHeight)
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

        terrainData.SetAlphamaps(0, 0, numArray);
        reader.Close();
    }

    void SaveTerrainHeight(string levelName, TerrainData terrain)
    {
        string folderPath = "Assets\\StreamingAssets\\Levels\\" + levelName;
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
        string folderPath = "Assets\\StreamingAssets\\Levels\\" + levelName;
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
        if (!AssetDatabase.IsValidFolder("Assets/StreamingAssets/Levels"))
        {
            string guid = AssetDatabase.CreateFolder("Assets/StreamingAssets", "Levels");


        }
        if (!AssetDatabase.IsValidFolder("Assets/StreamingAssets/Levels/" + levelName))
        {
            string guid = AssetDatabase.CreateFolder("Assets/StreamingAssets/levels", levelName);


        }
    }

}
