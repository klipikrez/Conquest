using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "newAIType", menuName = "AI/Type")]
public class AIType : ScriptableObject
{
    public string TypeName = "Default:)";
    public float clockCycleTime = 1;/*in seconds*/
    public AIState[] States;
    public int expandState = 0;
    public int enemiesNearbyState = 1;
    public int attackEnemyState = 2;
    public float aggresivnes = 20;
    [FormerlySerializedAs("shootingBuildingMinimumUnits")]
    public int shootingBuildingReserve = 100;
    public float shootingBuildingValueMultiplier = 2f;

    public void CalculateMove(AIManager manager, AIPlayer player)
    {
        List<BuildingMain> enemyNeighbors = new List<BuildingMain>();
        foreach (BuildingMain building in player.buildings)
        {

            foreach (BuildingMain neighbor in AINeighborUtility.GetNeighbors(building))
            {
                if (neighbor.team.teamid != player.team && neighbor.team.teamid != 0)
                {
                    enemyNeighbors.Add(neighbor);
                    break;
                }
            }
        }
        if (enemyNeighbors.Count > 0)
        {
            bool currentEnemyIsNeighbor = false;
            foreach (BuildingMain neighbor in enemyNeighbors)
            {
                if (neighbor.team.teamid == player.currentEnemyTeam)
                {
                    currentEnemyIsNeighbor = true;
                    break;
                }
            }
            if (currentEnemyIsNeighbor)
            {
                States[attackEnemyState].CalculateMove(manager, player);
                Debug.Log("Attack enemy");
            }
            else
            {
                States[enemiesNearbyState].CalculateMove(manager, player);
                Debug.Log("Enemy nearby");
            }
        }
        else
        {
            States[expandState].CalculateMove(manager, player);
            Debug.Log("Expand");
        }
    }
}