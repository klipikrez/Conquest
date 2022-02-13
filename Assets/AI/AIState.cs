using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIState : ScriptableObject
{
    public AIBehaviorAttack attackBehavior;
    public AIBehaviorDefend defendBehavior;
    public AIBehaviorExpand expandBehavior;
    public abstract void CalculateMove(AIManager manager, AIPlayer player);
}
