using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerSelectionDictionary : MonoBehaviour
{
    public Dictionary<int, GameObject> selected = new Dictionary<int, GameObject>();
    public BuildingUI optionsActive;

    public static playerSelectionDictionary Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void AddSelected(GameObject add)
    {
        int key = add.GetInstanceID();

        if (!(selected.ContainsKey(key)))
        {
            selected.Add(key, add);
            add.GetComponent<BuildingUI>().Selected();
        }
    }

    public void AddSelectedEditor(GameObject add)
    {
        int key = add.GetInstanceID();

        if (!(selected.ContainsKey(key)))
        {
            selected.Add(key, add);
            add.GetComponent<EditorTower>().Selected();
        }
    }

    public void RemoveSelected(int removeKey)//key id .GetInstanceID()
    {
        selected[removeKey].GetComponent<BuildingUI>().Deselected();
        selected.Remove(removeKey);
    }



    public void RemoveAll()
    {
        if (optionsActive != null)
        {
            removeOprionsSelected();
        }

        foreach (KeyValuePair<int, GameObject> pair in selected)
        {
            if (pair.Value != null)
            {
                selected[pair.Key].GetComponent<BuildingUI>().Deselected();
            }
        }
        selected.Clear();
    }

    public void RemoveAllEditor()
    {
        foreach (KeyValuePair<int, GameObject> pair in selected)
        {
            if (pair.Value != null)
            {
                selected[pair.Key].GetComponent<EditorTower>().Deselected();
            }
        }
        selected.Clear();
    }

    public void RemoveSelectedEditor(int removeKey)//key id .GetInstanceID()
    {
        Debug.Log(selected[removeKey].name);
        selected[removeKey].GetComponent<EditorTower>().Deselected();
        selected.Remove(removeKey);
    }

    public void addOprionsSelected(BuildingUI building)
    {
        optionsActive = building;
    }

    public void removeOprionsSelected()
    {
        if (optionsActive != null)
        {
            optionsActive.Deselected();
            optionsActive = null;
        }
    }

    public void Attack(Transform attack, int percent)
    {
        foreach (KeyValuePair<int, GameObject> pair in selected)
        {
            if (pair.Value != null)
            {
                if (selected[pair.Key] != attack.gameObject)
                {
                    selected[pair.Key].GetComponent<UnitController>().Attack(percent, attack);
                }
                else
                {
                    selected[pair.Key].GetComponent<UnitController>().StopAttackUnits();
                }
            }
        }
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySendSound();
        }
    }

    public void ContinuousAttack()
    {
        foreach (KeyValuePair<int, GameObject> pair in selected)
        {
            if (pair.Value != null)
            {
                if (selected[pair.Key] != optionsActive.gameObject)
                {
                    selected[pair.Key].GetComponent<UnitController>().ContinuousAttack(optionsActive.transform);
                }
            }
        }
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySendSound();
        }
    }

    public void Gift(int percent)
    {
        foreach (KeyValuePair<int, GameObject> pair in selected)
        {
            if (pair.Value != null)
            {
                if (selected[pair.Key] != optionsActive.gameObject)
                {
                    selected[pair.Key].GetComponent<UnitController>().Gift(percent, optionsActive.transform);
                }
            }
        }
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayAudioClip(7);
        }
    }

}
