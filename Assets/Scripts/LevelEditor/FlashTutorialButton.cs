using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashTutorialButton : MonoBehaviour
{

    public int toolID = 0;
    // Start is called before the first frame update
    void OnEnable()
    {
        Settings settings = Options.GetSettings();
        if (!settings.doneToolTotorial[toolID])
        {
            Debug.Log("Flash " + toolID);
            gameObject.GetComponent<Animator>().Play("Flash");
        }
    }

    public void DisableFlashing()
    {
        gameObject.GetComponent<Animator>().Play("infoStart");
        Settings settings = Options.GetSettings();
        settings.doneToolTotorial[toolID] = true;
        Options.SetSettings(settings);
    }

}
