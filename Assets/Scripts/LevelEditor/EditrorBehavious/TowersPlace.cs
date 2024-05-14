using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEditor.Scripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;




public class TowersPlace : EditorBehaviour
{

    bool startedPlaceBuilding = false;
    bool clickedOnSelectedBuilding = false;
    public Vector3 previousPosition;
    float moveDistance = 0;
    EditorTower startingConnection;
    TowerConnection editorConnection;
    public override void ChangedEditorMode(EditorManager editor)
    {
        editor.editorSelection.enableSelection = true;
        startedPlaceBuilding = false;
    }

    public override void EditorUpdate(EditorManager editor)
    {
        //start connect tow towers from this one
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 50000.0f, LayerMask.GetMask("building")) && !EventSystem.current.IsPointerOverGameObject())
            {
                startingConnection = hit.collider.gameObject.GetComponent<EditorTower>();
                if (editorConnection != null)
                    editorConnection.gameObject.SetActive(true);
            }
        }
        if (Input.GetMouseButtonUp(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            //delete/edit connection
            if (startingConnection == null && Physics.Raycast(ray, out hit, 50000.0f, LayerMask.GetMask("connection")) && !EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("aaa");
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    TowerConnection conn = hit.collider.transform.parent.gameObject.GetComponent<TowerConnection>();
                    if (conn != null)
                    {
                        conn.Delete();
                    }
                }
                else
                {
                    hit.collider.transform.parent.gameObject.GetComponent<TowerConnection>().CycleConnectionType();
                }
            }
            //finish connecting towers
            if (startingConnection != null)
            {
                if (Physics.Raycast(ray, out hit, 50000.0f, LayerMask.GetMask("building")) && !EventSystem.current.IsPointerOverGameObject())
                {
                    EditorTower connectTower = hit.collider.gameObject.GetComponent<EditorTower>();
                    startingConnection.AddConnection(connectTower);

                }
                startingConnection = null;
                editorConnection.gameObject.SetActive(false);
            }
        }

        //visual drag display of connection
        if (Input.GetMouseButton(1) && startingConnection != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (editorConnection == null) editorConnection = Object.Instantiate(editor.editorConnection).GetComponent<TowerConnection>();
            if (Physics.Raycast(ray, out hit, 50000.0f, LayerMask.GetMask("terrain")) && !EventSystem.current.IsPointerOverGameObject())
            {
                editorConnection.Drag(startingConnection, hit.point);
            }
        }




        //delete selected towers
        if (Input.GetKeyUp(KeyCode.Delete) || Input.GetKeyUp(KeyCode.X))
        {

            foreach (KeyValuePair<int, GameObject> tower in editor.editorSelection.selectedDictionary.selected)
            {
                editor.editorTowers.Remove(tower.Value.GetComponent<EditorTower>());
                tower.Value.GetComponent<EditorTower>().RemoveID();
                Object.Destroy(tower.Value);

            }
            editor.editorSelection.selectedDictionary.RemoveAllEditor();
            EditorOptions.Instance.SelectedEditorTowers();
        }

        //check what we are clicking on
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                CheckIfClickedOnBuilding(editor);//if click on building select it/ if on terrain place new building
            }
            else
            {
                if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.tag == "uiDontTest") { Debug.Log(EventSystem.current.currentSelectedGameObject.name); return; }

                editor.editorSelection.selectedDictionary.RemoveAllEditor();
                EditorOptions.Instance.SelectedEditorTowers();
            }


        }


        if (Input.GetMouseButtonUp(0))
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //if we are placing a building, check if it can be placed, place it, and select it
            if (startedPlaceBuilding && !editor.editorSelection.dragSelect && Physics.Raycast(ray, out hit, 50000.0f, LayerMask.GetMask("terrain")) && !EventSystem.current.IsPointerOverGameObject())
            {

                GameObject obj = Object.Instantiate(editor.editorTowerPrefab, hit.point, Quaternion.identity);
                EditorTower tower = obj.GetComponent<EditorTower>();
                //apply default preset and overrides if there are any
                tower.SetPreset(editor.towerPresets.presetData, editor.towerPresets.GetName());
                foreach (towerEditorToggle toggle in editor.toggles)
                {
                    if (!toggle.toggle.isOn)
                    {
                        tower.AddOverride(toggle.slider.textString, toggle.slider.sliderElement.value);
                    }
                }
                EditorManager.Instance.editorTowers.Add(tower);
                tower.SetId();
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    editor.editorSelection.selectedDictionary.RemoveAllEditor();
                }
                editor.editorSelection.selectedDictionary.AddSelectedEditor(obj);
                EditorOptions.Instance.SelectedEditorTowers();

            }
            else
            {
                //stuff if we clicked on a tower instead of the empty ground
                if (Physics.Raycast(ray, out hit, 50000.0f, LayerMask.GetMask("building")) && !EventSystem.current.IsPointerOverGameObject())
                {
                    if (moveDistance < 0.1f)
                    {
                        if (!startedPlaceBuilding && !editor.editorSelection.dragSelect)
                        {

                            bool selected = hit.collider.gameObject.GetComponent<EditorTower>().selected;

                            if (!Input.GetKey(KeyCode.LeftShift))
                            {

                                editor.editorSelection.selectedDictionary.RemoveAllEditor();
                                if (!selected)
                                    editor.editorSelection.selectedDictionary.AddSelectedEditor(hit.collider.gameObject);
                            }
                            else
                            {
                                if (selected)
                                    editor.editorSelection.selectedDictionary.RemoveSelectedEditor(hit.collider.gameObject.GetInstanceID());
                                else
                                    editor.editorSelection.selectedDictionary.AddSelectedEditor(hit.collider.gameObject);
                            }

                            EditorOptions.Instance.SelectedEditorTowers();

                        }
                    }
                }
            }
            moveDistance = 0;
            startedPlaceBuilding = false;
        }

        //do drag and move selected towers
        if (Input.GetMouseButton(0) && !startedPlaceBuilding && editor.editorSelection.selectedDictionary.selected.Count > 0 && clickedOnSelectedBuilding)
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 50000.0f, LayerMask.GetMask("terrain")) && !EventSystem.current.IsPointerOverGameObject())
            {

                moveDistance += (hit.point - previousPosition).magnitude;


                foreach (KeyValuePair<int, GameObject> tower in editor.editorSelection.selectedDictionary.selected)
                {

                    tower.Value.GetComponent<EditorTower>().MoveTo(tower.Value.transform.position + hit.point - previousPosition);
                }
                previousPosition = hit.point;
            }
        }
    }

    void CheckIfClickedOnBuilding(EditorManager editor)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 50000.0f, LayerMask.GetMask("building")) && !EventSystem.current.IsPointerOverGameObject())
        {
            startedPlaceBuilding = false;
            clickedOnSelectedBuilding = hit.collider.gameObject.GetComponent<EditorTower>().selected;
            previousPosition = hit.point;
            editor.editorSelection.enableSelection = false;

            if (Physics.Raycast(ray, out hit, 50000.0f, LayerMask.GetMask("terrain")) && !EventSystem.current.IsPointerOverGameObject())
            {
                previousPosition = hit.point;
            }

        }
        else
        {
            startedPlaceBuilding = true;
            editor.editorSelection.enableSelection = true;
        }
    }
}
