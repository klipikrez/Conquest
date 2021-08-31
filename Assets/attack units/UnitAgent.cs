using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class UnitAgent : MonoBehaviour
{

    Collider agentCollider;
    public Collider AgentCollider { get { return agentCollider; } }

    public float floatHeight = 2;
    [SerializeField]
    Queue<Transform> trackPositions = new Queue<Transform>();//ide prema ovoj poziciji
    public Queue<Transform> TrackPositions { get { return trackPositions; } }

    UnitController controller;
    public UnitController Controller { get { return controller; } }

    public Vector2 currentMoveDirection = new Vector2(0,0);

    public int selfTeam;
    public Color selfCol;
    public int id;
    public bool isGift = false;

    public Renderer rend;

    // Start is called before the first frame update
    void Start()
    {
        agentCollider = GetComponent<Collider>();
        
    }

    public void Initialize(UnitController unitController)
    {
        controller = unitController;
        rend.material.SetColor("Color_ID", selfCol);
    }

    public void Move(Vector2 velocity)
    {

        if (velocity != Vector2.zero)
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(velocity.x, 0, velocity.y));
            float y = SetHeight()/ (Time.deltaTime * 20);
            transform.position += new Vector3(velocity.x , y, velocity.y) * Time.deltaTime;
        }

        CheckAttack(velocity);
    }

    float SetHeight()
    {
        Vector3 offset = new Vector3(0, 100, 0);
        RaycastHit hit;
        if (!Physics.Raycast(transform.position + offset, Vector3.down, out hit, 200f, LayerMask.GetMask("terrain")))
        {
            Debug.Log("nes nera di bur z");
            return 0;
        }
        
        return  floatHeight - hit.distance + offset.y;

    }

    void CheckAttack(Vector2 velocity)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, new Vector3(velocity.x, 0, velocity.y), out hit, 0.2f, LayerMask.GetMask("unit")))
        {


            if (hit.transform.name[0] != selfTeam.ToString()[0]) //prvo slovo imena im je isto kao broj na kom su timu ((int)teamid)
            {

                if (hit.transform.name[1] != 'g' && !isGift)
                {
                    UnitAgent enemy = hit.transform.GetComponent<UnitAgent>();

                    UnitPool.Instance.ReurnUnitsToPool(enemy);// Destroy(hit.transform.gameObject);

                    UnitPool.Instance.ReurnUnitsToPool(this);// Destroy(gameObject);
                }
            }
            else
            {
                if(hit.transform.name[1] == 'g' && !isGift)//chek if gift 
                {
                    UnitAgent enemy = hit.transform.GetComponent<UnitAgent>();

                    UnitPool.Instance.ReurnUnitsToPool(enemy);// Destroy(hit.transform.gameObject);

                    UnitPool.Instance.ReurnUnitsToPool(this);// Destroy(gameObject);
                }
            }
        }
    }

    public void AddPath(Transform[] path)
    {
        trackPositions.Clear();
        foreach (Transform point in path)
        {
            trackPositions.Enqueue(point);
        }
    }

    public void GoToNext()
    {
        rend.material.SetColor("Color_ID", Color.black); //debug
        trackPositions.Dequeue();
        
    }


    //void CheckAttack(Vector2 velocity)
    //{
    //    RaycastHit hit;
    //    if (Physics.Raycast(transform.position, new Vector3(velocity.x, 0, velocity.y), out hit, 0.2f, LayerMask.GetMask("unit", "building")))
    //    {

    //        if (hit.transform == trackPosition)
    //        {//stigli smo de treba
    //            Team team = hit.transform.GetComponent<Team>();

    //            if (team.teamid == selfTeam)
    //            {
    //                team.Reinforce();
    //            }
    //            else
    //            {
    //                team.Damage(this);
    //            }

    //            UnitPool.Instance.ReurnUnitsToPool(this);// Destroy(gameObject);
    //        }
    //        else
    //        {

    //            if (hit.transform.name[0] != selfTeam.ToString()[0]) //prvo slovo imena im je isto kao broj na kom su timu ((int)teamid)
    //            {
    //                if (hit.transform.gameObject.layer != 6)//6 je building layer. zbog nekog razlogga nije telo da mi radi LayerMask.GetMask("building") :(
    //                {//sudar sa neprijateljski unit
    //                    UnitPool.Instance.ReurnUnitsToPool(hit.transform.GetComponent<UnitAgent>());// Destroy(hit.transform.gameObject);

    //                    UnitPool.Instance.ReurnUnitsToPool(this);// Destroy(gameObject);
    //                }
    //            }
    //        }
    //    }
    //}

}


