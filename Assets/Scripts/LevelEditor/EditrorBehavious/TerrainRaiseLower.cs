using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TerrainRaiseLower : EditorBehaviour
{

    bool editing = false;
    float time = 1f / 20f; // bice 20 puta u sekundi
    float timer = 0;
    public override void ChangedEditorMode(EditorManager editor)
    {
        editing = false;

    }

    public override void EditorUpdate(EditorManager editor)
    {
        timer += Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) { editor.terrain.drawTreesAndFoliage = false; editing = true; }
        if (Input.GetMouseButtonUp(0)) { editor.terrain.drawTreesAndFoliage = true; editing = false; }

        if (timer > time)
        {
            timer = 0;



            if (Input.GetMouseButton(0) && editing)
            {

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 50000.0f, LayerMask.GetMask("terrain")) && !EventSystem.current.IsPointerOverGameObject())
                {
                    ChangeTerrainHeight(hit.textureCoord * editor.terrain.terrainData.heightmapResolution, editor.terrain, editor);

                }
            }
        }
    }

    void ChangeTerrainHeight(Vector2 pos, Terrain terrain, EditorManager editor)
    {
        float[,] dat = terrain.terrainData.GetHeights(0, 0, terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution);
        //        Debug.Log(editor.BrushImage.name);
        switch (EditorOptions.Instance.BrushImage.name)
        {
            case "1LinearCircle.png":
                {
                    dat = DrawLinnearCircle(pos.x, pos.y, dat, terrain.terrainData.heightmapResolution, EditorOptions.Instance.brushSize, EditorOptions.Instance.brushStrenth);
                    break;
                }
            default:
                {
                    dat = DrawBrush(pos.x, pos.y, dat, terrain.terrainData.heightmapResolution, EditorOptions.Instance.brushSize, EditorOptions.Instance.brushStrenth);
                    break;
                }
        }
        terrain.terrainData.SetHeights(0, 0, dat);
    }

    private float[,] DrawBrush(float x, float y, float[,] pixels, int resolution, float radious, float multiplyer)
    {
        //Debug.Log(/*"x=" + (int)x + "  y=" + (int)y + */"x1=" + (int)(x - (radious / 2)) + "  y1=" + (int)(y - (radious / 2)) + "\nx2=" + (int)(x + (radious / 2)) + "  y=" + (int)(y + (radious / 2)));
        // string a = "";
        for (int i = (int)(x - (radious / 2)); i < (int)(x + (radious / 2)); i++)
        {
            for (int j = (int)(y - (radious / 2)); j < (int)(y + (radious / 2)); j++)
            {
                //a += i + " " + j + ", ";

                if (i >= 0 && j >= 0 && j < resolution && i < resolution) pixels[j, i] += (Input.GetKey(KeyCode.LeftShift) ? -1 : 1) * 0.0001f * multiplyer * (
                    EditorOptions.Instance.BrushImage.GetPixelBilinear((i - (x - (radious / 2))) / radious, (j - (y - (radious / 2))) / radious).r);
            }
        }
        //Debug.Log(a);
        return pixels;
    }

    private float[,] DrawBrushStatic(float x, float y, float[,] pixels, int resolution, float radious, float multiplyer)
    {
        //Debug.Log(/*"x=" + (int)x + "  y=" + (int)y + */"x1=" + (int)(x - (radious / 2)) + "  y1=" + (int)(y - (radious / 2)) + "\nx2=" + (int)(x + (radious / 2)) + "  y=" + (int)(y + (radious / 2)));
        // string a = "";
        for (int i = (int)(x - (radious / 2)); i < (int)(x + (radious / 2)); i++)
        {
            for (int j = (int)(y - (radious / 2)); j < (int)(y + (radious / 2)); j++)
            {
                //a += i + " " + j + ", ";

                if (i >= 0 && j >= 0 && j < resolution && i < resolution) pixels[j, i] += (Input.GetKey(KeyCode.LeftShift) ? -1 : 1) * 0.0001f * multiplyer * (
                    EditorOptions.Instance.BrushImage.GetPixelBilinear((float)i / radious, (float)j / radious).r);
            }
        }
        //Debug.Log(a);
        return pixels;
    }

    private float[,] DrawLinnearCircle(float x, float y, float[,] pixels, int resolution, float radious, float multiplyer = 1)
    {

        for (int i = (int)(x - (radious / 2)); i < (int)(x + (radious / 2)); i++)
        {
            for (int j = (int)(y - (radious / 2)); j < (int)(y + (radious / 2)); j++)
            {
                //a += i + " " + j + ", ";

                if (i >= 0 && j >= 0 && j < resolution && i < resolution)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), new Vector2(i, j));

                    pixels[j, i] += (Input.GetKey(KeyCode.LeftShift) ? -1 : 1) * 0.0001f * multiplyer * Mathf.Max((radious / 2 - distance) / radious, 0);
                }
            }
        }

        /*for (int i = 0; i < resolution; i++)
         {
             for (int j = 0; j < resolution; j++)
             {
                 float distance = Vector2.Distance(new Vector2(x, y), new Vector2(i, j));

                 pixels[j, i] += 0.0001f * multiplyer * Mathf.Max((radious - distance) / radious, 0);
             }
         }*/
        return pixels;
    }
}
