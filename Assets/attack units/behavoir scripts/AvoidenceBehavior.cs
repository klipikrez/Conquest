using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BoidUnit/Behavior/AvoidenceBehavior")]
public class AvoidenceBehavior : FilterUnitBehavior
{
    public override Vector2 CalculateMove(UnitAgent agent, List<Transform> context, UnitController controller)
    {
        //ako si usamljen kid ne radimo nista
        if (context.Count <= 1)// ovo 1 je transform od track lokacije(to uvek imamo)
        {
            return Vector2.zero;
        }

        //prosek
        Vector2 avoidenceMove = Vector2.zero;
        int avoid = 0;
        List<Transform> filterContext = (filter == null) ? context : filter.Filter(agent, context);
        for (int i = 1; i< filterContext.Count ; i++)//skipuje prvi element ove liste
        {
            if(Vector2.SqrMagnitude(Vec3ToVec2(agent.transform.position - filterContext[i].position))< controller.SuaredAvoidenceRadious)
            {
                avoid++;

                Vector2 vector2;
                vector2 = (Vector2)(Vec3ToVec2((agent.transform.position - filterContext[i].position)));
                vector2 = vector2.normalized * (vector2.magnitude - controller.AvoidenceRadious);

                avoidenceMove += vector2/*new Vector2(controller.SuaredAvoidenceRadious,controller.SuaredAvoidenceRadious) - (Vector2)(Vec3ToVec2((agent.transform.position - filterContext[i].position)))*/;
            }
            
        }

        if(avoid >0)
        {
            avoidenceMove /= avoid;
        }

        return avoidenceMove;
    }

    Vector2 Vec3ToVec2(Vector3 input)
    {
        return new Vector2(input.x, input.z);
    }
}
