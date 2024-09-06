using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TerrainPaint : EditorBehaviour
{
    bool editing = false;
    float time = 1f / 20f; // bice 20 puta u sekundi
    float timer = 0;
    bool startedPaint = true;
    Vector2 previousPointerPos = new Vector2(0, 0);
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




            if (Input.GetMouseButton(0) && editing)
            {
                timer = 0;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 50000.0f, LayerMask.GetMask("terrain")) && !EventSystem.current.IsPointerOverGameObject())
                {
                    ChangeTerrainSplat(hit.textureCoord * editor.terrain.terrainData.alphamapResolution, editor.terrain, editor);

                }
            }

        }
    }

    void ChangeTerrainSplat(Vector2 pos, Terrain terrain, EditorManager editor)
    {
        float[,,] numArray = terrain.terrainData.GetAlphamaps(0, 0, terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight);
        //Debug.Log(numArray.GetLength(0) + " - " + numArray.GetLength(1) + " - " + numArray.GetLength(2));
        /* switch (editor.BrushImage.name)
         {
             case "1LinearCircle.png":
                 {
                     numArray = DrawLinnearCircle(pos.x, pos.y, numArray, terrain.terrainData.heightmapResolution, EditorOptions.Instance.brushSize, EditorOptions.Instance.brushStrenth);
                     break;
                 }
             default:
                 {
        numArray = DrawBrush(pos.x, pos.y, numArray, terrain.terrainData.heightmapResolution, EditorOptions.Instance.brushSize, EditorOptions.Instance.brushStrenth, editor);
        break;
    }
}*/
        if (!Input.GetMouseButtonDown(0))
        {
            //Debug.Log(previousPointerPos + "  --  " + pos);
            float distance = Vector2.Distance(pos, previousPointerPos);
            int repeat = (int)(distance / (EditorOptions.Instance.brushSize / 2) - 0.5f);
            for (int i = 1; i < repeat; i++)
            {
                Vector2 temp = Vector2.Lerp(previousPointerPos, pos, (float)i / (float)repeat);
                numArray = DrawBrush(temp.x, temp.y, numArray, terrain.terrainData.alphamapResolution, EditorOptions.Instance.brushSize, EditorOptions.Instance.brushStrenth, EditorOptions.Instance.selectedTexture);
            }
            //numArray = DrawBrush(pos.x, pos.y, numArray, terrain.terrainData.heightmapResolution, EditorOptions.Instance.brushSize, EditorOptions.Instance.brushStrenth, editor);
            numArray = DrawBrush(previousPointerPos.x, previousPointerPos.y, numArray, terrain.terrainData.alphamapResolution, EditorOptions.Instance.brushSize, EditorOptions.Instance.brushStrenth, EditorOptions.Instance.selectedTexture);
        }
        else
        {
            //            Debug.Log("aaaa");

            numArray = DrawBrush(pos.x, pos.y, numArray, terrain.terrainData.alphamapResolution, EditorOptions.Instance.brushSize, EditorOptions.Instance.brushStrenth, EditorOptions.Instance.selectedTexture);
        }
        previousPointerPos = pos;
        terrain.terrainData.SetAlphamaps(0, 0, numArray);
    }

    private float[,,] DrawBrush(float x, float y, float[,,] pixels, int resolution, float radious, float multiplyer, int textureIndex)
    {
        //Debug.Log(/*"x=" + (int)x + "  y=" + (int)y + */"x1=" + (int)(x - (radious / 2)) + "  y1=" + (int)(y - (radious / 2)) + "\nx2=" + (int)(x + (radious / 2)) + "  y=" + (int)(y + (radious / 2)));
        // string a = "";

        for (int textureLayer = 0; textureLayer < EditorManager.Instance.terrain.terrainData.terrainLayers.Length; textureLayer++)
        {
            for (int i = (int)(x - (radious / 2)); i < (int)(x + (radious / 2)); i++)
            {
                for (int j = (int)(y - (radious / 2)); j < (int)(y + (radious / 2)); j++)
                {
                    //a += i + " " + j + ", ";

                    if (i >= 0 && j >= 0 && j < resolution - 1 && i < resolution - 1) pixels[j, i, textureLayer] += (textureIndex == textureLayer ? 1 : -1) * multiplyer / EditorOptions.Instance.brushStrenthSlider.maxValue * (
                        EditorOptions.Instance.BrushImage.GetPixelBilinear((i - (x - (radious / 2))) / radious, (j - (y - (radious / 2))) / radious).r);
                }
            }
        }
        //Debug.Log(a);
        return pixels;
    }

    private float[,,] DrawBrushStatic(float x, float y, float[,,] pixels, int resolution, float radious, float multiplyer)
    {
        //Debug.Log(/*"x=" + (int)x + "  y=" + (int)y + */"x1=" + (int)(x - (radious / 2)) + "  y1=" + (int)(y - (radious / 2)) + "\nx2=" + (int)(x + (radious / 2)) + "  y=" + (int)(y + (radious / 2)));
        // string a = "";
        for (int i = (int)(x - (radious / 2)); i < (int)(x + (radious / 2)); i++)
        {
            for (int j = (int)(y - (radious / 2)); j < (int)(y + (radious / 2)); j++)
            {
                //a += i + " " + j + ", ";

                if (i >= 0 && j >= 0 && j < resolution && i < resolution) pixels[j, i, 0] += (Input.GetKey(KeyCode.LeftShift) ? -1 : 1) * 0.0001f * multiplyer * (
                    EditorOptions.Instance.BrushImage.GetPixelBilinear((float)i / radious, (float)j / radious).r);
            }
        }
        //Debug.Log(a);
        return pixels;
    }

    private float[,,] DrawLinnearCircle(float x, float y, float[,,] pixels, int resolution, float radious, float multiplyer = 1)
    {

        for (int i = (int)(x - (radious / 2)); i < (int)(x + (radious / 2)); i++)
        {
            for (int j = (int)(y - (radious / 2)); j < (int)(y + (radious / 2)); j++)
            {
                if (i >= 0 && j >= 0 && j < resolution && i < resolution)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), new Vector2(i, j));

                    pixels[j, i, 0] += (Input.GetKey(KeyCode.LeftShift) ? -1 : 1) * 0.0001f * multiplyer * Mathf.Max((radious / 2 - distance) / radious, 0);
                }
            }
        }
        return pixels;
    }


}