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
    private void Awake()
    {
        Instance = this;
    }



    public void ChangeBehaviour(int val)
    {
        Debug.Log("Editor: " + val);
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
            default:
                selectedBehaivour = null;
                return;
        }
        selectedBehaivour.ChangedEditorMode(this);
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



}
