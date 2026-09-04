using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newAIBehavior", menuName = "AI/Behaviors/Expand")]

public class AIBehaviorExpand : AIBehavior
{
    public int expandAmount = 90;
    public float EnemyCostMultiplyer = 10;
    public float minUnitDifferenceSendPercent = 0.75f;
    public override bool ExecuteMove(AIManager manager, AIPlayer player)
    {
        if (TryStockShootingBuilding(player)) return true;

        List<BuildingMain> sendFrom = new List<BuildingMain>();
        BuildingMain sendTo = null;
        float units = 0f;

        foreach (BuildingMain sourceBuilding in player.buildings)
        {
            bool hasEnemyNeighbour = false;
            foreach (BuildingMain neighbor in AINeighborUtility.GetNeighbors(sourceBuilding))
            {
                if (neighbor.team.teamid != player.team && neighbor.team.teamid != 0)
                {
                    hasEnemyNeighbour = true;
                    break;
                }
            }
            if (hasEnemyNeighbour) continue;

            sendFrom.Add(sourceBuilding);
            units += GetSendableUnits(player, sourceBuilding) * minUnitDifferenceSendPercent;
        }

        float bestAttackValue = 0;
        foreach (BuildingMain sourceBuilding in player.buildings)
        {
            foreach (BuildingMain neighbor in AINeighborUtility.GetNeighbors(sourceBuilding))
            {
                if (neighbor.team.teamid == player.team) continue;
                float attackValue = units - neighbor.production.product
                    * (neighbor.team.teamid == 0 ? 1 : EnemyCostMultiplyer);
                if (attackValue > bestAttackValue)
                {
                    bestAttackValue = attackValue;
                    sendTo = neighbor;
                }
            }
        }

        if (sendTo != null && sendFrom.Count > 0)
        {
            Debug.Log("AI " + player.team + " expanding to: " + sendTo.id);
            foreach (BuildingMain from in sendFrom)
            {
                from.unitController.Attack(expandAmount, sendTo.transform, false, GetUnitsToKeep(player, from));
                Debug.Log("  -from: " + from.id);
            }

            return true;
        }
        Debug.Log(
    "AI " + player.team +
    " could not find a tower that needs defending or a tower to send from.");
        return false;

    }
}