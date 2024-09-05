using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class IntroTutorialCard : MonoBehaviour
{
    public Toggle toggle;
    public void CloseTutorial()
    {
        if (toggle != null && !toggle.isOn)
        {
            Settings settings = JsonUtility.FromJson<Settings>(File.ReadAllText(Application.dataPath + "/StreamingAssets/klipik.rez"));
            settings.showEditorTutorial = false;
            File.WriteAllText(Application.dataPath + "/StreamingAssets/klipik.rez", JsonUtility.ToJson(settings));//update setings json
        }
    }
}
