using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class GenerateMenuLevelButtons : MonoBehaviour
{

    public GameObject levelPrefab;
    public EditorLevelInfoManage levelManager;
    public bool includeOnlyOfficialLevels = false;

    // Start is called before the first frame update
    void Awake()
    {
        CheckLevelFolder();
        string folderPath = Application.dataPath + "/StreamingAssets/Levels";

        string[] dir = Directory.GetDirectories(folderPath);
        foreach (string dirName in dir)
        {
            if (includeOnlyOfficialLevels && !Regex.IsMatch(Path.GetFileName(dirName), @"^[0-9]+-[0-9]+"))
            {
                //Debug.Log("Skipping non-official level: " + Path.GetFileName(dirName));
                continue;
            }
            MenuLevel level = GameObject.Instantiate(levelPrefab, transform).GetComponent<MenuLevel>();
            level.Initialize(
                dirName.Substring(dirName.LastIndexOf('\\') + 1)
                , levelManager, includeOnlyOfficialLevels);

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
