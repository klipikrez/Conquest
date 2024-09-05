using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMain : MonoBehaviour
{
    public int id = -52;
    [System.NonSerialized]
    public UnitController unitController;
    [System.NonSerialized]
    public Production production;
    [System.NonSerialized]
    public Team team;
    [System.NonSerialized]
    public BuildingUI buildingUI;
    [System.NonSerialized]
    public UnitDetector unitDetector;
    [System.NonSerialized]
    public BuildingOptions buildingOptions;

    public List<BuildingMain> neighbours;

    public bool autoInitialize = false;
    private void Awake()
    {
        if (autoInitialize) Inicialize();
    }

    public void Inicialize()
    {
        unitController = gameObject.GetComponent<UnitController>();
        if (unitController == null) Debug.LogError("unit Controller Missing on tower: " + id);
        production = gameObject.GetComponent<Production>();
        if (production == null) Debug.LogError(" production Missing on tower: " + id);
        team = gameObject.GetComponent<Team>();
        if (team == null) Debug.LogError("team Missing on tower: " + id);
        buildingUI = gameObject.GetComponent<BuildingUI>();
        if (buildingUI == null) Debug.LogError("buildingUI Missing on tower: " + id);
        unitDetector = gameObject.GetComponent<UnitDetector>();
        if (unitDetector == null) Debug.LogError("unitDetector Missing on tower: " + id);
        buildingOptions = gameObject.GetComponent<BuildingOptions>();
        if (buildingOptions == null) Debug.LogError("buildingOptions Missing on tower: " + id);
        unitController.building = this;
        production.building = this;
        team.building = this;
        buildingUI.building = this;
        unitDetector.building = this;
        buildingOptions.building = this;


        if (autoInitialize)
        {
            CheckNeighbours();
        }
    }

    void CheckNeighbours()
    {
        foreach (BuildingMain neighbour in neighbours)
        {
            if (!neighbour.neighbours.Contains(this))
            {
                neighbour.neighbours.Add(this);
            }
        }
    }
}
