using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public GameObject[] GameObjectUIPanels;
    public GameObject options;
    public GameObject background;
    public GameObject ScenesManagerSpawn;
    private void Awake()
    {


    }
    private void Start()
    {
        if (ScenesManager.Instance == null)
        {
            Instantiate(ScenesManagerSpawn);
        }
        GameObjectUIPanels[2].SetActive(false); //opcije moraju obavezno da budu ukljucene
        options.SetActive(true);
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            SetObjectsActive(0);
        }
        background.SetActive(true);
    }
    public void MainManu()
    {
        SetObjectsActive(0);
    }
    public void Play()
    {
        SetObjectsActive(1);
    }
    public void Options()
    {
        SetObjectsActive(2);
    }

    public void Levels()
    {
        SetObjectsActive(3);
    }

    public void SetObjectsActive(int i)
    {
        foreach (GameObject obj in GameObjectUIPanels)
        {
            if (obj != null)
                obj.SetActive(false);
        }
        GameObjectUIPanels[i].SetActive(true);
    }


    public void Exit()
    {
        Application.Quit();
    }
}
