using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{

    bool isBuilding = false;
    public int teamid = 0;
    public MeshRenderer meshRenderer;
    public MeshRenderer markerRend;
    public Animator ObjectAnimatior;
    Production prod;
    [System.NonSerialized]
    public UnitController controller;

    void Start()
    {

        if (GetComponent<BuildingMain>() != null)//isnt Player
        {
            isBuilding = true;
            prod = GetComponent<Production>();
            controller = GetComponent<UnitController>();
            //StartCoroutine(BuildingStart());
            UpdatColor();

            if (ObjectAnimatior == null && meshRenderer != null)
            {
                ObjectAnimatior = meshRenderer.gameObject.GetComponent<Animator>();
            }

            if (ObjectAnimatior != null && teamid == 1)
            {
                ObjectAnimatior.Play("Base Layer.playerStart");
            }
            WinConditions.Instance.AddBuildingTeam(this);
        }






    }

    IEnumerator BuildingStart()
    {
        yield return new WaitForSeconds(5);
        /*
        markerRend.material.SetFloat("_SineEnabled", 1);
        yield return new WaitForSeconds(5);
        markerRend.material.SetFloat("_SineEnabled", 0);*/
    }

    void UpdatColor()
    {
        //Debug.Log(controller/*.UnitBuildingSpawnBehavior.buildingBehaviors[teamid].color*/);
        //meshRenderer.material.SetColor("Color_", controller.UnitBuildingSpawnBehavior.buildingBehaviors[teamid].color);

        prod.numberRefrence.color = controller.UnitBuildingSpawnBehavior.buildingBehaviors[teamid].color;//get color from building spawn behaviour

        if (meshRenderer != null)
        {
            float materalTeamTextureOffset = 1f / meshRenderer.material.mainTexture.height;
            meshRenderer.material.mainTextureOffset = new Vector2(0, -materalTeamTextureOffset * teamid);
        }
        if (markerRend != null)
        {
            float materalTeamTextureOffset = 1f / markerRend.material.mainTexture.height;
            markerRend.material.mainTextureOffset = new Vector2(0, -materalTeamTextureOffset * teamid);
        }
        if (teamid != 0)
        {
            prod.productionDullMultiplyer = 1;
        }
        else
        {
            prod.productionDullMultiplyer = 0.2f;
        }
    }

    public void Damage(UnitAgent att)
    {

        if (isBuilding)
        {
            prod.product -= 0.81f;
            if (prod.product < 0)
            {
                controller.StopAttackUnits();
                prod.product = 0;
                teamid = att.selfTeam;
                UpdatColor();
                WinConditions.Instance.CheckTeams();

            }
        }
    }

    public void Reinforce()
    {
        if (isBuilding)
        {
            prod.product += 1f;
        }
    }

}
