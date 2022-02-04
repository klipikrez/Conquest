using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public GameObject[] GameObjectUIPanels;
    private void Start()
    {
        SetObjectsActive(0);
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
