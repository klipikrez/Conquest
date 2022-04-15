using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OptionsManager : MonoBehaviour
{/*
    public AudioMixer main;
    Options optionsRef;
    public static OptionsManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        Debug.Log("znak zivota");


    }

    private void Start()
    {
        UpdateResolution();
    }

    public void FullScreenValue(bool value)
    {
        Screen.fullScreen = value;
        PlayerPrefs.SetInt("fullScreen", value ? 1 : 0);
    }
*/
    /*
        // Start is called before the first frame update
        void Start()
        {
            if (!PlayerPrefs.HasKey("fullScreen"))
            {
                PlayerPrefs.SetInt("fullScreen", 1);
            }
            Options.Instance.fullScreenToggle.isOn = (PlayerPrefs.GetInt("fullScreen", 1) == 1 ? true : false);
            if (!PlayerPrefs.HasKey("fps"))
            {
                PlayerPrefs.SetInt("fps", 60);
            }
            if (!PlayerPrefs.HasKey("VSync"))
            {
                PlayerPrefs.SetInt("VSync", PlayerPrefs.GetInt("fps") <= 0 ? 1 : 0);
            }
            for (int i = 0; i <= 3; i++)
            {
                if (!PlayerPrefs.HasKey("Volume" + i))
                {
                    PlayerPrefs.SetFloat("Volume" + i, 1);
                    //Options.Instance.UpdateVolumeUi(PlayerPrefs.GetFloat("Volume" + i) * 100, i);
                }
                SetVolume(PlayerPrefs.GetFloat("Volume" + i), i);
            }
            UpdateResolution();
            //Options.Instance.UpdateUi();
            Debug.Log("fullScreen" + ": " + (PlayerPrefs.GetInt("fullScreen") == 1 ? true : false));


        }

        public void SetFullScreen(bool value)
        {
            PlayerPrefs.SetInt("fullScreen", value ? 1 : 0);

            UpdateResolution();
        }

        public void SetFramerate(float value)
        {
            PlayerPrefs.SetInt("fps", (int)value);
            Debug.Log("valjda " + PlayerPrefs.GetInt("fps"));
            if (value <= 0.001f)
            {
                if (QualitySettings.vSyncCount != 1)
                    SetVSync(true);
            }
            else
            {
                if (QualitySettings.vSyncCount != 0)
                    SetVSync(false);
            }
            UpdateResolution();

        }

        public void SetVSync(bool value)
        {
            QualitySettings.vSyncCount = value ? 1 : 0;
            PlayerPrefs.SetInt("VSync", value ? 1 : 0);

            UpdateResolution();
        }

        public void SetVolume(float value, int index)
        {
            PlayerPrefs.SetFloat("Volume" + index, value);
            main.SetFloat(index.ToString(), (Mathf.Log10(value) * 20) != float.NegativeInfinity ? Mathf.Log10(value) * 20 : -52);
        }

       *//*
    void UpdateResolution()
    {
        Screen.SetResolution(Screen.width, Screen.height, PlayerPrefs.GetInt("fullScreen") == 1 ? true : false, PlayerPrefs.GetInt("fps"));
        Application.targetFrameRate = PlayerPrefs.GetInt("fps");

    }

*/

}
