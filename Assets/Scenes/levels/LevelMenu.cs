using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMenu : MonoBehaviour
{
    public GameObject UI;
    public MainMenuUI Main;
    List<playerMovement> movementScripts = new List<playerMovement>();
    FlyCamera editorMovement;
    List<Selection> selectionScripts = new List<Selection>();

    public GameObject WinUI;
    public GameObject LoseUI;

    public bool paused = false;
    public static float timeSinceStart = 0f;

    private void Start()
    {


        GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject obj in objs)
        {
            playerMovement mv = obj.GetComponent<playerMovement>();
            if (mv != null)
                movementScripts.Add(mv);
        }
        if (movementScripts.Count == 0) editorMovement = objs[0].GetComponent<FlyCamera>();
        foreach (GameObject obj in objs)
        {
            selectionScripts.Add(obj.GetComponent<Selection>());
        }
        Resume();
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
        if (NavManager.Instance != null) NavManager.Instance.SetPauseBuildings(true);
        Main.SetObjectsActive(0);
        if (movementScripts.Count != 0)
            foreach (playerMovement pm in movementScripts)
            {
                pm.Paused = true;
            }
        else
        {
            editorMovement.Paused = true;
        }
        foreach (playerSelection pm in selectionScripts)
        {
            pm.Paused = true;
        }
    }
    public void Resume()
    {
        paused = false;
        Main.SetObjectsActive(0);
        //Time.timeScale = 1.0f;
        if (NavManager.Instance != null) NavManager.Instance.SetPauseBuildings(false);
        UI.SetActive(false);
        if (movementScripts.Count != 0)
            foreach (playerMovement pm in movementScripts)
            {
                pm.Paused = false;
            }
        else
        {
            editorMovement.Paused = false;
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
        Main.SetObjectsActive(1);
        WinUI.SetActive(true);
        LoseUI.SetActive(false);
    }

    public void LoseScreen()
    {
        Pause();
        Main.SetObjectsActive(1);
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
