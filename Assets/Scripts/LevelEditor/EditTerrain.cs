using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EditTerrain : MonoBehaviour
{



    /************************************************************************************
        bool editing = false;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) { terrain.drawTreesAndFoliage = false; editing = false; }
            if (Input.GetMouseButtonUp(0)) { terrain.drawTreesAndFoliage = true; editing = true; }

            if (Input.GetMouseButton(0) && editing)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 50000.0f, LayerMask.GetMask("terrain")) && !EventSystem.current.IsPointerOverGameObject())
                {
                    bbj.transform.position = hit.point;
                    ChangeTerrainHeight(hit.textureCoord * terrain.terrainData.heightmapResolution);

                }
            }
        }

        void ChangeTerrainHeight(Vector2 pos)
        {
            float[,] dat = terrain.terrainData.GetHeights(0, 0, terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution);

            dat = DrawFilledCircle(pos.x, pos.y, dat, terrain.terrainData.heightmapResolution, EditorOptions.Instance.brushSize, EditorOptions.Instance.brushStrenth);
            terrain.terrainData.SetHeights(0, 0, dat);
        }

        private float[,] DrawFilledCircle(float x, float y, float[,] pixels, int resolution, float radious, float multiplyer = 1)
        {
            for (int i = 0; i < resolution; i++)
            {
                for (int j = 0; j < resolution; j++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), new Vector2(i, j));

                    pixels[j, i] += 0.0001f * multiplyer * Mathf.Max((radious - distance) / radious, 0);
                }
            }
            return pixels;
        }*/
}
