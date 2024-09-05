using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BoidUnit/Behavior/CohesionBehavior")]
public class CohesionBehavior : FilterUnitBehavior
{
    public override Vector2 CalculateMove(UnitAgent agent, List<Transform> context, UnitController controller)
    {
        //ako si usamljen kid ne radimo nista
        if(context.Count <= 1)// ovo 1 je transform od track lokacije(to uvek imamo)
        {
            return Vector2.zero;
        }

        //prosek
        Vector2 coehesionMove = Vector2.zero;
        List<Transform> filterContext = (filter == null) ? context : filter.Filter(agent, context);
        for (int i = 1; i < filterContext.Count; i++)//skipuje prvi element ove liste
        {
            coehesionMove += (Vector2)Vec3ToVec2(filterContext[i].position);
        }

        if (filterContext.Count - 1 != 0)
        {
            coehesionMove /= filterContext.Count - 1;
        }


        //offset
        coehesionMove -= (Vector2)Vec3ToVec2(agent.transform.position);
        return coehesionMove;
    }

    Vector3 Vec3ToVec2(Vector3 input)
    {
        return new Vector2(input.x, input.z);
    }
}
