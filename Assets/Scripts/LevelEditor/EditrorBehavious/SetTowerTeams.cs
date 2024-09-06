using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SetTowerTeams : EditorBehaviour
{
    bool editing = false;
    public override void ChangedEditorMode(EditorManager editor)
    {
        editing = false;


    }

    public override void EditorUpdate(EditorManager editor)
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) { editor.terrain.drawTreesAndFoliage = true; editing = true; }
        if (Input.GetMouseButtonUp(0)) { editor.terrain.drawTreesAndFoliage = true; editing = false; }
        if (Input.GetMouseButton(0) && editing)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 50000.0f, LayerMask.GetMask("terrain")) && !EventSystem.current.IsPointerOverGameObject())
            {
                Debug.DrawRay(hit.point, Vector3.up * 5);
                Collider[] colliders = Physics.OverlapSphere(hit.point, EditorOptions.Instance.brushSize / 10, LayerMask.GetMask("building"));
                foreach (Collider collider in colliders)
                {
                    collider.gameObject.GetComponent<EditorTower>().team = EditorOptions.Instance.team;
                    collider.gameObject.GetComponent<EditorTower>().UpdatColor();
                }
            }

        }
    }

    void OnDrawGizmos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 50000.0f, LayerMask.GetMask("terrain")) && !EventSystem.current.IsPointerOverGameObject())
        {
            // Draw a yellow sphere at the transform's position
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(Vector3.left, EditorOptions.Instance.brushSize);
        }
    }

}
