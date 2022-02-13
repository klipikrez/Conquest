using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIBehavior : ScriptableObject
{
    public abstract bool ExecuteMove(AIManager manager, AIPlayer player);
}
