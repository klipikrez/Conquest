using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BoidUnit/Behavior/CompositeBehavior")]
public class CompositeBehavior : UnitBehaivour
{

    public UnitBehaivour[] behaivours;
    public float[] weights;

    public override Vector2 CalculateMove(UnitAgent agent, List<Transform> context, UnitController controller)
    {
        if (weights.Length != behaivours.Length)
        {
            Debug.LogError("neradi neradi neradi" + name, this);
            return Vector2.zero;
        }

        //setup
        Vector2 move = Vector2.zero;

        //prodji kroz sve brauz
        for (int i = 0; i < behaivours.Length; i++)
        {

            Vector2 partalMove = behaivours[i].CalculateMove(agent, context, controller) * weights[i];

            if (partalMove != Vector2.zero)
            {
                if (partalMove.sqrMagnitude > weights[i] * weights[i])
                {
                    partalMove.Normalize();
                    partalMove *= weights[i];
                }

                move += partalMove;

            }

        }

        return move;
    }
}
