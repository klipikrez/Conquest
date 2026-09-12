using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;


public class ActivateAfterCampaign : MonoBehaviour
{

    // Start is called before the first frame update
    void OnEnable()
    {
        Settings settings = JsonUtility.FromJson<Settings>(File.ReadAllText(Application.dataPath + "/StreamingAssets/klipik.rez"));

        gameObject.GetComponent<Button>().interactable = CampaignComplete();
    }
    public bool CampaignComplete()
    {
        CheckLevelFolder();
        string folderPath = Application.dataPath + "/StreamingAssets/Levels";
        if (!Directory.Exists(folderPath))
        {
            return false;
        }

        Settings settings = JsonUtility.FromJson<Settings>(File.ReadAllText(Application.dataPath + "/StreamingAssets/klipik.rez"));

        int officialLevelCount = 0;
        string[] dirs = Directory.GetDirectories(folderPath);
        foreach (string dirName in dirs)
        {
            if (Regex.IsMatch(Path.GetFileName(dirName), @"^[0-9]+-[0-9]+"))
            {
                officialLevelCount++;
            }
        }

        return settings.campaignLevel >= officialLevelCount;
    }

    void CheckLevelFolder()
    {
        if (!System.IO.Directory.Exists(Application.dataPath + "/StreamingAssets/Levels"))
        {
            System.IO.Directory.CreateDirectory(Application.dataPath + "/StreamingAssets/Levels");


        }
    }

}
