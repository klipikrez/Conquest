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
            Settings settings = Options.GetSettings();
            settings.showEditorTutorial = false;
            Options.SetSettings(settings);
        }
    }
}
