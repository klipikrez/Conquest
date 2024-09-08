using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newAIBehavior", menuName = "AI/Behaviors/Defend")]
public class AIBehaviorDefend : AIBehavior
{
    public int expandAmount = 100;

    public override bool ExecuteMove(AIManager manager, AIPlayer player)
    {
        BuildingMain sendTo = null;// zasad salje samo jednom liku, jkasnije bi voleo da podeli svim tornjevima koji su u opasnosti od napada
        List<BuildingMain> sendFrom = new List<BuildingMain>();
        float bestNumberOfEnemyUnitsNearby = 0;
        foreach (BuildingMain tower in player.buildings)
        {
            float numberOfEnemyUnitsNearby = 0;
            foreach (BuildingMain neighbor in tower.neighbours)//pass trough all neighbours of current tower
            {
                if (neighbor.team.teamid != player.team)// 
                {
                    numberOfEnemyUnitsNearby += neighbor.production.product;

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
            Debug.Log("AI " + player.team + " defending(support units): " + sendTo.id + " from: ");
            foreach (BuildingMain from in sendFrom)
            {
                from.unitController.Attack(expandAmount, sendTo.transform, false);
                Debug.Log("  -from: " + from.id);
            }

            return true;
        }
        Debug.Log("AI " + player.team + " Nowhere to attack");
        return false;

    }

}
