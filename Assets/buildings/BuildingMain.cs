using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingMain : MonoBehaviour
{
    public Vector3 UiOffset;
    public GameObject selector;
    public GameObject Ui;
    public GameObject rect;
    public GameObject sword;
    public GameObject allyOptions;
    public GameObject enemyOptions;
    public Button[] otherOptions;
    public TextMeshProUGUI[] optionsCost;
    public int screenEdgeBuffer = 40;
    [System.NonSerialized]
    public Team team;
    [System.NonSerialized]
    public bool selected = false;

    Quaternion rotateTo;

    void Start()
    {
        Ui.SetActive(true);
        selector.SetActive(false);
        allyOptions.SetActive(false);
        enemyOptions.SetActive(false);
        allyOptions.transform.parent.gameObject.SetActive(false);
        team = GetComponent<Team>();
    }

    void Update()
    {
        if (selector.activeInHierarchy)
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
        selector.SetActive(true);
        UiFollow();
        selected = true;
    }

    public void Deselected()
    {
        team.markerRend.material.SetFloat("_SineEnabled", 0);
        selector.SetActive(false);
        SetAllyOptions(false);
        SetEnemyOptions(false);
        selected = false;
    }

    public void SetAllyOptions(bool val)
    {
        selected = val;
        allyOptions.SetActive(val);
        selector.SetActive(val);

        allyOptions.transform.parent.gameObject.SetActive(val);
        SwordResetRotation();

        UiFollow();
    }

    public void SetEnemyOptions(bool val)
    {
        enemyOptions.SetActive(val);
        selector.SetActive(val);

        allyOptions.transform.parent.gameObject.SetActive(val);
        SwordResetRotation();

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


        rect.transform.position = pos;

        if (allyOptions.transform.parent.gameObject.activeSelf)
        {

            sword.transform.rotation *= Quaternion.Lerp(Quaternion.Euler(0, 0, 0), rotateTo * Quaternion.Inverse(sword.transform.rotation), Time.deltaTime * 10);

        }

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


            rect.transform.position = pos;

        } ///za kasnije da popravis ovo gedzeru jedan triguzlavi
        else
        {
            if (Ui.activeSelf)
            {
                Ui.SetActive(false);
            }
        }
    }


    public void SwordUpdateRotation(Transform pos)
    {
        //Quaternion look = Quaternion.LookRotation(pos.position - sword.transform.position, Vector3.forward);
        //look.eulerAngles = new Vector3(0, 0, look.eulerAngles.x + 90);

        Vector3 dir = pos.position - sword.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
        rotateTo = rotation;

    }

    public void SwordResetRotation()
    {
        rotateTo = Quaternion.Euler(0, 0, 0);
    }

}
