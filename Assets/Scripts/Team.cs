using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{

    public bool isBuilding = false;
    public int teamid = 0;
    public Color[] colors = new Color[5];

    public GameObject colorIdentefier;
    MeshRenderer meshRenderer;
    Production prod;
    UnitController controller;

    void Start()
    {
        if (colorIdentefier != null)
        {
            meshRenderer = colorIdentefier.GetComponent<MeshRenderer>();
            UpdateColor();
        }

        if(GetComponent<BuildingMain>() != null)
        {
            isBuilding = true;
            prod = GetComponent<Production>();
            controller = GetComponent<UnitController>();
        }

    }

    void UpdateColor()
    {
        meshRenderer.material.SetColor("Color_", colors[teamid]);
    }

    public void Damage(UnitAgent att)
    {

        prod.product -= 0.81f;
        if (prod.product < 0)
        {
            controller.StopAttackUnits();
            prod.product = 0;
            teamid = att.selfTeam;
            UpdateColor();            
        }

    }

    public void Reinforce()
    {
        prod.product += 1f;
    }

}
