using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "newAIState", menuName = "AI/States/Attacked")]
public class AIStateAttacked : AIState
{
    public override void CalculateMove(AIManager manager, AIPlayer player)
    {
        Debug.Log("attacked");
        int action = GetAction();
        chanceBehaviorsTable[action].behavior.ExecuteMove(manager, player);
        /*
        //????
        attackBehavior.ExecuteMove(manager, player);
        defendBehavior.ExecuteMove(manager, player);
        expandBehavior.ExecuteMove(manager, player);*/
    }

}
