using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereChangeTowersTeams : MonoBehaviour
{
    public void inicialize()
    {
        SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();


        sphereCollider.radius = EditorOptions.Instance.brushSize;

    }




}
