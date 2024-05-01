using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
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

        TerrainDetailsCompiled details = new TerrainDetailsCompiled(workPrototypes);

        string folderPath = "Assets\\StreamingAssets\\Levels\\" + levelName;
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

    void LoadTerrainDetails(string levelName, TerrainData terrain)
    {

        string folderPath = "Assets\\StreamingAssets\\Levels\\" + levelName;
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


            string path = Application.streamingAssetsPath + "/Levels/" + levelName + "/" + levelName + "_DetailMap" + layNum + ".rez";


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

        string folderPath = "Assets\\StreamingAssets\\Levels\\" + levelName;
        string filePath = Path.Combine(folderPath, levelName + "_Trees.rez");
        File.WriteAllText(filePath, JsonUtility.ToJson(trees));//update setings json

    }

    void LoadTerrainTrees(string levelName, TerrainData terrain)
    {



        string folderPath = "Assets\\StreamingAssets\\Levels\\" + levelName;
        string filePath = Path.Combine(folderPath, levelName + "_Trees.rez");
        TerrainTreesCompiled trees = JsonUtility.FromJson<TerrainTreesCompiled>(File.ReadAllText(filePath));//update setings json



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

    void SaveTerrainAlpha(string levelName, TerrainData terrain)
    {

        string folderPath = "Assets\\StreamingAssets\\Levels\\" + levelName;
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

        string folderPath = "Assets\\StreamingAssets\\Levels\\" + levelName;
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
        if (!System.IO.Directory.Exists("Assets/StreamingAssets/Levels"))
        {
            System.IO.Directory.CreateDirectory("Assets/StreamingAssets/Levels");


        }
        if (!System.IO.Directory.Exists("Assets/StreamingAssets/Levels/" + levelName))
        {
            System.IO.Directory.CreateDirectory("Assets/StreamingAssets/levels/" + levelName);


        }
    }

}
