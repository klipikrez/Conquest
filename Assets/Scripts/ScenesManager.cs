using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Tymski;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class ScenesManager : MonoBehaviour
{
    public GameObject tekibelike;
    public LoadingGizmo gozmo;
    public static ScenesManager Instance { get; private set; }
    public SceneReference mainMenu;
    public int levelNumber = -52;
    public string[] campaignLevels;
    private SceneReference levelScene = null;

    private void Awake()
    {
        SetLoadingGizmos(false);

        Instance = this;

        DontDestroyOnLoad(gameObject);


    }
    private void Start()
    {

        SceneManager.activeSceneChanged += ChangedActiveScene;
        LoadCampaignNames();
    }

    private void ChangedActiveScene(Scene current, Scene next)
    {
        //SetLoadingGizmos(false);
    }

    public void ReturnToMainMenu()
    {

        //if (Application.isEditor)
        //{
        Load(mainMenu);
        //}
        //else
        //{
        //    RestartApplication();//we do this to stop buffer overflow 
        //                         //unity stupid
        //}
    }

    public void RestartApplication()
    {
        // Path to the built executable (adjust extension as needed)
        string exePath = Application.dataPath.Replace("_Data", ".exe");
        Process.Start(exePath);    // Launches a new process
        Application.Quit();        // Exits the current process
    }

    public void Load(SceneReference sceneRef)
    {
        SetLoadingGizmos(true);
        //
        // 
        //Debug.Log(tekibelike.activeSelf);
        StartCoroutine(LoadAsyncScene(sceneRef));

    }

    public void LoadLevel(SceneReference sceneRef, string levelName, int levelNumber = -52)
    {
        if (levelNumber >= 0) this.levelNumber = levelNumber;
        levelScene = sceneRef;
        SetLoadingGizmos(true);
        //
        // 
        //Debug.Log(tekibelike.activeSelf);
        StartCoroutine(LoadAsyncSceneLevel(sceneRef, levelName, levelNumber));

    }
    public void SetCampaignLevels(string[] names)
    {
        campaignLevels = names;
    }
    public void LoadNextLevel()
    {

        if (levelNumber < 0) { UnityEngine.Debug.LogError("Campaign number error, given: " + (levelNumber + 1)); return; }
        if (levelNumber > campaignLevels.Length - 1) Load(0); // return to menu if campaign is done :)


        SetLoadingGizmos(true);

        StartCoroutine(LoadAsyncSceneLevel(levelScene, campaignLevels[levelNumber], levelNumber));
    }

    public void UpdateCurrentCampaignProgress()
    {
        levelNumber++;
        Settings settings = JsonUtility.FromJson<Settings>(File.ReadAllText(Application.dataPath + "/StreamingAssets/klipik.rez"));
        if (levelNumber < settings.campaignLevel) return;
        settings.campaignLevel = levelNumber;
        File.WriteAllText(Application.dataPath + "/StreamingAssets/klipik.rez", JsonUtility.ToJson(settings));//update setings json

    }

    public void LoadEditor(SceneReference sceneRef, string levelName)
    {
        SetLoadingGizmos(true);
        //
        // 
        //Debug.Log(tekibelike.activeSelf);
        StartCoroutine(LoadAsyncSceneEditor(sceneRef, levelName));

    }

    public void Load(int cseneIndex)
    {
        SetLoadingGizmos(true);
        //
        // 
        //Debug.Log(tekibelike.activeSelf);
        StartCoroutine(LoadAsyncScene(cseneIndex));

    }

    IEnumerator LoadAsyncScene(SceneReference sceneRef)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        yield return new WaitForEndOfFrame();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneRef);

        // Don't let Unity activate the scene immediately.
        asyncLoad.allowSceneActivation = false;

        // Wait until the asynchronous scene fully loads
        while (asyncLoad.progress < 0.9f)
        {

            yield return null;
        }

        // Scene is now loaded and ready to be activated.
        asyncLoad.allowSceneActivation = true;

        // Wait until Unity has finished activating the scene.
        while (!asyncLoad.isDone)
        {

            yield return null;
        }

        SetLoadingGizmos(false);

    }

    IEnumerator LoadAsyncSceneLevel(SceneReference sceneRef, string levelName, int levelNumber = -52)
    {
        UnityEngine.Debug.Log("LoadedLevelNumber: " + levelNumber);
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        yield return new WaitForEndOfFrame();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneRef);

        // Don't let Unity activate the scene immediately.
        asyncLoad.allowSceneActivation = false;

        // Wait until the asynchronous scene fully loads
        while (asyncLoad.progress < 0.9f)
        {

            yield return null;
        }

        // Scene is now loaded and ready to be activated.
        asyncLoad.allowSceneActivation = true;

        // Wait until Unity has finished activating the scene.
        while (!asyncLoad.isDone)
        {

            yield return null;
        }

        // Give Unity one frame after activating the scene.
        yield return null;



        if (SaveLoadLevel.GetInstance() != null)
        {

            yield return SaveLoadLevel.GetInstance().LoadLevelAsync(levelName);

        }
        this.levelNumber = levelNumber;
        SetLoadingGizmos(false);
    }

    IEnumerator LoadAsyncSceneEditor(SceneReference sceneRef, string levelName)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        yield return new WaitForEndOfFrame();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneRef);

        // Don't let Unity activate the scene immediately.
        asyncLoad.allowSceneActivation = false;

        // Wait until the asynchronous scene fully loads
        while (asyncLoad.progress < 0.9f)
        {

            yield return null;
        }

        // Scene is now loaded and ready to be activated.
        asyncLoad.allowSceneActivation = true;

        // Wait until Unity has finished activating the scene.
        while (!asyncLoad.isDone)
        {

            yield return null;
        }

        // Give Unity one frame after activating the scene.
        yield return null;


        if (SaveLoadLevel.GetInstance() != null)
        {

            yield return SaveLoadLevel.GetInstance().LoadLevelEditorAsync(levelName);

        }

        SetLoadingGizmos(false);
    }

    IEnumerator LoadAsyncScene(int cseneIndex)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        yield return new WaitForEndOfFrame();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(cseneIndex);

        // Don't let Unity activate the scene immediately.
        asyncLoad.allowSceneActivation = false;

        // Wait until the asynchronous scene fully loads
        while (asyncLoad.progress < 0.9f)
        {

            yield return null;
        }

        // Scene is now loaded and ready to be activated.
        asyncLoad.allowSceneActivation = true;

        // Wait until Unity has finished activating the scene.
        while (!asyncLoad.isDone)
        {

            yield return null;
        }

        SetLoadingGizmos(false);

    }

    void SetLoadingGizmos(bool val)
    {
        tekibelike.SetActive(val);

        if (val)
        {
            gozmo.StartInvokeRepeating();
        }
        else
        {
            gozmo.CancleInvokeRepeating();
        }
    }


    private void LoadCampaignNames()
    {
        List<string> levelNames = new List<string>();
        CheckLevelFolder();
        string folderPath = Application.dataPath + "/StreamingAssets/Levels";

        string[] dir = Directory.GetDirectories(folderPath);
        foreach (string dirName in dir)
        {
            if (!Regex.IsMatch(Path.GetFileName(dirName), @"^[0-9]+-[0-9]+"))
            {
                //Debug.Log("Skipping non-official level: " + Path.GetFileName(dirName));
                continue;
            }
            levelNames.Add(dirName.Substring(dirName.LastIndexOf('\\') + 1));

        }

        ScenesManager.Instance.SetCampaignLevels(levelNames.ToArray());
    }

    void CheckLevelFolder()
    {
        if (!System.IO.Directory.Exists(Application.dataPath + "/StreamingAssets/Levels"))
        {
            System.IO.Directory.CreateDirectory(Application.dataPath + "/StreamingAssets/Levels");


        }
    }

}