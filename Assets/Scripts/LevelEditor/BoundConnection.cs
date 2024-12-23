using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundConnection : Connection
{
    public Transform bound1;
    public Transform bound2;

    public void SetConnection(Transform p1, Transform p2)
    {
        bound1 = p1;
        bound2 = p2;

        UpdatePosition();
    }

    public void UpdatePosition()
    {
        CalculateFollowTerrain(bound1.position + Vector3.up, bound2.position + Vector3.up);
    }



    public void Delete()
    {
        Destroy(gameObject);
    }

}
