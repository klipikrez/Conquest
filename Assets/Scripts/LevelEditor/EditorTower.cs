using System.Collections;
using System.Collections.Generic;

using UnityEngine;


[System.Serializable]
public class TowerPresetData
{
    public float product = 0;//nes nis dobit//
    public float cost = 10;//nes nis dobit//
    public float productProduction = 0.4f;//increment by 0.4?//
    public int maxUnits = 10;//increment by 10?//
    public float vulnerability = 0.81f; //
    public string[] enabledUpgrades; //
    public string[] meshPath; // meshes


}



public class EditorTower : MonoBehaviour
{
    // public BuildingPreset preset; old
    public string presetName;
    public TowerPresetData preset;
    public Dictionary<string, object> towerOverrides = new Dictionary<string, object>();
    public MeshFilter meshFilter;
    public string meshName;
    public bool selected = false;
    public List<TowerConnection> connections = new List<TowerConnection>();
    public GameObject connectionPrefab;
    float screenEdgeBuffer = 13;
    public GameObject Ui;
    public GameObject rect;
    Vector3 UiOffset = Vector3.up;
    public static Dictionary<int, EditorTower> TowerIDs = new Dictionary<int, EditorTower>();
    public int selfID;
    public void SetId(int givenId = -1)
    {
        if (givenId == -1)
            for (int i = 0; true; i++)
            {
                if (!TowerIDs.ContainsKey(i)) { TowerIDs.Add(i, this); selfID = i; break; }

            }
        else { if (!TowerIDs.ContainsKey(givenId)) { TowerIDs.Add(givenId, this); selfID = givenId; } }
    }

    public void RemoveID()
    {
        if (TowerIDs.ContainsKey(selfID)) { TowerIDs.Remove(selfID); }
    }

    private void Update()
    {
        if (selected) UiFollow();
    }

    void UiFollow()
    {


        Vector3 pos = Camera.main.WorldToScreenPoint(this.transform.position + UiOffset);

        if (pos.x < screenEdgeBuffer)
        {
            pos.x = screenEdgeBuffer;
        }
        else
        {
            if (pos.x > Screen.width - screenEdgeBuffer)
            {
                pos.x = Screen.width - screenEdgeBuffer;
            }
        }
        if (pos.y < screenEdgeBuffer)
        {
            pos.y = screenEdgeBuffer;
        }
        else
        {
            if (pos.y > Screen.height - screenEdgeBuffer)
            {
                pos.y = Screen.height - screenEdgeBuffer;
            }
        }


        rect.transform.position = pos;


    }


    public void SetPreset(TowerPresetData preset, string presetName, meshAndName mesh = null)
    {
        this.preset = preset;
        this.presetName = presetName;
        mesh = mesh == null ? EditorManager.Instance.towerPresets.GetRandomMesh() : mesh;
        meshFilter.sharedMesh = mesh.mesh;
        meshName = mesh.name;
        ClearOverrides();
    }

    public void ClearOverrides()
    {
        towerOverrides.Clear();
    }

    public void ClearOverride(string name)
    {
        if (towerOverrides.ContainsKey(name))
        {
            towerOverrides.Remove(name);
        }
    }

    public void AddOverride(string name, object value)
    {
        if (towerOverrides.ContainsKey(name))
        {
            towerOverrides[name] = value;
        }
        else
        {
            towerOverrides.Add(name, value);
        }
    }
    public void Selected()
    {

        Ui.SetActive(true);
        selected = true;
    }

    public void Deselected()
    {
        Ui.SetActive(false);
        selected = false;
    }

    public void AddConnection(EditorTower connection, int direction = 0)
    {
        //check if connection is self or if it already exists in local connections :)
        if (connection == this) return;
        foreach (TowerConnection conn in connections)
        {
            if ((conn.tower2 == connection && conn.tower1 == this) || (conn.tower1 == connection && conn.tower2 == this)) return;
        }

        //check if connection already exists in global connections
        TowerConnection ExistigTowerConnection = null;
        foreach (TowerConnection conn in EditorManager.Instance.editorconnections)
        {
            if ((conn.tower2 == connection && conn.tower1 == this) || (conn.tower1 == connection && conn.tower2 == this)) { ExistigTowerConnection = conn; break; }
        }

        if (ExistigTowerConnection == null)
        {
            ExistigTowerConnection = Instantiate(connectionPrefab).GetComponent<TowerConnection>();
            ExistigTowerConnection.SetConnection(this, connection);
            EditorManager.Instance.editorconnections.Add(ExistigTowerConnection);
        }

        switch (direction)
        {
            case 0:
                {
                    connection.connections.Add(ExistigTowerConnection);
                    connections.Add(ExistigTowerConnection);

                    break;
                }

            case 1:
                {
                    connections.Add(ExistigTowerConnection);
                    break;
                }

            case 2:
                {
                    connection.connections.Add(ExistigTowerConnection);
                    break;
                }
        }
        if (direction != 0)
            ExistigTowerConnection.CalculateUppdateConnectionType();


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
