using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum overrideType { team, startAmount, production, maxUnits, vulnerability }

public class TowerOverride
{
    public overrideType type;
    int intValue;
    float floatValue;
}

public class TowerPresetData
{
    public string imagePath;
    public float product = 0;//nes nis dobit//
    public float cost = 10;//nes nis dobit//
    public float productProduction = 0.4f;//increment by 0.4?//
    public int maxUnits = 10;//increment by 10?//
    public float vulnerability = 0.81f; //
    public int[] enabledUpgrades; //
    public string meshPath; // not implemented yet

}



public class EditorTower : MonoBehaviour
{
    public BuildingPreset preset;
    public List<TowerOverride> towerOverrides = new List<TowerOverride>();
    public MeshFilter meshFilter;
    public GameObject selector;
    public bool selected = false;
    public List<TowerConnection> connections = new List<TowerConnection>();
    public GameObject connectionPrefab;

    private void Start()
    {

    }

    public void ClearOverrides()
    {
        towerOverrides.Clear();
    }

    public void Selected()
    {
        selector.SetActive(true);
        selected = true;
    }

    public void Deselected()
    {
        selector.SetActive(false);
        selected = false;
    }

    public void AddConnection(EditorTower connection)
    {
        if (connection == this) return;
        foreach (TowerConnection conn in connections)
        {
            if ((conn.tower2 == connection && conn.tower1 == this) || (conn.tower1 == connection && conn.tower2 == this)) return;
        }
        TowerConnection towerConnection = Instantiate(connectionPrefab).GetComponent<TowerConnection>();
        towerConnection.SetConnection(this, connection);
        connection.connections.Add(towerConnection);
        connections.Add(towerConnection);
    }

    public void RemoveConnection(TowerConnection conn)
    {
        if (connections.Contains(conn))
        {
            connections.Remove(conn);
        }
    }

    public void MoveTo(Vector3 pos)
    {
        transform.position = pos;
        RaycastHit hit;
        if (!Physics.Raycast(transform.position + Vector3.up * 520, Vector3.down, out hit, 1040, LayerMask.GetMask("terrain")))
            Debug.Log("erro");
        transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);

        foreach (TowerConnection conn in connections)
            conn.UpdatePosition();
    }

}
