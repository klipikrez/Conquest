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
using Yarn;
using Yarn.Compiler;
using Yarn.Unity;
using Yarn.Unity.Example;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

///////////////////////////////////////////////////////////////////
//                                                               //
//     Storage classes - for temp storage of unity classes,      //
//                 to be able to read/write them                 //
//                                                               //
//     Compiled classes -  for easier manipulation of arrays     //
//                                                               //
///////////////////////////////////////////////////////////////////


public class SaveLoadLevel : MonoBehaviour
{
    public Terrain terrain;
    public GameObject towerDefaultPrefab;
    public GameObject editorTowerPrefab;
    [SerializeField]
    public TextAsset yarnProject;
    public DialogueManager dialogueManager;
    public DialogueRunner dialogueRunner;
    public YarnProject project;



    //public Terrain terrain2;

    // Start is called before the first frame update
    void Start()
    {
        //LoadDialogue("Test");
        //SaveTerrain("bababoj", terrain1.terrainData);
        //LoadLevelEditor("brki");
        //ScenesManager.Instance.LoadLevel("trkdsada");
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SaveTerrain()
    {

        string levelName = EditorOptions.Instance.terrainNameInput.text;
        Debug.Log("SaveLevel: " + levelName + " || playerSpawn: " + EditorManager.Instance.playerSpawn);
        if (levelName == "" || levelName == null) { ErrorManager.Instance.SendError("Name your level, bruh..."); return; }
        if (!levelName.All(char.IsLetterOrDigit)) { ErrorManager.Instance.SendError("Only letters and numbers are allowed in level name >:V"); return; }
        if (char.IsDigit(levelName[0])) { ErrorManager.Instance.SendError("The first character in the save name can't be a number :("); return; }
        if (math.abs(EditorManager.Instance.playerSpawn.x) > 200) { ErrorManager.Instance.SendError("Player spawn location not set :P"); return; }
        if (EditorManager.Instance.bounds == null || EditorManager.Instance.bounds.Count <= 2) { ErrorManager.Instance.SendError("Playspace bounds not set >:L"); return; }

        if (levelName.ToLower() == "bob" || levelName.ToLower() == "sajba" || levelName.ToLower() == "kingco" || levelName.ToLower().Contains("teki")) { ErrorManager.Instance.SendError("Nuh Uh ;) change your level name."); return; }

        Debug.Log("Valid level name");
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
        if (levelName == "") return;// if empty string, we want a blank level. dont load anything.
        CheckLevelFolder(levelName);
        LoadTerrainHeight(levelName, terrain.terrainData);
        LoadTerrainAlpha(levelName, terrain.terrainData);
        LoadTerrainTrees(levelName, terrain.terrainData);
        LoadTerrainDetails(levelName, terrain.terrainData, true);
        LoadTerrainTowersEditor(levelName);
        LoadLevelOptionsEditor(levelName);

        EditorOptions.Instance.terrainNameInput.text = levelName;


    }

    public IEnumerator LoadLevelAsync(string levelName)
    {
        CheckLevelFolder(levelName);
        LoadTerrainHeight(levelName, terrain.terrainData);
        LoadTerrainAlpha(levelName, terrain.terrainData, false);
        LoadTerrainTrees(levelName, terrain.terrainData);
        LoadTerrainDetails(levelName, terrain.terrainData, false);
        LoadTerrainTowers(levelName);
        LoadLevelOptions(levelName);
        GameObject.Find("navManager").GetComponent<NavManager>().Inicialize(GameObject.FindGameObjectsWithTag("building"));
        GameObject.Find("TowerAIManager").GetComponent<AIManager>().Inicialize();
        LoadDialogue(levelName);
        yield return null;
    }
    /*
        void CompileDialogueHelper(string speaker, string[] lines)
        {
            // Generate unique node name
            string nodeName = $"Dynamic_{System.Guid.NewGuid()}";

            // Create Yarn script
            string yarnScript = $"title: {nodeName}\n---\n";
            foreach (string line in lines)
            {
                yarnScript += $"{speaker}: {line}\n";
            }
            yarnScript += "===";

            // Compile the script
            var compilationJob = CompilationJob.CreateFromString(nodeName, yarnScript, new Yarn.Library());
            var result = Compiler.Compile(compilationJob);

            // Load the compiled program
            qDialogueRunner.Dialogue.AddProgram(result.Program);

            UnityEngine.Debug.Log("yarnScript = " + yarnScript);
            UnityEngine.Debug.Log("result.Program = " + result.Program);
            UnityEngine.Debug.Log("result.Diagnostics = " + result.Diagnostics);
            UnityEngine.Debug.Log("result.Diagnostics.ToList() = " + result.Diagnostics.ToList());
            UnityEngine.Debug.Log("Join() for result.Diagnostics = " + string.Join(", ", result.Diagnostics.ToList()));
            UnityEngine.Debug.Log("qDialogueRunner.NodeExists(\"TestScript\") = " + qDialogueRunner.NodeExists("TestScript"));
            UnityEngine.Debug.Log("qDialogueRunner.NodeExists(nodeName) = " + qDialogueRunner.NodeExists(nodeName));

            // Start the dialogue
            qDialogueRunner.StartDialogue(nodeName);
        }*/

    void LoadDialogue(string levelName)
    {

        string folderPath = Application.dataPath + "/StreamingAssets/Levels/" + levelName;
        string filePath = Path.Combine(folderPath, "Dialogue.yarn");

        if (!File.Exists(filePath))
        {
            Debug.Log("Failed to load dialaogue for: " + levelName);
            File.WriteAllText(AssetDatabase.GetAssetPath(yarnProject), "52NEM52");
            EditorUtility.SetDirty(yarnProject);
            dialogueManager.DialogueComplete();
            return;
        }

        TextAsset text = new TextAsset(System.IO.File.ReadAllText(filePath));

        if (text == null || text.text == "")
        {
            Debug.Log("Failed to load dialogue for: " + levelName);
            return;
        }

        dialogueManager.Initiate();


        CompileYarnFile(filePath);


        dialogueRunner.startNode = "Start";
        Debug.Log(dialogueRunner.startNode);
        Debug.Log(dialogueRunner.NodeExists(dialogueRunner.startNode));
        //project.NodeNames.Add(dialogueRunner.startNode);
        Debug.Log(project.NodeNames.Length);

        foreach (var s in project.NodeNames)
        {
            Debug.Log(s);
        }
        //dialogueRunner.SetProject(project);
        dialogueManager.StartDialogue("Start");
        foreach (var v in dialogueRunner.Dialogue.NodeNames)
        {
            Debug.Log(v);
        }
    }


    public void CompileYarnFile(string filePath)
    {
        /*try
        {*/
        // Read the Yarn file
        string yarnSource = File.ReadAllText(filePath);

        // Create a CompilationJob
        var compilationJob = CompilationJob.CreateFromString("Start", yarnSource);

        // Compile the Yarn file
        var result = Compiler.Compile(compilationJob);
        Debug.Log(result.StringTable);

        foreach (var s in result.StringTable)
        {
            Debug.Log(s.Key + " " + s.Value);
        }



        dialogueRunner.Dialogue.SetProgram(result.Program);


        foreach (var v in dialogueRunner.Dialogue.NodeNames)
        {
            Debug.Log(v);
        }
        dialogueRunner.yarnProject.compiledYarnProgram = File.ReadAllBytes(filePath);
        Debug.Log("TELL MEEEEE::::" + dialogueRunner.yarnProject.NodeNames.Length);




        // Save the compiled bytecode (yarn program)
        //SaveYarnProgram(result.Program);

        // Save the string table
        //SaveStringTable(result.StringTable);
        /* }
         catch (Exception e)
         {
             Debug.LogError($"Failed to compile Yarn file: {e.Message}");
         }*/
    }

    private void SaveYarnProgram(byte[] program)
    {
        string programPath = Path.Combine(Application.persistentDataPath, "compiled.yarnc");
        File.WriteAllBytes(programPath, program);
        Debug.Log($"Yarn program saved at: {programPath}");
    }

    private void SaveStringTable(Dictionary<string, StringInfo> stringTable)
    {
        string stringTablePath = Path.Combine(Application.persistentDataPath, "strings.json");

        var stringTableJson = JsonUtility.ToJson(stringTable);
        File.WriteAllText(stringTablePath, stringTableJson);

        Debug.Log($"String table saved at: {stringTablePath}");
    }


    void SaveYarnScriptToFile(string fileName, string content)
    {
        string path = Path.Combine(Application.dataPath, fileName); // Save in the Assets folder
        File.WriteAllText(path, content);
        Debug.Log($"Yarn script saved to: {path}");
    }

    void SaveTerrainTowers(string levelName)
    {

        TowerCompiled towers = new TowerCompiled(EditorManager.Instance.editorTowers.ToArray());
        string s = "";
        foreach (var t in EditorManager.Instance.editorTowers.ToArray())
        {
            s += " -" + t.selfID;
        }
        Debug.Log("savedLevelTowersIds:" + s);

        string folderPath = Application.dataPath + "/StreamingAssets/Levels/" + levelName;
        string filePath = Path.Combine(folderPath, "Towers.rez");
        File.WriteAllText(filePath, JsonUtility.ToJson(towers, true));//update setings json


    }

    void LoadTerrainTowers(string levelName)
    {
        string folderPath = Application.dataPath + "/StreamingAssets/Levels/" + levelName;
        string filePath = Path.Combine(folderPath, "Towers.rez");

        if (!File.Exists(filePath))
        {
            Debug.Log("Failed to load towers for: " + levelName);
            return;
        }

        TowerCompiled towers = JsonUtility.FromJson<TowerCompiled>(File.ReadAllText(filePath));//update setings json

        if (towers == null)
        {
            towers = new TowerCompiled(new EditorTower[0]);
            Debug.Log("Failed to load towers for: " + levelName);
        }
        towers.LoadPresets();
        towers.InstantiateInLevel(towerDefaultPrefab);
    }

    void LoadTerrainTowersEditor(string levelName)
    {
        EditorTower.TowerIDs.Clear();
        string folderPath = Application.dataPath + "/StreamingAssets/Levels/" + levelName;
        string filePath = Path.Combine(folderPath, "Towers.rez");

        TowerCompiled towers;

        if (!File.Exists(filePath))
        {
            towers = new TowerCompiled(new EditorTower[0]);
            Debug.Log("Failed to load towers for: " + levelName);
        }
        else
        {
            towers = JsonUtility.FromJson<TowerCompiled>(File.ReadAllText(filePath));//update setings json
            string s = "";
            foreach (var t in towers.towers)
            {
                s += " -" + t.id;
            }
            Debug.Log("LoadedEditorTowerIds:" + s);
        }
        towers.LoadPresets();
        towers.InstantiateInLevel(editorTowerPrefab, true);
    }

    void SaveLevelOptionsEditor(string levelName)
    {
        LevelOptions options = new LevelOptions
        {
            playerPos = levelName != "" ? EditorManager.Instance.playerSpawn : Vector3.negativeInfinity,
            boundsPoints = EditorManager.Instance.bounds.ToArray()
        };
        string folderPath = Application.dataPath + "/StreamingAssets/Levels/" + levelName;
        string filePath = Path.Combine(folderPath, "Options.rez");
        File.WriteAllText(filePath, JsonUtility.ToJson(options, true));//update setings json
    }

    void LoadLevelOptionsEditor(string levelName)
    {

        string folderPath = Application.dataPath + "/StreamingAssets/Levels/" + levelName;
        string filePath = Path.Combine(folderPath, "Options.rez");

        LevelOptions options;

        if (!File.Exists(filePath))
        {
            options = new LevelOptions();
            Debug.Log("Failed to load options for: " + levelName);
            return;
        }

        options = JsonUtility.FromJson<LevelOptions>(File.ReadAllText(filePath));//update setings json


        if (options == null)
        {
            options = new LevelOptions();
            Debug.Log("Failed to load options for (content mismatch): " + levelName);
        }

        EditorManager.Instance.playerSpawn = options.playerPos;
        if (options.boundsPoints != null)
            EditorManager.Instance.bounds.AddRange(options.boundsPoints);
    }

    void LoadLevelOptions(string levelName)
    {
        string folderPath = Application.dataPath + "/StreamingAssets/Levels/" + levelName;
        string filePath = Path.Combine(folderPath, "Options.rez");

        if (!File.Exists(filePath))
        {
            Debug.Log("Failed to load options for: " + levelName);
            return;
        }

        LevelOptions options = JsonUtility.FromJson<LevelOptions>(File.ReadAllText(filePath));//update setings json

        if (options == null)
        {
            options = new LevelOptions();
            Debug.Log("Failed to load options for (content mismatch): " + levelName);
            return;
        }

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) Debug.Log("No player found on loading level");

        player.transform.position = options.playerPos + Vector3.up * player.GetComponent<playerMovement>().zoomLevel;

        DynamicMeshGenerator boundGenerator = GameObject.FindGameObjectWithTag("bounds").GetComponent<DynamicMeshGenerator>();
        if (boundGenerator == null) Debug.Log("No bound generator found in level...");

        boundGenerator.SetMeshOnPlay(options.boundsPoints);
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
        string filePath = Path.Combine(folderPath, "Details.rez");
        File.WriteAllText(filePath, JsonUtility.ToJson(details, true));//update setings json


        int numDetailLayers = terrain.detailPrototypes.Length;
        for (int layNum = 0; layNum < numDetailLayers; layNum++)
        {
            filePath = Path.Combine(folderPath, "DetailMap" + layNum + ".rez");
            float[,] thisDetailLayer = EditorManager.Instance.folage;

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

    void LoadTerrainDetails(string levelName, TerrainData terrain, bool inEditor)
    {

        string folderPath = Application.dataPath + "/StreamingAssets/Levels/" + levelName;
        string filePath = Path.Combine(folderPath, "Details.rez");
        TerrainDetailsCompiled details;
        if (!File.Exists(filePath))
        {
            details = new TerrainDetailsCompiled(new DetailPrototype[0]);
            Debug.Log("Failed to load foliage for: " + levelName);
        }
        else
        {
            details = JsonUtility.FromJson<TerrainDetailsCompiled>(File.ReadAllText(filePath));//update setings json
            if (details == null)
            {
                Debug.Log("Failed to load foliage for (content mismatch): " + levelName);
                return;
            }
        }


        List<DetailPrototype> clonedDetails = new List<DetailPrototype>();
        for (int ti = 0; ti < details.details.Length; ti++)
        {
            clonedDetails.Add(details.details[ti].Copy());

        }
        terrain.detailPrototypes = clonedDetails.ToArray();


        int layNum = 0;
        while (true)
        {


            string path = Application.dataPath + "/StreamingAssets/Levels/" + levelName + "/" + "DetailMap" + layNum++ + ".rez";


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

            if (inEditor)
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
        string filePath = Path.Combine(folderPath, "Details.rez");
        File.WriteAllText(filePath, JsonUtility.ToJson(details, true));//update setings json


        int numDetailLayers = terrain.detailPrototypes.Length;
        for (int layNum = 0; layNum < numDetailLayers; layNum++)
        {
            filePath = Path.Combine(folderPath, "DetailMap" + layNum + ".rez");
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
        string filePath = Path.Combine(folderPath, "Details.rez");

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


            string path = Application.dataPath + "/StreamingAssets/Levels/" + levelName + "/" + "DetailMap" + layNum + ".rez";


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
        string filePath = Path.Combine(folderPath, "Trees.rez");
        File.WriteAllText(filePath, JsonUtility.ToJson(trees, true));//update setings json

    }

    void LoadTerrainTrees(string levelName, TerrainData terrain)
    {



        string folderPath = Application.dataPath + "/StreamingAssets/Levels/" + levelName;
        string filePath = Path.Combine(folderPath, "Trees.rez");
        TerrainTreesCompiled trees;
        if (!File.Exists(filePath))
        {
            trees = new TerrainTreesCompiled(new TreeInstance[0], new TreePrototype[0]);
            Debug.Log("Failed to load trees for: " + levelName);
        }
        else
        {
            trees = JsonUtility.FromJson<TerrainTreesCompiled>(File.ReadAllText(filePath));//update setings json
            if (trees == null)
            {
                Debug.Log("Failed to load trees for (content mismatch): " + levelName);
                return;
            }
        }




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
        string filePath = Path.Combine(folderPath, "Splat.txt");


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


        folderPath = Application.dataPath + "/StreamingAssets/Levels/" + levelName;
        filePath = Path.Combine(folderPath, "SplatLayers.txt");

        LayersCompiled layersCompiled = new LayersCompiled(EditorManager.Instance.terrain.terrainData.terrainLayers.ToArray());

        File.WriteAllText(filePath, JsonUtility.ToJson(layersCompiled, true));//update setings json

        writer.Close();
    }

    void LoadTerrainAlpha(string levelName, TerrainData terrain, bool isEditor = true)
    {

        terrain.terrainLayers = new TerrainLayer[0];

        string folderPath = Application.dataPath + "/StreamingAssets/Levels/" + levelName;
        string filePath = Path.Combine(folderPath, "SplatLayers.txt");

        if (!System.IO.File.Exists(filePath))
        {
            Debug.Log("Failed to load terrain layers for:" + filePath);
            return;
        }

        LayersCompiled layersCompiled = JsonUtility.FromJson<LayersCompiled>(File.ReadAllText(filePath));//update setings json

        layersCompiled.Load(terrain, isEditor);



        folderPath = Application.dataPath + "/StreamingAssets/Levels/" + levelName;
        filePath = Path.Combine(folderPath, "Splat.txt");

        if (!File.Exists(filePath))
        {
            Debug.Log("Failed to load splat for: " + levelName);
            return;
        }

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

                    if (index >= layersCompiled.layers.Length)
                    {
                        num2++;
                        break;
                    }
                    //Debug.Log(num + " -- " + num2 + " -- " + index + " || " + numArray.GetLength(0) + " - " + numArray.GetLength(1) + " - " + numArray.GetLength(2));
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
        string filePath = Path.Combine(folderPath, "Heightmap.txt");

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
        string filePath = Path.Combine(folderPath, "Heightmap.txt");

        if (!File.Exists(filePath))
        {
            Debug.Log("Failed to load height for: " + levelName);
            return;
        }

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