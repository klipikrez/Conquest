using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

public class EditorManager : MonoBehaviour
{

    private EditorBehaviour selectedBehaivour;

    public Terrain terrain;
    public Texture2D grassNoiseTexture;
    public static EditorManager Instance;
    public float[,] folage = new float[512, 512];
    public GameObject playerSpawnPrefab;
    public GameObject boundPrefab;
    public Vector3 playerSpawn = Vector3.negativeInfinity;
    public EditorSelection editorSelection;
    public GameObject editorTowerPrefab;
    public GameObject editorConnection;
    public GameObject boundsConnectionObject;
    public towerEditorToggle[] toggles;
    public TowerButton towerPresets;
    public GameObject tutorialCards;
    public bool drawBrushGraphic = false;
    public DynamicMeshGenerator dynamicMeshGenerator;
    public List<EditorTower> editorTowers = new List<EditorTower>();

    public List<TowerConnection> editorconnections = new List<TowerConnection>();
    public List<Vector2> bounds = new List<Vector2>();



    private void Awake()
    {
        Instance = this;
    }


    private void Start()
    {
        tutorialCards.SetActive(false);
        Settings settings = JsonUtility.FromJson<Settings>(File.ReadAllText(Application.dataPath + "/StreamingAssets/klipik.rez"));
        if (settings.showEditorTutorial)
        {
            tutorialCards.SetActive(true);
        }
    }

    public void ChangeBehaviour(int val)
    {


        if (editorSelection != null)
            editorSelection.enableSelection = false;

        if (selectedBehaivour != null) selectedBehaivour.ExitEditorMode(this);

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
            case 9:
                selectedBehaivour = new SetBounds();
                break;
            default:
                Debug.Log("Editor ERROR: " + val);
                selectedBehaivour = null;
                return;
        }
        Debug.Log("Editor: " + val + " -- " + selectedBehaivour);
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
            { Debug.Log("erro"); return; }
            tower.transform.position = new Vector3(tower.transform.position.x, hit.point.y, tower.transform.position.z);
        }

        GameObject[] connections = GameObject.FindGameObjectsWithTag("ConnectionTower");

        foreach (GameObject connection in connections)
        {
            connection.GetComponent<TowerConnection>().UpdatePosition();
        }
    }


    public void HideObjects()
    {
        GameObject[] connections = GameObject.FindGameObjectsWithTag("ConnectionTower");
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
        GameObject[] connections = GameObject.FindGameObjectsWithTag("ConnectionTower");
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

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
        {
            EditorOptions.Instance.UpdateBrushSize(++EditorOptions.Instance.brushSize);
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0) // backwards
        {
            EditorOptions.Instance.UpdateBrushSize(--EditorOptions.Instance.brushSize >= 1 ? EditorOptions.Instance.brushSize : ++EditorOptions.Instance.brushSize);
        }*/

        if (selectedBehaivour != null)
            selectedBehaivour.EditorUpdate(this);


        if (drawBrushGraphic)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 50000.0f, LayerMask.GetMask("terrain")) && !EventSystem.current.IsPointerOverGameObject())
            {
                EditorOptions.Instance.decalProjectorBrush.gameObject.SetActive(true);
                EditorOptions.Instance.decalProjectorBrush.gameObject.transform.position = hit.point + Vector3.up * 520;
            }
            else
            {
                EditorOptions.Instance.decalProjectorBrush.gameObject.SetActive(false);
            }
        }

    }

    public void ShowBrushVisual(bool val, Texture2D tex = null)
    {
        if (tex != null)
        {
            EditorOptions.Instance.SetBrushImageNoUpdate(tex);
        }
        drawBrushGraphic = val;
        EditorOptions.Instance.decalProjectorBrush.gameObject.SetActive(val);
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
