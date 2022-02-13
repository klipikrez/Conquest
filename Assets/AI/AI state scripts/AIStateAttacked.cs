using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateAttacked : AIState
{
    public override void CalculateMove(AIManager manager, AIPlayer player)
    {
        //????
        attackBehavior.ExecuteMove(manager, player);
        defendBehavior.ExecuteMove(manager, player);
        expandBehavior.ExecuteMove(manager, player);
    }

}
