using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BoidUnit/Behavior/TrackBehavior")]

public class TrackBehavior : UnitBehaivour
{
    public float maxInfluence = 0.1f;
    public override Vector2 CalculateMove(UnitAgent agent, List<Transform> context, UnitController controller)
    {
        //ako si usamljen kid ne radimo nista
        if (context.Count == 0)
        {
            return Vector2.zero;
        }

        //prosek
        Vector2 trackMove = Vec3ToVec2(context[0].position);

        //offset
        trackMove -= (Vector2)Vec3ToVec2(agent.transform.position);

        //normalized
        trackMove = trackMove.normalized * maxInfluence;


        return trackMove;
    }

    Vector3 Vec3ToVec2(Vector3 input)
    {
        return new Vector2(input.x, input.z);
    }
}
