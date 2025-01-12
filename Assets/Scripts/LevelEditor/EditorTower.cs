
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using TMPro;
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
    [SerializedDictionary("NAME", "VALAUEEUE")]
    public SerializedDictionary<string, float> towerOverrides = new SerializedDictionary<string, float>();
    public SerializedDictionary<string, TextMeshProUGUI> tmpGUI = new SerializedDictionary<string, TextMeshProUGUI>();
    public MeshFilter meshFilter;
    public string meshName;
    public bool selected = false;
    public List<TowerConnection> connections = new List<TowerConnection>();
    public GameObject connectionPrefab;
    float screenEdgeBuffer = 13;
    public GameObject Ui;
    public GameObject rect;
    public GameObject infoGUI;
    Vector3 UiOffset = Vector3.up;
    public static Dictionary<int, EditorTower> TowerIDs = new Dictionary<int, EditorTower>();
    public int selfID;
    public int team;
    public MeshRenderer meshRenderer;
    public void SetId(int givenId = -1)
    {
        Debug.Log(givenId);
        if (givenId == -1)
            for (int i = 0; true; i++)
            {
                if (!TowerIDs.ContainsKey(i)) { TowerIDs.Add(i, this); selfID = i; break; }

            }
        else
        {
            if (!TowerIDs.ContainsKey(givenId))
            {
                TowerIDs.Add(givenId, this);
                selfID = givenId;
            }
        }
    }

    public void RemoveID()
    {
        if (TowerIDs.ContainsKey(selfID)) { TowerIDs.Remove(selfID); }
    }

    public void DeleteTower()
    {
        while (connections.Count > 0)
        {
            connections[0].Delete();
        }
        RemoveID();
        Object.Destroy(gameObject);
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
        infoGUI.transform.position = pos + Vector3.up * 50;

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
        UpdateGUI();

    }

    public void ClearOverride(string name)
    {

        if (towerOverrides.ContainsKey(name))
        {
            towerOverrides.Remove(name);
        }

        UpdateGUI();
    }

    public void AddOverride(string name, float value)
    {
        if (towerOverrides.ContainsKey(name))
        {
            towerOverrides[name] = value;
        }
        else
        {
            towerOverrides.Add(name, value);
        }

        UpdateGUI();
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

    public void UpdateGUI()
    {
        tmpGUI["ID"].text = "ID:" + selfID.ToString();
        tmpGUI["Starting units"].text = preset.product.ToString();
        tmpGUI["Max units"].text = preset.maxUnits.ToString();
        tmpGUI["Unit production"].text = preset.productProduction.ToString();
        tmpGUI["Vulnerability"].text = preset.vulnerability.ToString();
        foreach (var ovr in towerOverrides)
        {
            switch (ovr.Key)
            {
                case "ID":
                    {
                        tmpGUI["ID"].text = ovr.Value.ToString();
                        return;
                    }
                case "Starting units":
                    {
                        tmpGUI["Starting units"].text = ovr.Value.ToString();
                        return;
                    }
                case "Max units":
                    {
                        tmpGUI["Max units"].text = ovr.Value.ToString();
                        return;
                    }
                case "Unit production":
                    {
                        tmpGUI["Unit production"].text = ovr.Value.ToString();
                        return;
                    }
                case "Vulnerability":
                    {
                        tmpGUI["Vulnerability"].text = ovr.Value.ToString();
                        return;
                    }
            }
        }
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

    public void UpdateTeam(int team)
    {
        this.team = team;
        UpdatColor();
    }

    public void UpdatColor()
    {
        //Debug.Log(controller/*.UnitBuildingSpawnBehavior.buildingBehaviors[teamid].color*/);
        //meshRenderer.material.SetColor("Color_", controller.UnitBuildingSpawnBehavior.buildingBehaviors[teamid].color);



        if (meshRenderer != null)
        {
            float materalTeamTextureOffset = 1f / meshRenderer.material.mainTexture.height;
            meshRenderer.material.mainTextureOffset = new Vector2(0, -materalTeamTextureOffset * team);
        }

    }

}
