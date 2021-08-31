using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BoidUnit/Filter/SameFlockFilter")]
public class SameFlockFilter : ContextFilter
{
    public override List<Transform> Filter(UnitAgent agent, List<Transform> original)
    {
        List<Transform> filtered = new List<Transform>();
        for (int i = 1;i<original.Count;i++)
        {
            UnitAgent itemAgent = original[i].GetComponent<UnitAgent>();
            if(itemAgent != null && itemAgent.Controller == agent.Controller)
            {
                filtered.Add(original[i]);
            }
        }
        return filtered;
    }
}
