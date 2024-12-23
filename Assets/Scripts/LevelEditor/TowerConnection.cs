using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerConnection : Connection
{
    public EditorTower tower1;
    public EditorTower tower2;




    private void OnDestroy()
    {
        EditorManager.Instance.editorconnections.Remove(this);
    }

    public void SetConnection(EditorTower p1, EditorTower p2)
    {
        tower1 = p1;
        tower2 = p2;
        /*line1.SetPosition(0, p1.transform.position + Vector3.up);
        line1.SetPosition(1, p2.transform.position + Vector3.up);
        line2.SetPosition(0, p2.transform.position + Vector3.up);
        line2.SetPosition(1, p1.transform.position + Vector3.up);*/
        CalculateFollowTerrain(p1.transform.position + Vector3.up, p2.transform.position + Vector3.up);
    }

    public void Drag(EditorTower p1, Vector3 p2)
    {
        /*line1.SetPosition(0, p1.transform.position + Vector3.up);
        line1.SetPosition(1, p2 + Vector3.up);
        line2.SetPosition(0, p2 + Vector3.up);
        line2.SetPosition(1, p1.transform.position + Vector3.up);*/
        CalculateFollowTerrain(p1.transform.position + Vector3.up, p2 + Vector3.up);
    }

    public void UpdatePosition()
    {
        /*line1.SetPosition(0, tower1.transform.position + Vector3.up);
        line1.SetPosition(1, tower2.transform.position + Vector3.up);
        line2.SetPosition(0, tower2.transform.position + Vector3.up);
        line2.SetPosition(1, tower1.transform.position + Vector3.up);*/
        CalculateFollowTerrain(tower1.transform.position + Vector3.up, tower2.transform.position + Vector3.up);
    }



    public void Delete()
    {
        tower1.RemoveConnection(this);
        tower2.RemoveConnection(this);
        Destroy(gameObject);
    }

    public void CycleConnectionType()
    {
        tower1.RemoveConnection(this);
        tower2.RemoveConnection(this);

        if (line1.enabled && line2.enabled)
        {
            Debug.Log("cycle tower connection mode - 1");
            line1.enabled = false; line2.enabled = true;
            //tower1.RemoveConnection(this);
            tower2.AddConnection(tower1);
            return;
        }

        if (!line1.enabled && line2.enabled)
        {
            Debug.Log("cycle tower connection mode - 2");
            line1.enabled = true; line2.enabled = false;
            tower1.AddConnection(tower2);
            //tower2.RemoveConnection(this);
            return;
        }

        Debug.Log("cycle tower connection mode - 3");
        line1.enabled = true; line2.enabled = true;
        tower1.AddConnection(tower2);
        tower2.AddConnection(tower1);
    }

    public void CalculateUppdateConnectionType()
    {

        if (tower1.connections.Contains(this)) { line1.enabled = true; } else { line1.enabled = false; }

        if (tower2.connections.Contains(this)) { line2.enabled = true; } else { line2.enabled = false; }

    }
}
