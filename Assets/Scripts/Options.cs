using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Options : MonoBehaviour
{
    /**podsetnik*/
    /*
    {
            string json = JsonUtility.ToJson(settings);// citaj klasu kao json string

            settings = JsonUtility.FromJson<Settings>(json);// pisi klasu od json stringa

            File.ReadAllText(Application.dataPath + "/Wision5252/klipik.rez");// citaj json kao string

            File.WriteAllText(Application.dataPath + "/Wision5252/klipik.rez", json);//pisi u json kao json string

            JsonUtility.FromJsonOverwrite(json, settings);//pisi u klasu kao json string
    }*/
    public Toggle fullScreenToggle;
    public Slider Fps;
    public Slider[] VolumeSliders = new Slider[4];
    public AudioMixer audioMixer;

    public class Settings
    {
        public bool fullScreen = true;
        public int fps = 60;
        public float[] volumes = { 1, 1, 1, 1 };
        public bool vsync = false;
        /*public float volume0 = 1;
        public float volume1 = 1;
        public float volume2 = 1;
        public float volume3 = 1;*/

    }
    Settings settings;
    public static Options Instance { get; private set; }

    private void Awake()
    {//ptickixcce idu tut tut
        settings = JsonUtility.FromJson<Settings>(File.ReadAllText(Application.dataPath + "/StreamingAssets/klipik.rez"));
        Instance = this;
    }

    void Start()
    {
        fullScreenToggle.isOn = settings.fullScreen;
        Fps.value = settings.fps;
        UpdateSettings(settings.fullScreen ? 1 : 0, settings.fps);

        for (int i = 0; i < VolumeSliders.Length; i++)
        {
            VolumeSliders[i].value = settings.volumes[i] * 100;
        }
        VsyncValue(settings.vsync);//ovo nije potrebno

    }


    public void FullScreenValue(bool value)
    {
        if (settings != null)
        {
            settings.fullScreen = value;
            UpdateSettings(value ? 1 : 0, settings.fps);
        }

    }

    public void FpsValue(float value)
    {
        if (settings != null)
        {
            settings.fps = (int)value;
            VsyncValue(value <= 0.1f ? true : false);
            UpdateSettings(settings.fullScreen ? 1 : 0, (int)value);
        }
    }

    public void VsyncValue(bool value)
    {
        if (settings != null)
        {
            settings.vsync = value;
            QualitySettings.vSyncCount = value ? 1 : 0;
        }
    }

    public void Volume0Value(float value)
    {
        VolumeValue(value, 0);
    }
    public void Volume1Value(float value)
    {
        VolumeValue(value, 1);
    }
    public void Volume2Value(float value)
    {
        VolumeValue(value, 2);
    }
    public void Volume3Value(float value)
    {
        VolumeValue(value, 3);
    }
    void VolumeValue(float value, int index)
    {
        if (settings != null)
        {
            value /= 100;
            settings.volumes[index] = value;
            audioMixer.SetFloat(index.ToString(), (Mathf.Log10(value) * 20) != float.NegativeInfinity ? Mathf.Log10(value) * 20 : -52);
        }
    }

    void UpdateSettings(int fullScreen = -1, int hz = -1)
    {
        Application.targetFrameRate = hz != -1 ? (hz) : Application.targetFrameRate;

        Screen.SetResolution(
            Screen.width,
             Screen.height,
              fullScreen != -1 ? (fullScreen == 1 ? true : false) : Screen.fullScreen,
               hz != -1 ? (hz) : Application.targetFrameRate);

        File.WriteAllText(Application.dataPath + "/StreamingAssets/klipik.rez", JsonUtility.ToJson(settings));//update setings json
    }

    /*
        public void FullScreen(bool value)
        {
            OptionsManager.Instance.SetFullScreen(value);
            //UpdateUi();
        }

        public void SetVolume1(float value)
        {
            OptionsManager.Instance.SetVolume(value / 100, 0);
            //UpdateVolumeUi(value, 0); nediraj pokvareno
        }
        public void SetVolume2(float value)
        {
            OptionsManager.Instance.SetVolume(value / 100, 1);
            //UpdateVolumeUi(value, 1);
        }
        public void SetVolume3(float value)
        {
            OptionsManager.Instance.SetVolume(value / 100, 2);
            //UpdateVolumeUi(value, 2);
        }
        public void SetVolume4(float value)
        {
            OptionsManager.Instance.SetVolume(value / 100, 3);
            //UpdateVolumeUi(value, 3);
        }
        public void SetVolume(float value, int index)
        {
            OptionsManager.Instance.SetVolume(value / 100, index);
            //UpdateVolumeUi(value, index);
        }


        public void SetFramerate(float value)
        {
            OptionsManager.Instance.SetFramerate(value);
            //UpdateUi();
        }

        public void SetVSync(bool value)
        {

            OptionsManager.Instance.SetVSync(value);
            //UpdateUi();
        }*/
    /*
        public void UpdateUi()
        {
            Fps.value = PlayerPrefs.GetInt("fps");
            fullScreenToggle.isOn = PlayerPrefs.GetInt("fullScreen") == 1 ? true : false;
            for (int i = 0; i <= 3; i++)
            {
                if (PlayerPrefs.HasKey("Volume" + i))
                {
                    //SetVolume(PlayerPrefs.GetFloat("Volume" + i), i);
                    VolumeSliders[i].value = (PlayerPrefs.GetFloat("Volume" + i) * 100);
                }
            }
        }*/

}
