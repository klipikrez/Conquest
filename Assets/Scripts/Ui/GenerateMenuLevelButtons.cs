using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GenerateMenuLevelButtons : MonoBehaviour
{

    public GameObject levelPrefab;
    public LevelInfoManage levelManager;

    // Start is called before the first frame update
    void Start()
    {
        CheckLevelFolder();
        string folderPath = Application.dataPath + "/StreamingAssets/Levels";

        string[] dir = Directory.GetDirectories(folderPath);
        foreach (string dirName in dir)
        {
            MenuLevel level = GameObject.Instantiate(levelPrefab, transform).GetComponent<MenuLevel>();
            level.Initialize(dirName, levelManager);

        }

    }

    void CheckLevelFolder()
    {
        if (!System.IO.Directory.Exists(Application.dataPath + "/StreamingAssets/Levels"))
        {
            System.IO.Directory.CreateDirectory(Application.dataPath + "/StreamingAssets/Levels");


        }
    }

    private bool IsJson(string fileName)
    {
        string extension = Path.GetExtension(fileName).ToLower();
        return extension == ".rez";
    }
}
