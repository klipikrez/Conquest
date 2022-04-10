using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newAIBehavior", menuName = "AI/Behaviors/Attack")]
public class AIBehaviorAttack : AIBehavior
{
    public int expandAmount = 90;
    public float minUnitDifferenceSendPercent = 0.75f;
    public override bool ExecuteMove(AIManager manager, AIPlayer player)
    {
        UnitController sendTo = null;
        UnitController sendFrom = null;
        float units = 0f;
        foreach (UnitController tower in player.Towers)
        {
            foreach (UnitController neighbor in tower.neighbours)//pass trough all neighbours of current tower
            {
                float attackValue = tower.production.product * minUnitDifferenceSendPercent - neighbor.production.product;
                if (neighbor.team.teamid != player.team && neighbor.team.teamid == player.currentEnemyTeam && attackValue > units)// if calc better update new target
                {
                    units = attackValue;
                    sendTo = neighbor;
                    sendFrom = tower;
                }
            }
        }

        if (sendTo != null && sendFrom != null)
        {
            sendFrom.Attack(expandAmount, sendTo.transform, false);
            return true;
        }
        return false;

    }

}
