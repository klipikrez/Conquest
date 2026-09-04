using System.Collections;
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

        foreach (BuildingMain tower in player.buildings)//da vidimo dal' ima neprijatelja ko komsiju, ako ima ignorisemo ga
        {
            bool hasEnemyNeighbour = false;
            foreach (BuildingMain neighbor in tower.neighbours)
            {
                if (neighbor.team.teamid != player.team && neighbor.team.teamid != 0)
                {
                    hasEnemyNeighbour = true;
                    break;
                }
            }
            if (hasEnemyNeighbour) continue;

            sendFrom.Add(tower);
            units += GetSendableUnits(player, tower) * minUnitDifferenceSendPercent;
        }

        float bestAttackValue = 0;
        foreach (BuildingMain tower in player.buildings)
        {
            foreach (BuildingMain neighbor in tower.neighbours)//pass trough all neighbours of current tower
            {
                if (neighbor.team.teamid == player.team) continue;
                float attackValue = units - neighbor.production.product * (neighbor.team.teamid == 0 ? 1 : EnemyCostMultiplyer);
                if (attackValue > bestAttackValue)// if calc better update new target
                {
                    bestAttackValue = attackValue;
                    sendTo = neighbor;
                }
            }
        }

        if (sendTo != null && sendFrom.Count > 0)
        {
            Debug.Log("AI " + player.team + " defending(support units): " + sendTo.id + " from: ");
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