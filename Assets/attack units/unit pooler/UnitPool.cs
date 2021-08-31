using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPool : MonoBehaviour
{

    [SerializeField] private UnitAgent unitPrefab;

    [SerializeField] private Queue<UnitAgent> unitsQueue = new Queue<UnitAgent>();

    public static UnitPool Instance { get; private set; }

    int number = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        AddUnitsToQueue(200);
    }

    public UnitAgent Get()
    {
        if(unitsQueue.Count == 0)
        {
            AddUnitsToQueue(1);
        }

        return unitsQueue.Dequeue();
    }

    void AddUnitsToQueue(int count)
    {
        for(int i = 0; i<count;i++)
        {
            UnitAgent unitInstantce = Instantiate(unitPrefab,transform);
            unitInstantce.gameObject.SetActive(false);
            unitInstantce.id = number++;
            unitsQueue.Enqueue(unitInstantce);
        }
    }

    public void ReurnUnitsToPool(UnitAgent unit)
    {
        unit.gameObject.SetActive(false);
        unit.Controller.agents.Remove(unit);
        unitsQueue.Enqueue(unit);
    }

}
