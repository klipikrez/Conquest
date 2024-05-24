using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Tymski;

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
        SetLoadingGizmos(false);
    }

    public void ReturnToMainMenu()
    {

        Load(mainMenu);
    }

    public void Load(SceneReference sceneRef)
    {
        SetLoadingGizmos(true);
        //
        // 
        //Debug.Log(tekibelike.activeSelf);
        StartCoroutine(LoadAsyncScene(sceneRef));

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
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {

            yield return null;
        }
    }

    IEnumerator LoadAsyncSceneEditor(SceneReference sceneRef, string levelName)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.
        yield return new WaitForEndOfFrame();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneRef);
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {

            yield return null;
        }

        GameObject.Find("Canvas").GetComponent<SaveLoadEditedTerrain>().LoadLevelEditor(levelName);
    }

    IEnumerator LoadAsyncScene(int cseneIndex)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.
        yield return new WaitForEndOfFrame();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(cseneIndex);
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {

            yield return null;
        }
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
