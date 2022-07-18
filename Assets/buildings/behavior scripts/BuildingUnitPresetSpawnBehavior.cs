using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Building/Behavior/UnitPresetSpawnBehavior")]
public class BuildingUnitPresetSpawnBehavior : BuildingBehavior
{
    public Color color = Color.white;
    public float[] scale;
    public Mesh[] mesh;
    public Mesh[] defaultBuildingMesh;
    public Mesh[] defenseBuildingMesh;
    public Mesh[] productionBuildingMesh;
    public Material[] matrial;

    public override void InitializeUnit(UnitAgent newAgent, Transform[] path, UnitController unitController, int team, bool isGift, float updateTime)
    {
        newAgent.name = team.ToString() + (isGift ? "g" : "n") + " - " + newAgent.id;
        newAgent.AddPath(path);
        newAgent.isGift = isGift;
        newAgent.updateTime = updateTime;
        newAgent.selfTeam = team;

        int rand = Random.Range(0, mesh.Length);

        //if item at index is null then take item at index 0
        float scaleS = scale.Length > rand ? scale[rand] : scale[0];
        Material matS = matrial.Length > rand ? matrial[rand] : matrial[0];
        newAgent.SetUnitApperence(mesh[rand], matS, new Vector3(scaleS, scaleS, scaleS));
        newAgent.SetColorVariant(team);

        newAgent.currentMoveDirection = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
        newAgent.Initialize(unitController);
    }

}
