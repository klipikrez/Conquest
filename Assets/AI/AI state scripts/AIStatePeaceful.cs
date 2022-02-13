using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newAIState", menuName = "AI/States/Peaceful")]
public class AIStatePeaceful : AIState
{

    public override void CalculateMove(AIManager manager, AIPlayer player)
    {
        //za sd samo jedna sttvar
        //Be00GraD
        //Be0-Grad
        attackBehavior.ExecuteMove(manager, player);
        expandBehavior.ExecuteMove(manager, player);
        defendBehavior.ExecuteMove(manager, player);

    }

}
