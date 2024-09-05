using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Team : MonoBehaviour
{

    bool isBuilding = false;
    public int teamid = 0;
    public MeshRenderer meshRenderer;
    public MeshRenderer markerRend;
    public MeshFilter meshFilter;
    public Animator ObjectAnimatior;

    public float vulnerability = 0.81f;

    [System.NonSerialized]
    public BuildingMain building;

    void Start()
    {

        if (GetComponent<BuildingUI>() != null)//isnt Player
        {
            isBuilding = true;
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

        }






    }


    void UpdatColor()
    {
        //Debug.Log(controller/*.UnitBuildingSpawnBehavior.buildingBehaviors[teamid].color*/);
        //meshRenderer.material.SetColor("Color_", controller.UnitBuildingSpawnBehavior.buildingBehaviors[teamid].color);

        building.production.numberRefrence.color = building.unitController.UnitBuildingSpawnBehavior.buildingBehaviors[teamid].color;//get color from building spawn behaviour

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

    public void SetMesh(Mesh mesh)
    {
        if (meshFilter != null)
        {
            meshFilter.mesh = mesh;
        }
    }

    public void Damage(UnitAgent attacker)
    {

        if (isBuilding)
        {
            building.production.SubtractProduct(vulnerability);
            if (building.production.product < 0)
            {
                building.unitController.StopAttackUnits();
                building.production.SetProduct(1);
                if (SoundManager.Instance != null && teamid == 1)
                {
                    SoundManager.Instance.PlayAudioClip(5);
                }
                if (AIManager.Instance != null)
                {
                    AIManager.Instance.UpdateTeamTowers(building, teamid, attacker.selfTeam);
                }
                ChangeTeam(attacker.selfTeam);
                if (WinConditions.Instance != null)
                {
                    WinConditions.Instance.CheckTeams();
                }
            }
        }
    }

    public void ChangeTeam(int team)
    {
        teamid = team;
        UpdatColor();
    }

    public void Reinforce()
    {
        if (isBuilding)
        {
            building.production.AddProduct(1f);
        }
    }

}
