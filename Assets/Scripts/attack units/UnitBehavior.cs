using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitBehaivour : ScriptableObject
{

    public abstract Vector2 CalculateMove(UnitAgent agent, List<Transform> context, UnitController controller);



}
