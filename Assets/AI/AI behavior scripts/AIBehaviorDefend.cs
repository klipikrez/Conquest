using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newAIBehavior", menuName = "AI/Behaviors/Defend")]
public class AIBehaviorDefend : AIBehavior
{
    public int expandAmount = 100;

    public override bool ExecuteMove(AIManager manager, AIPlayer player)
    {
        UnitController sendTo = null;// zasad salje samo jednom liku, jkasnije bi voleo da podeli svim tornjevima koji su u opasnosti od napada
        List<UnitController> sendFrom = new List<UnitController>();
        float bestNumberOfEnemyUnitsNearby = 0;
        foreach (UnitController tower in player.Towers)
        {
            float numberOfEnemyUnitsNearby = 0;
            foreach (UnitController neighbor in tower.neighbours)//pass trough all neighbours of current tower
            {
                if (neighbor.team.teamid != player.team)// 
                {
                    numberOfEnemyUnitsNearby += neighbor.team.controller.production.product;

                }
            }

            if (numberOfEnemyUnitsNearby <= 0)
            {
                sendFrom.Add(tower);
            }
            else
            {
                if (numberOfEnemyUnitsNearby > bestNumberOfEnemyUnitsNearby)
                {
                    sendTo = tower;
                }
            }
        }

        if (sendTo != null && sendFrom != null)
        {
            foreach (UnitController from in sendFrom)
            {
                from.Attack(expandAmount, sendTo.transform, false);
            }
            return true;
        }
        return false;

    }

}
