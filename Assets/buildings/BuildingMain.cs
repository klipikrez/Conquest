using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingMain : MonoBehaviour
{
    public Vector3 UiOffset;
    public GameObject UiCanvas;
    public GameObject Ui;
    public GameObject allyOptions;
    public GameObject enemyOptions;

    public int screenEdgeBuffer = 40;
    [System.NonSerialized]
    public Team team;

    void Start()
    {
        UiCanvas.SetActive(true);
        UiCanvas.SetActive(false);
        allyOptions.SetActive(true);
        enemyOptions.SetActive(true);
        allyOptions.SetActive(false);
        enemyOptions.SetActive(false);

        team = GetComponent<Team>();
    }

    void Update()
    {
        if (UiCanvas.activeInHierarchy)
        {
            UiFollow();
        }
        else
        {
            UiIdle();
        }
    }

    public void Selected()
    {
        team.markerRend.material.SetFloat("_SineEnabled", 1);
        UiCanvas.SetActive(true);
        UiFollow();
    }

    public void Deselected()
    {
        team.markerRend.material.SetFloat("_SineEnabled", 0);
        UiCanvas.SetActive(false);
        SetAllyOptions(false);
        SetEnemyOptions(false);
    }

    public void SetAllyOptions(bool val)
    {
        allyOptions.SetActive(val);
        UiFollow();
    }

    public void SetEnemyOptions(bool val)
    {
        enemyOptions.SetActive(val);
        UiFollow();
    }

    void UiFollow()
    {
        if (!Ui.activeSelf)
        {
            Ui.SetActive(true);
        }

        Vector3 pos = Camera.main.WorldToScreenPoint(this.transform.position + UiOffset);

        if (pos.x < screenEdgeBuffer)
        {
            pos.x = screenEdgeBuffer;
        }
        else
        {
            if (pos.x > Screen.width - screenEdgeBuffer)
            {
                pos.x = Screen.width - screenEdgeBuffer;
            }
        }
        if (pos.y < screenEdgeBuffer)
        {
            pos.y = screenEdgeBuffer;
        }
        else
        {
            if (pos.y > Screen.height - screenEdgeBuffer)
            {
                pos.y = Screen.height - screenEdgeBuffer;
            }
        }


        Ui.transform.position = pos;
    }

    void UiIdle()
    {
        if (Vector3.Angle(transform.position - Camera.main.transform.position, Camera.main.transform.forward) > 90.0f) return;

        Vector3 pos = Camera.main.WorldToScreenPoint(this.transform.position + UiOffset);
        //Rect screenRect = new Rect(-Screen.width / 5, -Screen.height / 5, Screen.width + Screen.width / 5, Screen.height + Screen.height / 5);
        Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);
        if (screenRect.Contains(pos))
        {
            // Inside screen vision feild
            if (!Ui.activeSelf)
            {
                Ui.SetActive(true);
            }

            Ui.transform.position = pos;
        } ///za kasnije da popravis ovo gedzeru jedan triguzlavi
        else
        {
            if (Ui.activeSelf)
            {
                Ui.SetActive(false);
            }
        }
    }

}
