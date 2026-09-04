using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIBehavior : ScriptableObject
{
    public abstract bool ExecuteMove(AIManager manager, AIPlayer player);

    protected bool IsShootingBuilding(BuildingMain building)
    {
        return building != null && building.buildingShoot != null && building.buildingShoot.enabled;
    }

    protected float GetUnitsToKeep(AIPlayer player, BuildingMain building)
    {
        return IsShootingBuilding(building) ? player.aiType.shootingBuildingMinimumUnits : 1f;
    }

    protected float GetSendableUnits(AIPlayer player, BuildingMain building)
    {
        return Mathf.Max(0f, building.production.product - GetUnitsToKeep(player, building));
    }

    protected float GetDefensiveValue(AIPlayer player, BuildingMain building, float enemyUnitsNearby)
    {
        if (!IsShootingBuilding(building)) return enemyUnitsNearby;
        return enemyUnitsNearby + player.aiType.shootingBuildingMinimumUnits * player.aiType.shootingBuildingValueMultiplier;
    }

    protected bool TryStockShootingBuilding(AIPlayer player)
    {
        BuildingMain sendTo = null;
        BuildingMain sendFrom = null;
        float largestDeficit = 0f;

        foreach (BuildingMain building in player.buildings)
        {
            if (!IsShootingBuilding(building)) continue;

            float deficit = player.aiType.shootingBuildingMinimumUnits - building.production.product;
            if (deficit > largestDeficit)
            {
                largestDeficit = deficit;
                sendTo = building;
            }
        }

        if (sendTo == null) return false;

        float mostSendableUnits = 0f;
        foreach (BuildingMain building in player.buildings)
        {
            if (building == sendTo) continue;

            float sendableUnits = GetSendableUnits(player, building);
            if (sendableUnits > mostSendableUnits)
            {
                mostSendableUnits = sendableUnits;
                sendFrom = building;
            }
        }

        if (sendFrom == null) return false;

        int sendPercent = Mathf.Clamp(
            Mathf.CeilToInt(largestDeficit / sendFrom.production.product * 100f), 1, 100);
        sendFrom.unitController.Attack(
            sendPercent, sendTo.transform, false, GetUnitsToKeep(player, sendFrom));
        Debug.Log("AI " + player.team + " stocking shooting building: " + sendTo.id + " from: " + sendFrom.id);
        return true;
    }
}