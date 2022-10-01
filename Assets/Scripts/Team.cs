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
    Production prod;
    [System.NonSerialized]
    public UnitController controller;
    public float vulnerability = 0.81f;


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

        }






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
            prod.SubtractProduct(vulnerability);
            ;
            if (prod.product < 0)
            {
                controller.StopAttackUnits();
                prod.SetProduct(0);
                if (SoundManager.Instance != null && teamid == 1)
                {
                    SoundManager.Instance.PlayAudioClip(5);
                }
                if (AIManager.Instance != null)
                {
                    AIManager.Instance.UpdateTeamTowers(controller, teamid, attacker.selfTeam);
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
            prod.AddProduct(1f);
        }
    }

}
