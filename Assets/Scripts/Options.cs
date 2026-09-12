using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Settings
{
    public bool fullScreen = true;
    public int fps = 60;
    public float[] volumes = { 1, 1, 1, 1 };
    public bool vsync = true;
    public bool showEditorTutorial = true;
    public int campaignLevel = 0;
    public bool[] doneToolTotorial = { false, false, false, false, false, false };

}

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
    public string[] volumeMixerParameters = { "MasterVolume", "MusicVolume", "EffectsVolume", "VoiceVolume" };

    private static string SettingsFilePath()
    {
        return Path.Combine(Application.dataPath, "StreamingAssets", "klipik.rez");
    }

    public static Settings GetSettings()
    {
        string filePath = SettingsFilePath();
        string directory = Path.GetDirectoryName(filePath);

        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        Settings settings;
        if (!File.Exists(filePath))
        {
            settings = new Settings();
            File.WriteAllText(filePath, JsonUtility.ToJson(settings, true));
            Debug.Log("File created at: " + filePath);
            return settings;
        }

        try
        {
            string json = File.ReadAllText(filePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                settings = new Settings();
                SetSettings(settings);
                return settings;
            }

            settings = JsonUtility.FromJson<Settings>(json);
            if (settings == null)
            {
                settings = new Settings();
                SetSettings(settings);
            }

            return settings;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Failed to read settings file: " + e.Message);
            settings = new Settings();
            SetSettings(settings);
            return settings;
        }
    }

    public static void SetSettings(Settings settings)
    {
        string filePath = SettingsFilePath();
        string directory = Path.GetDirectoryName(filePath);

        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(filePath, JsonUtility.ToJson(settings, true));
    }

    void Start()
    {
        Settings settings = GetSettings();
        fullScreenToggle.isOn = settings.fullScreen;
        Fps.value = settings.fps;
        UpdateSettings(settings, settings.fullScreen ? 1 : 0, settings.fps);

        for (int i = 0; i < VolumeSliders.Length; i++)
        {
            VolumeSliders[i].value = settings.volumes[i] * 100;
        }
        VsyncValue(settings.vsync);//ovo nije potrebno

    }


    public void FullScreenValue(bool value)
    {
        Settings settings = GetSettings();
        if (settings != null)
        {
            settings.fullScreen = value;
            UpdateSettings(settings, value ? 1 : 0, settings.fps);
        }

    }

    public void FpsValue(float value)
    {
        Settings settings = GetSettings();
        if (settings != null)
        {
            settings.fps = (int)value;
            VsyncValue(value <= 0.1f ? true : false);
            UpdateSettings(settings, settings.fullScreen ? 1 : 0, (int)value);
        }
    }

    public void VsyncValue(bool value)
    {
        Settings settings = GetSettings();
        if (settings != null)
        {
            settings.vsync = value;
            QualitySettings.vSyncCount = value ? 1 : 0;
            UpdateSettings(settings);
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
        Settings settings = GetSettings();
        if (settings != null)
        {
            if (index < 0 || index >= settings.volumes.Length)
            {
                Debug.LogWarning("Volume index out of range: " + index);
                return;
            }

            if (index < 0 || index >= volumeMixerParameters.Length)
            {
                Debug.LogWarning("Missing mixer parameter name for volume index: " + index);
                return;
            }

            float clamped = Mathf.Clamp01(value / 100f);
            settings.volumes[index] = clamped;

            if (audioMixer != null)
            {
                float db = clamped <= 0f ? -80f : Mathf.Log10(clamped) * 20f;
                bool mixerSet = audioMixer.SetFloat(volumeMixerParameters[index], db);
                if (!mixerSet)
                {
                    Debug.LogWarning("AudioMixer parameter not found or not exposed: " + volumeMixerParameters[index]);
                }
            }

            UpdateSettings(settings);
        }
    }

    void UpdateSettings(Settings settings, int fullScreen = -1, int hz = -1)
    {
        Application.targetFrameRate = hz != -1 ? (hz) : Application.targetFrameRate;
        Vector2Int resolution = (fullScreen != -1 ? (fullScreen == 1 ? true : false) : Screen.fullScreen) ? new Vector2Int(Screen.currentResolution.width, Screen.currentResolution.height) : new Vector2Int(Screen.width, Screen.height);

        Screen.SetResolution(
            resolution.x,
             resolution.y,
              fullScreen != -1 ? (fullScreen == 1 ? true : false) : Screen.fullScreen,
               hz != -1 ? (hz) : Application.targetFrameRate);

        SetSettings(settings);
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
