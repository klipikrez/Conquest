using UnityEngine;
using System.Collections;
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

    public void CalculateMove(AIManager manager, AIPlayer player)
    {
        List<BuildingMain> enemyAsNeighbour = new List<BuildingMain>();
        foreach (BuildingMain building in player.buildings)
        {

            foreach (BuildingMain neighbour in building.neighbours)
            {
                if (neighbour.team.teamid == player.team || neighbour.team.teamid == 0)
                {

                }
                else
                {
                    enemyAsNeighbour.Add(neighbour);
                    break;
                }
            }
        }
        if (enemyAsNeighbour.Count > 0)
        {
            bool enemyIsNeighbour = false;
            foreach (BuildingMain neighbour in enemyAsNeighbour)
            {
                if (neighbour.team.teamid == player.currentEnemyTeam)
                {
                    enemyIsNeighbour = true;
                    break;
                }
            }
            if (enemyIsNeighbour)
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
        /*
        //Debug.Log(player.team);
        States[0].CalculateMove(manager, player);
        //Debug.Log("uspesno");*/
    }
}