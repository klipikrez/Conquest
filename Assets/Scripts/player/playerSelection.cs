using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class playerSelection : Selection
{
    Team selfTeam;

    [System.NonSerialized]
    public bool Paused = false;

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

        if (Physics.Raycast(ray, out hit, 50000.0f, LayerMask.GetMask("building")) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (selfTeam.teamid != hit.transform.gameObject.GetComponent<Team>().teamid)
            {
                selectedDictionary.Attack(hit.transform, 65);
                selectedDictionary.RemoveAll();
            }
            else
            {
                if (selectedDictionary.selected.Count != 0)
                {
                    selectedDictionary.Attack(hit.transform, 65);
                    selectedDictionary.RemoveAll();
                }
                else
                {
                    selectedDictionary.AddSelected(hit.collider.gameObject);
                }
            }
        }
        else
        {
            selectedDictionary.RemoveAll();
        }

    }

    public override void RayCastRightClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(p1);

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
        return;
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
