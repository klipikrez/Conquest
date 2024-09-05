using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Building/Behavior/CompilerBehavior")]
public class BuildingBehaviorCompiler : BuildingBehavior
{
    public BuildingUnitPresetSpawnBehavior[] buildingBehaviors;

    public override void InitializeUnit(UnitAgent newAgent, Transform[] path, UnitController unitController, int team, bool isGift, float updateTime)
    {
        if (buildingBehaviors.Length > team)
        {
            buildingBehaviors[team].InitializeUnit(newAgent, path, unitController, team, isGift, updateTime);
        }
    }
}
