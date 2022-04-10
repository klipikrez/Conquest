using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIState : ScriptableObject
{
    public int[] rngStateChances = new int[3] { 5, 35, 60 };//treba ukupno da bude 100 
    public AIBehaviorAttack attackBehavior;
    public AIBehaviorDefend defendBehavior;
    public AIBehaviorExpand expandBehavior;
    public abstract void CalculateMove(AIManager manager, AIPlayer player);
}
