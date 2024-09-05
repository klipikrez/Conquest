using System;

using UnityEngine;
using UnityEngine.EventSystems;

public class TerrainTrees : EditorBehaviour
{

    bool editing = false;
    float time = 1f / 20f; // bice 20 puta u sekundi
    float timer = 0;
    KdTree treeInstances = new KdTree(true);
    public override void ChangedEditorMode(EditorManager editor)
    {
        editing = false;
        treeInstances.Clear();
        foreach (TreeInstance tree in editor.terrain.terrainData.treeInstances)
        {
            TreeInstance tre = tree;
            tre.position = new Vector3(tree.position.x, 0.5f, tree.position.z);
            treeInstances.Add(tre);
        }
    }

    public override void EditorUpdate(EditorManager editor)
    {
        timer += Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) { editor.terrain.drawTreesAndFoliage = true; editing = true; }
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

                    ChangeTerrainTrees(hit.textureCoord, editor.terrain, editor);

                }
            }

        }
    }

    void ChangeTerrainTrees(Vector2 pos, Terrain terrain, EditorManager editor)
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (treeInstances.Count != 0)
            {

                Vector3 point = new Vector3(pos.x, 0.5f, pos.y);
                treeInstances.RemoveWithinRadius(point, (float)EditorOptions.Instance.brushSize / ((terrain.terrainData.size.x * terrain.terrainData.size.x)));

            }/*while (true)
                if (treeInstances.Count != 0)
                {
                    Vector3 point = new Vector3(pos.x, 0.5f, pos.y);
                    KdNode tree = treeInstances.RemoveWithinRadious(point);
                    point.y = tree.component.position.y;
                    //Debug.Log(tree.component.position * 1000f + ":" + point * 1000f + "  -=-  " + Vector3.Distance(tree.component.position, point) * 1000f + " -- " + ((float)EditorOptions.Instance.brushSize / terrain.terrainData.size.x) * 1000f);
                    if (Math.Abs(tree.component.position.x - point.x) + Math.Abs(tree.component.position.z - point.z) < (float)EditorOptions.Instance.brushSize / terrain.terrainData.size.x)
                        treeInstances.Remove(tree);
                    else
                    {
                        terrain.terrainData.SetTreeInstances(treeInstances.ToList().ToArray(), true);
                        return;
                    }

                }
                else

                {
                    terrain.terrainData.SetTreeInstances(treeInstances.ToList().ToArray(), true);
                    return;
                }*/

        }
        else
        {
            Vector3 treePosition = new Vector3(pos.x, 0.5f, pos.y);
            TreeInstance tree = new TreeInstance();

            if (treeInstances.Count <= 0)
            {

                tree.prototypeIndex = EditorOptions.Instance.selectedTree;
                tree.heightScale = UnityEngine.Random.Range(EditorOptions.Instance.minScale, EditorOptions.Instance.maxScale);
                tree.widthScale = tree.heightScale;


                //  treePosition.y = terrain.terrainData.GetHeight((int)treePosition.x, (int)treePosition.z) / terrain.terrainData.size.y;
                tree.position = treePosition;
                treeInstances.Add(tree);
            }

            for (int t = 0; t < EditorOptions.Instance.brushStrenth; t++)
            {
                Vector2 randomCircle = UnityEngine.Random.insideUnitCircle;
                treePosition = new Vector3(pos.x + randomCircle.x * EditorOptions.Instance.brushSize / terrain.terrainData.alphamapResolution,
                0.5f,
                pos.y + randomCircle.y * EditorOptions.Instance.brushSize / editor.terrain.terrainData.alphamapResolution);
                Vector3 cloasestTree = treeInstances.FindClosest(treePosition).position;
                cloasestTree -= treePosition;
                if (Math.Abs(cloasestTree.x) + Math.Abs(cloasestTree.z) > EditorOptions.Instance.treeSpacing / terrain.terrainData.size.x)
                {
                    tree = new TreeInstance();
                    tree.prototypeIndex = EditorOptions.Instance.selectedTree;
                    tree.heightScale = UnityEngine.Random.Range(EditorOptions.Instance.minScale, EditorOptions.Instance.maxScale);
                    tree.widthScale = tree.heightScale;


                    //treePosition.y = terrain.terrainData.GetHeight((int)treePosition.x, (int)treePosition.z) / terrain.terrainData.size.y;
                    tree.position = treePosition;
                    treeInstances.Add(tree);
                }
            }
        }

        terrain.terrainData.SetTreeInstances(treeInstances.ToList().ToArray(), true);


    }



    void LoadTerrainTrees(string levelName, TerrainData terrain)
    {






        /*      ovo je za dodavanje prefaba na teren
                List<TreePrototype> workTreePrototypes = new List<TreePrototype>();
                for (int tp = 0; tp < trees.workTreePrototypes.Length; tp++)
                {
                    workTreePrototypes.Add(trees.workTreePrototypes[tp].Copy());
                }
                terrain.treePrototypes = workTreePrototypes.ToArray();
        */


    }



}