using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuildingBehavior : ScriptableObject
{
    public abstract void InitializeUnit(UnitAgent newAgent, Transform[] path, UnitController unitController, int team, bool isGift, float updateTime);

}
