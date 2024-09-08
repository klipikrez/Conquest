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
        BuildingMain sendTo = null;
        BuildingMain sendFrom = null;
        float units = 0f;
        foreach (BuildingMain tower in player.buildings)
        {
            foreach (BuildingMain neighbor in tower.neighbours)//pass trough all neighbours of current tower
            {
                float attackValue = tower.production.product * minUnitDifferenceSendPercent - neighbor.production.product;//how valiable is this tower vs. how dificult it's to capture
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
            sendFrom.unitController.Attack(expandAmount, sendTo.transform, false);
            Debug.Log("AI " + player.team + " attack: " + sendTo.id + " from: " + sendFrom.id);
            return true;
        }
        Debug.Log("AI " + player.team + " Nowhere to attack");
        return false;

    }

}
