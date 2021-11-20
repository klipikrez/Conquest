using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{

    bool isBuilding = false;
    public int teamid = 0;
    public MeshRenderer meshRenderer;
    public MeshRenderer markerRend;
    Production prod;
    UnitController controller;

    void Start()
    {

        if (GetComponent<BuildingMain>() != null)
        {
            isBuilding = true;
            prod = GetComponent<Production>();
            controller = GetComponent<UnitController>();
            StartCoroutine(BuildingStart());
            UpdatColor();
        }






    }

    IEnumerator BuildingStart()
    {
        markerRend.material.SetFloat("_SineEnabled", 1);
        yield return new WaitForSeconds(5);
        markerRend.material.SetFloat("_SineEnabled", 0);
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
