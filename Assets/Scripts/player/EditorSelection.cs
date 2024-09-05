using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EditorSelection : Selection
{
    public bool enableSelection = false;
    void Start()
    {
        selectedDictionary = GameObject.FindGameObjectWithTag("Player").GetComponent<playerSelectionDictionary>();
        dragSelect = false;

    }

    void Update()
    {
        if (enableSelection)
            Select();
    }


    public override void RayCastLeftClick()
    {


    }

    public override void RayCastRightClick()
    {
        /* Ray ray = Camera.main.ScreenPointToRay(p1);

         if (Physics.Raycast(ray, out hit, 50000.0f, LayerMask.GetMask("building")) && !EventSystem.current.IsPointerOverGameObject())
         {

         }
         return;*/
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("" + other.gameObject.name);
        if (other.gameObject.GetComponent<EditorTower>() != null)
        {
            selectedDictionary.AddSelectedEditor(other.gameObject);
            EditorOptions.Instance.SelectedEditorTowers();
        }
    }
}
