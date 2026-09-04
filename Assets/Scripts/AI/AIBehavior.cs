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
        return IsShootingBuilding(building) ? GetShootingBuildingReserve(player) : 1f;
    }

    protected float GetShootingBuildingReserve(AIPlayer player)
    {
        return Mathf.Max(0f, player.aiType.shootingBuildingReserve);
    }

    protected float GetSendableUnits(AIPlayer player, BuildingMain building)
    {
        return Mathf.Max(0f, building.production.product - GetUnitsToKeep(player, building));
    }

    protected float GetDefensiveValue(AIPlayer player, BuildingMain building, float enemyUnitsNearby)
    {
        if (!IsShootingBuilding(building)) return enemyUnitsNearby;
        return enemyUnitsNearby + GetShootingBuildingReserve(player) * player.aiType.shootingBuildingValueMultiplier;
    }

    protected bool TryStockShootingBuilding(AIPlayer player)
    {
        BuildingMain sendTo = null;
        BuildingMain sendFrom = null;
        float largestDeficit = 0f;

        foreach (BuildingMain building in player.buildings)
        {
            if (!IsShootingBuilding(building)) continue;

            float deficit = GetShootingBuildingReserve(player) - building.production.product;
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

public static class AINeighborUtility
{
    public static List<BuildingMain> GetNeighbors(BuildingMain building)
    {
        List<BuildingMain> neighbors = new List<BuildingMain>();
        if (building == null || building.neighbours == null)
        {
            return neighbors;
        }

        HashSet<BuildingMain> visited = new HashSet<BuildingMain> { building };
        Stack<BuildingMain> pending = new Stack<BuildingMain>(building.neighbours);

        while (pending.Count > 0)
        {
            BuildingMain neighbor = pending.Pop();
            if (neighbor == null || !visited.Add(neighbor))
            {
                continue;
            }

            if (!CanAIInteractWith(neighbor))
            {
                if (neighbor.neighbours == null) continue;

                foreach (BuildingMain linkedNeighbor in neighbor.neighbours)
                {
                    pending.Push(linkedNeighbor);
                }

                continue;
            }

            neighbors.Add(neighbor);
        }

        return neighbors;
    }

    private static bool CanAIInteractWith(BuildingMain building)
    {
        return building.unitDetector == null || building.unitDetector.Engage;
    }
}