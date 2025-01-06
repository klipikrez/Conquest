using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class playerSelection : Selection
{
    Team selfTeam;

    [System.NonSerialized]
    public bool Paused = false;

    [Tooltip("Duration between 2 clicks in seconds")]
    [Range(0.01f, 0.5f)] public float doubleClickDuration = 0.4f;
    public UnityEvent onDoubleClick;

    public byte clicks = 0;
    public Transform clickedOn = null;
    Coroutine clickCoroutine = null;

    void Start()
    {
        selfTeam = GetComponent<Team>();
        selectedDictionary = GetComponent<playerSelectionDictionary>();
        dragSelect = false;

    }

    void Update()
    {
        if (!Paused)
        {
            Select();

        }


    }


    public override void RayCastLeftClick()
    {

        Ray ray = Camera.main.ScreenPointToRay(p1);
        //Debug.Log("|||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
        if (Physics.Raycast(ray, out hit, 50000.0f, LayerMask.GetMask("building")) && !EventSystem.current.IsPointerOverGameObject())
        {//click on tower 
            //Debug.Log("click on tower ");
            PlayAttackSound();
            if (selfTeam.teamid != hit.transform.gameObject.GetComponent<Team>().teamid)
            {//click on enemy tower
                //Debug.Log("click on enemy tower");
                if (Input.GetKey(KeyCode.LeftShift)) return;
                if (clickCoroutine != null)
                {//double click detected
                    //Debug.Log("double click detected");
                    if (hit.collider.transform == clickedOn)
                    {//double click on selected tower
                        //Debug.Log("double click on selected tower");
                        clicks++;
                    }
                    else
                    {//we clicked once on one tower and then fast on another
                        //Debug.Log("we clicked once on one tower and then fast on another");
                        StopCoroutine(clickCoroutine);
                        clickCoroutine = StartCoroutine(ClickDetection());
                    }
                }
                else
                {//normal click
                    //Debug.Log("normal click");
                    clickCoroutine = StartCoroutine(ClickDetection());
                }

            }
            else
            {//click on friendly tower
                //Debug.Log("click on friendly tower");
                if (selectedDictionary.selected.Count != 0)
                {//selected more than 0
                    if (Input.GetKey(KeyCode.LeftShift))
                        if (selectedDictionary.selected.ContainsKey(hit.collider.gameObject.GetInstanceID())) selectedDictionary.RemoveSelected(hit.collider.gameObject.GetInstanceID());
                        else selectedDictionary.AddSelected(hit.collider.gameObject);
                    else
                    {
                        //Debug.Log("selected more than 0");
                        if (clickCoroutine != null)
                        {//double click detected
                            //Debug.Log("double click detected");
                            if (hit.collider.transform == clickedOn)
                            {//double click on selected tower
                                //Debug.Log("double click on selected tower");
                                if (Input.GetKey(KeyCode.LeftShift)) return;
                                clicks++;
                            }
                            else
                            {//we clicked once on one tower and then fast on another
                                if (selectedDictionary.selected.Count == 1 && selectedDictionary.selected.First().Value.transform == hit.collider.transform)
                                {


                                    //Debug.Log("we clicked on the only selected tower");
                                    StopCoroutine(clickCoroutine);
                                    selectedDictionary.RemoveAll();
                                    clicks = 1;
                                    clickedOn = null;

                                }
                                else
                                {
                                    //Debug.Log("we clicked once on one tower and then fast on another");
                                    StopCoroutine(clickCoroutine);
                                    clickCoroutine = StartCoroutine(ClickDetection());
                                }
                            }
                        }
                        else
                        {//normal click
                            if (selectedDictionary.selected.Count == 1 && selectedDictionary.selected.First().Value.transform == hit.collider.transform)
                            {
                                //Debug.Log("we clicked on the only selected tower");
                                selectedDictionary.RemoveAll();
                                clicks = 1;
                                clickedOn = null;
                            }
                            else
                            {
                                //Debug.Log("normal click");
                                clickCoroutine = StartCoroutine(ClickDetection());
                            }
                        }
                    }
                }
                else
                {//selected 0

                    //Debug.Log("selected 0");
                    selectedDictionary.AddSelected(hit.collider.gameObject);
                    if (clickCoroutine != null)
                        StopCoroutine(clickCoroutine);

                }
            }
        }
        else
        {//if click on ground erase all
         //send immidietly if previously clicked on tower
         //Debug.Log("if click on ground erase all");
            if (clickCoroutine != null && !Input.GetKey(KeyCode.LeftShift))
            {
                //Debug.Log("send immidietly if previously clicked on tower");
                StopCoroutine(clickCoroutine);
                if (clickedOn != null) selectedDictionary.Attack(clickedOn, 62);
                selectedDictionary.RemoveAll();
                clicks = 1;

                clickedOn = null;
            }
        }


    }

    void PlayAttackSound()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySendSound();
        }
    }
    IEnumerator ClickDetection()
    {
        clicks = 1;
        clickedOn = hit.transform;
        yield return new WaitForSeconds(doubleClickDuration);
        // //Debug.Log(clicks + "|||||||||||||||||||||||||||||||||||||||||||||||");
        if (clicks >= 2)
        {
            selectedDictionary.ContinuousAttack(clickedOn);
            selectedDictionary.RemoveAll();
        }
        else if (clicks == 1)
        {
            selectedDictionary.Attack(clickedOn, 62);
            selectedDictionary.RemoveAll();
        }

        clickedOn = null;

    }

    public override void RayCastRightClick()
    {
        /*   Ray ray = Camera.main.ScreenPointToRay(p1);

           if (Physics.Raycast(ray, out hit, 50000.0f, LayerMask.GetMask("building")) && !EventSystem.current.IsPointerOverGameObject())
           {
               BuildingMain hitBuilding = hit.transform.GetComponent<BuildingMain>();

               selectedDictionary.removeOprionsSelected();
               selectedDictionary.addOprionsSelected(hitBuilding.buildingUI);
               if (selfTeam.teamid == hitBuilding.team.teamid)
               {
                   //kada desni klik na naseg lika
                   hitBuilding.buildingUI.SetAllyOptions(true);
                   return;
               }
               else
               {
                   //nemeny
                   hitBuilding.buildingUI.SetEnemyOptions(true);
                   return;
               }
           }
           selectedDictionary.RemoveAll();
           return;*/
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.GetComponent<BuildingUI>() != null)
        {
            if (selfTeam.teamid == other.gameObject.GetComponent<Team>().teamid)
            {
                selectedDictionary.AddSelected(other.gameObject);
            }
        }
    }
}
