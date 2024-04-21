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
        CheckLevelFolder();
        SaveTerrain("bababoj", terrain1.terrainData);
        Debug.Log(terrain1.terrainData.heightmapResolution);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SaveTerrain(string filename, TerrainData terrain)
    {


        string folderPath = Path.Combine("..", "StreamingAssets/Levels"); // ".." moves up one directory level to "App" directory
        string filePath = Path.Combine(folderPath, filename + ".txt");

        float[,] dat = terrain.GetHeights(0, 0, terrain.heightmapResolution, terrain.heightmapResolution);
        FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
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

    void LoadTerrain(string filename, TerrainData terrain)
    {
        float[,] dat = terrain.GetHeights(0, 0, terrain.heightmapResolution, terrain.heightmapResolution);
        FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite);
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

    void CheckLevelFolder()
    {
        if (!AssetDatabase.IsValidFolder("Assets/StreamingAssets/Levels"))
        {
            string guid = AssetDatabase.CreateFolder("Assets/StreamingAssets", "Levels");
            string newFolderPath = AssetDatabase.GUIDToAssetPath(guid);
            Debug.Log(guid);
        }
    }

}
