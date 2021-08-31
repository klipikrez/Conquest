using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingMain : MonoBehaviour
{
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
        UiCanvas.SetActive(true);
        UiFollow();
    }

    public void Deselected()
    {
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

    //public void Selected()
    //{
    //    SetAllyOptions(false);
    //    SetEnemyOptions(false);
    //    UiCanvas.SetActive(true);
    //    UiFollow();
    //}

    //public void Deselected()
    //{
    //    UiCanvas.SetActive(false);
    //}

    //public void SetAllyOptions(bool val)
    //{

    //    if (allyOptions.activeInHierarchy != val)
    //    {          
    //        if (val)
    //        {              
    //            UiCanvas.SetActive(true);
    //        }
    //        allyOptions.SetActive(val);
    //        enemyOptions.SetActive(!val);
    //    }
    //}

    //public void SetEnemyOptions(bool val)
    //{

    //    if (enemyOptions.activeInHierarchy != val)
    //    {           
    //        if (val)
    //        {               
    //            UiCanvas.SetActive(true);
    //        }
    //        enemyOptions.SetActive(val);
    //        allyOptions.SetActive(!val);
    //    }
    //}

    void UiFollow()
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(this.transform.position);

        if(pos.x < screenEdgeBuffer)
        {
            pos.x = screenEdgeBuffer;
        }
        else
        {
            if(pos.x > Screen.width - screenEdgeBuffer)
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
        Vector3 pos = Camera.main.WorldToScreenPoint(this.transform.position);
        Ui.transform.position = pos;
    }

}
