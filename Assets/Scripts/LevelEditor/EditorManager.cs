using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class EditorManager : MonoBehaviour
{

    private EditorBehaviour selectedBehaivour;

    public Terrain terrain;
    public Texture2D grassNoiseTexture;
    public static EditorManager Instance;
    public float[,] folage;
    public GameObject playerSpawnPrefab;
    public Vector3 playerSpawn = Vector3.negativeInfinity;
    public EditorSelection editorSelection;
    public GameObject editorTowerPrefab;
    public GameObject editorConnection;
    public towerEditorToggle[] toggles;


    public List<EditorTower> editorTowers = new List<EditorTower>();

    private void Awake()
    {
        Instance = this;
    }


    public void ChangeBehaviour(int val)
    {
        Debug.Log("Editor: " + val);

        if (editorSelection != null)
            editorSelection.enableSelection = false;

        switch (val)
        {
            case 0:
                selectedBehaivour = new TerrainRaiseLower();
                break;
            case 1:
                selectedBehaivour = new TerraiFlatten();
                break;
            case 2:
                selectedBehaivour = new TerrainSmooth();
                break;
            case 3:
                selectedBehaivour = new TerrainPaint();
                break;
            case 4:
                selectedBehaivour = new TerrainTrees();
                break;
            case 5:
                selectedBehaivour = new TerrainFolage();
                break;
            case 6:
                selectedBehaivour = new TowersPlace();
                break;
            case 7:
                selectedBehaivour = new TowersConnect();
                break;
            case 8:
                selectedBehaivour = new SetPalyerSpawn();
                break;
            default:
                selectedBehaivour = null;
                return;
        }
        selectedBehaivour.ChangedEditorMode(this);
    }

    public void SetPlayerSpawn(Vector3 pos)
    {

        playerSpawn = pos;
    }

    public void RefreshDetailTerrain(float[,] data)
    {
        folage = data;
        TerrainFolage foliage = new TerrainFolage();
        foliage.RefreshDetailTerrain(this, folage);
    }

    public void RecalculateObjectsHeight()
    {
        foreach (EditorTower tower in editorTowers)
        {
            RaycastHit hit;
            if (!Physics.Raycast(tower.transform.position + Vector3.up * 520, Vector3.down, out hit, 1040, LayerMask.GetMask("terrain")))
                Debug.Log("erro");
            tower.transform.position = new Vector3(tower.transform.position.x, hit.point.y, tower.transform.position.z);
        }

        GameObject[] connections = GameObject.FindGameObjectsWithTag("Connection");

        foreach (GameObject connection in connections)
        {
            connection.GetComponent<TowerConnection>().UpdatePosition();
        }
    }


    public void HideObjects()
    {
        GameObject[] connections = GameObject.FindGameObjectsWithTag("Connection");
        foreach (GameObject connection in connections)
        {
            connection.transform.GetChild(0).gameObject.SetActive(false);
            connection.transform.GetChild(1).gameObject.SetActive(false);
        }
        foreach (EditorTower tower in editorTowers)
        {
            tower.gameObject.SetActive(false);
        }
    }

    public void ShowObjects()
    {
        GameObject[] connections = GameObject.FindGameObjectsWithTag("Connection");
        foreach (GameObject connection in connections)
        {
            connection.transform.GetChild(0).gameObject.SetActive(true);
            connection.transform.GetChild(1).gameObject.SetActive(true);
        }
        foreach (EditorTower tower in editorTowers)
        {
            tower.gameObject.SetActive(true);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
        {
            EditorOptions.Instance.UpdateBrushSize(++EditorOptions.Instance.brushSize);
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0) // backwards
        {
            EditorOptions.Instance.UpdateBrushSize(--EditorOptions.Instance.brushSize >= 1 ? EditorOptions.Instance.brushSize : ++EditorOptions.Instance.brushSize);
        }

        if (selectedBehaivour != null)
            selectedBehaivour.EditorUpdate(this);

    }

    public void SetTerrainTextures(TerrainLayer[] layers)
    {
        terrain.terrainData.terrainLayers = layers;
    }

    public void ResetAllOverrides()
    {
        foreach (towerEditorToggle toggle in toggles)
        {
            toggle.ResetOverride();
        }
    }

}
