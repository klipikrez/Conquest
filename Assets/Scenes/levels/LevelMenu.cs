using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMenu : MonoBehaviour
{
    public GameObject UI;
    public MainMenuUI Main;
    List<playerMovement> movementScripts = new List<playerMovement>();
    List<playerSelection> selectionScripts = new List<playerSelection>();

    public GameObject WinUI;
    public GameObject LoseUI;

    public bool paused = false;
    public static float timeSinceStart = 0f;

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
        if (!paused)
        {
            timeSinceStart += Time.deltaTime;
        }
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
        paused = true;
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
        paused = false;
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

    public void WinScreen()
    {
        Pause();
        Main.SetObjectsActive(2);
        WinUI.SetActive(true);
        LoseUI.SetActive(false);
    }

    public void LoseScreen()
    {
        Pause();
        Main.SetObjectsActive(2);
        WinUI.SetActive(false);
        LoseUI.SetActive(true);
    }

    public void Retry()
    {
        ScenesManager.Instance.Load(SceneManager.GetActiveScene().buildIndex);
    }

    public void Continue()
    {
        Resume();
    }
}
