using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingShoot : MonoBehaviour
{
    public float checkRadious = 10f;
    public float shootDelay = 0.1f;
    public float shootRandomness = 0.1f;
    public float shotLifeTime = 0.5f;
    public GameObject gunObj;
    public GameObject shotObj;
    [System.NonSerialized]
    public BuildingMain building;
    private float timer = 0;


    // Start is called before the first frame update
    void OnEnable()
    {
        gunObj.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer < shootDelay) return;

        timer = 0f + UnityEngine.Random.Range(-shootRandomness, shootRandomness);

        Collider[] contextColliders = Physics.OverlapSphere(transform.position, checkRadious, LayerMask.GetMask("unit"));
        foreach (Collider unit in contextColliders)
        {
            UnitAgent unitAgent = unit.GetComponent<UnitAgent>();

            if (unitAgent.Controller == building.unitController) continue;

            if (unitAgent.selfTeam != building.team.teamid && !unitAgent.isGift) //is enemy and is not gift
            {
                gunObj.transform.LookAt(unit.transform.position);
                Hit hit = Instantiate(shotObj, gunObj.transform.position, Quaternion.identity).GetComponent<Hit>();
                hit.Initialize(gunObj.transform.position, unitAgent.transform.position, shotLifeTime);
                UnitPool.Instance.ReurnUnitsToPool(unitAgent);
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayGunSound(transform.position);
                }
                return;
            }
        }
    }
}
