using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class TerrainFolage : EditorBehaviour
{

    bool editing = false;
    float time = 1f / 20f; // bice 20 puta u sekundi
    float timer = 0;

    int[,] outPixels;
    public override void ChangedEditorMode(EditorManager editor)
    {
        editor.ShowBrushVisual(true);
        editing = false;
        if (editor.folage == null)
            editor.folage = new float[editor.terrain.terrainData.detailWidth, editor.terrain.terrainData.detailHeight];
        outPixels = new int[editor.terrain.terrainData.detailWidth, editor.terrain.terrainData.detailHeight];
        RefreshDetailTerrain(editor, editor.folage);
        //folage = editor.terrain.terrainData.GetDetailLayer(0, 0, editor.terrain.terrainData.detailWidth, editor.terrain.terrainData.detailHeight, 0);
    }

    public override void EditorUpdate(EditorManager editor)
    {
        timer += Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) { editor.terrain.drawTreesAndFoliage = true; editing = true; }
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
                    ChangeTerrainDetails(hit.textureCoord * editor.terrain.terrainData.detailWidth, editor.terrain, editor);

                }
            }
        }
    }

    void ChangeTerrainDetails(Vector2 pos, Terrain terrain, EditorManager editor)
    {
        int layNum = 0;

        int[,] dat = DrawBrush(pos.x, pos.y, editor.folage, terrain.terrainData.detailWidth, EditorOptions.Instance.brushSize
        , Input.GetKey(KeyCode.LeftShift) ? EditorOptions.Instance.brushStrenthSlider.maxValue : EditorOptions.Instance.brushStrenth
        , Input.GetKey(KeyCode.LeftShift) ? 0 : EditorOptions.Instance.folageDensity);


        terrain.terrainData.SetDetailLayer(0, 0, layNum, dat);
    }

    private int[,] DrawBrush(float x, float y, float[,] pixels, int resolution, float radious, float multiplyer, float desiredDensity)
    {


        for (int i = (int)(x - (radious / 2)); i < (int)(x + (radious / 2)); i++)
        {
            for (int j = (int)(y - (radious / 2)); j < (int)(y + (radious / 2)); j++)
            {
                //a += i + " " + j + ", ";

                if (i >= 0 && j >= 0 && j < resolution && i < resolution)
                {
                    float brushSample = EditorOptions.Instance.BrushImage.GetPixelBilinear((i - (x - (radious / 2))) / radious, (j - (y - (radious / 2))) / radious).r;
                    float noiseSample = EditorManager.Instance.grassNoiseTexture.GetPixel(j, i).r;

                    float val = (float)Math.Pow(multiplyer / EditorOptions.Instance.brushStrenthSlider.maxValue, 2f) * brushSample * noiseSample;

                    if (pixels[j, i] > desiredDensity && pixels[j, i] + val > desiredDensity)
                        pixels[j, i] -= val;
                    else if (pixels[j, i] > desiredDensity && pixels[j, i] + val < desiredDensity)
                        pixels[j, i] = desiredDensity;

                    if (pixels[j, i] < desiredDensity && pixels[j, i] + val < desiredDensity)
                        pixels[j, i] += val;
                    else if (pixels[j, i] < desiredDensity && pixels[j, i] + val > desiredDensity)
                        pixels[j, i] = desiredDensity;

                    outPixels[j, i] = (
                      (int)(pixels[j, i] * 16));


                }
            }
        }
        return outPixels;
    }

    public void RefreshDetailTerrain(EditorManager editor, float[,] pixels)
    {
        int layNum = 0;
        int resolution = editor.terrain.terrainData.detailWidth;
        if (outPixels == null)
            outPixels = new int[editor.terrain.terrainData.detailWidth, editor.terrain.terrainData.detailHeight];
        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                //float noiseSample = EditorManager.Instance.grassNoiseTexture.GetPixel(j, i).r;

                outPixels[j, i] = (
                  (int)(pixels[j, i] * 16));
            }
        }

        editor.terrain.terrainData.SetDetailLayer(0, 0, layNum, outPixels);
    }

    public override void ExitEditorMode(EditorManager editor)
    {
        editor.ShowBrushVisual(false);
    }
}
