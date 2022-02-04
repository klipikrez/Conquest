using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMenu : MonoBehaviour
{
    public GameObject UI;
    public MainMenuUI Main;
    List<playerMovement> movementScripts = new List<playerMovement>();
    List<playerSelection> selectionScripts = new List<playerSelection>();


    private void Start()
    {
        Resume();

        GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject obj in objs)
        {
            movementScripts.Add(obj.GetComponent<playerMovement>());
        }
        foreach (GameObject obj in objs)
        {
            selectionScripts.Add(obj.GetComponent<playerSelection>());
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (UI.activeSelf)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        UI.SetActive(true);
        //Time.timeScale = 0.0f;
        NavManager.Instance.SetPauseBuildings(true);
        Main.SetObjectsActive(0);
        foreach (playerMovement pm in movementScripts)
        {
            pm.Paused = true;
        }
        foreach (playerSelection pm in selectionScripts)
        {
            pm.Paused = true;
        }
    }
    public void Resume()
    {
        //Time.timeScale = 1.0f;
        NavManager.Instance.SetPauseBuildings(false);
        UI.SetActive(false);
        foreach (playerMovement pm in movementScripts)
        {
            pm.Paused = false;
        }
        foreach (playerSelection pm in selectionScripts)
        {
            pm.Paused = false;
        }
    }

    public void ReturnToMainMenu()
    {
        //Time.timeScale = 1.0f;
        ScenesManager.Instance.ReturnToMainMenu();
    }
}
