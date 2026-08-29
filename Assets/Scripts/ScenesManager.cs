using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Tymski;
using System.Diagnostics;

public class ScenesManager : MonoBehaviour
{
    public GameObject tekibelike;
    public LoadingGizmo gozmo;
    public static ScenesManager Instance { get; private set; }
    public SceneReference mainMenu;

    private void Awake()
    {
        SetLoadingGizmos(false);

        Instance = this;

        DontDestroyOnLoad(gameObject);


    }
    private void Start()
    {

        SceneManager.activeSceneChanged += ChangedActiveScene;
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

    public void LoadLevel(SceneReference sceneRef, string levelName)
    {
        SetLoadingGizmos(true);
        //
        // 
        //Debug.Log(tekibelike.activeSelf);
        StartCoroutine(LoadAsyncSceneLevel(sceneRef, levelName));

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

    IEnumerator LoadAsyncSceneLevel(SceneReference sceneRef, string levelName)
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

            yield return SaveLoadLevel.GetInstance().LoadLevelAsync(levelName);

        }

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

}