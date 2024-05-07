using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SetPalyerSpawn : EditorBehaviour
{
    bool editing = false;
    GameObject playerPointer;
    public override void ChangedEditorMode(EditorManager editor)
    {
        editing = false;

    }

    public override void EditorUpdate(EditorManager editor)
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) { editor.terrain.drawTreesAndFoliage = true; editing = true; }

        if (editing)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 50000.0f, LayerMask.GetMask("terrain")) && !EventSystem.current.IsPointerOverGameObject())
            {
                if (playerPointer == null)
                {
                    playerPointer = Object.Instantiate(editor.playerSpawnPrefab);
                }
                playerPointer.transform.position = hit.point;


            }

            if (Input.GetMouseButtonUp(0))
            {

                if (hit.point != null && !EventSystem.current.IsPointerOverGameObject())
                    editor.SetPlayerSpawn(hit.point);
                Object.Destroy(playerPointer);

            }
            if (Input.GetMouseButtonUp(0)) { editor.terrain.drawTreesAndFoliage = true; editing = false; EditorOptions.Instance.SetMenuActive(5); editor.ChangeBehaviour(-1); }
        }
    }
}
