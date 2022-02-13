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
        UnitController sendFrom = null;
        UnitController sendTo = null;
        float units = 0f;
        foreach (UnitController tower in player.Towers)
        {
            foreach (UnitController neighbor in tower.neighbours)//pass trough all neighbours of current tower
            {
                float attackValue = tower.production.product * minUnitDifferenceSendPercent - neighbor.production.product * (neighbor.team.teamid == 0 ? 1 : EnemyCostMultiplyer);
                if (neighbor.team.teamid != player.team && attackValue > units)// if calc better update new target
                {
                    units = attackValue;
                    sendTo = neighbor;
                    sendFrom = tower;
                }
            }
        }

        if (sendTo != null && sendFrom != null)
        {
            sendFrom.Attack(expandAmount, sendTo.transform);
            return true;
        }
        return false;

    }



}
