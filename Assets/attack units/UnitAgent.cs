using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider))]
public class UnitAgent : MonoBehaviour
{
    Collider agentCollider;
    public Collider AgentCollider { get { return agentCollider; } }

    public float floatHeight = 0;
    [SerializeField]
    Queue<Transform> trackPositions = new Queue<Transform>();//ide prema ovoj poziciji
    public Queue<Transform> TrackPositions { get { return trackPositions; } }

    UnitController controller;
    public UnitController Controller { get { return controller; } }

    public Vector2 currentMoveDirection = new Vector2(0, 0);

    public int selfTeam = int.MinValue;
    public int id;
    public bool isGift = false;
    public float updateTime;
    float timeOffest;

    public float incrementMove = 0.5f;
    float incrementOffset;

    public Renderer rend;
    public MeshFilter mesh;
    public Transform obj;
    public Renderer markerRend;
    float materalTeamTextureOffset;

    //float timer;
    //public float moveTime = 1;
    //public Vector3 moveHere;
    //Quaternion currentRottation;

    // Start is called before the first frame update
    void Start()
    {
        agentCollider = GetComponent<Collider>();
        timeOffest = Random.Range(0, updateTime);
        //timer = moveTime+1;
        incrementMove = Random.Range(incrementMove * 0.5f, incrementMove);
        incrementOffset = Random.Range(0, incrementMove);

    }

    public void Initialize(UnitController unitController)
    {
        controller = unitController;
        InvokeRepeating("UpdateAgent", timeOffest, updateTime);
        //rend.material.SetColor("Color_ID", selfCol);
    }

    void UpdateAgent()
    {

        controller.UpdateAgent(this);
    }

    public void Move(Vector2 velocity)
    {
        if (velocity != Vector2.zero)
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(velocity.x, 0, velocity.y));
            float y = SetHeight();
            transform.position += new Vector3(velocity.x * Time.deltaTime, y, velocity.y * Time.deltaTime);
            CheckAttack(velocity);
            //UpdateObjectPos3(incrementMove);
        }
    }

    float SetHeight()
    {
        Vector3 offset = new Vector3(0, 52, 0);
        RaycastHit hit;
        if (!Physics.Raycast(transform.position + offset, Vector3.down, out hit, offset.y * 2, LayerMask.GetMask("terrain")))
        {
            Debug.Log(gameObject.name + "nes nera di bur z");
            return 0;
        }
        return floatHeight - hit.distance + offset.y;
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
                if (hit.transform.name[1] == 'g' && !isGift)//chek if gift 
                {
                    UnitAgent enemy = hit.transform.GetComponent<UnitAgent>();

                    UnitPool.Instance.ReurnUnitsToPool(enemy);// Destroy(hit.transform.gameObject);

                    UnitPool.Instance.ReurnUnitsToPool(this);// Destroy(gameObject);
                }
            }
        }
    }
    /*
        void UpdateObjectPos1(float incrementMoveL)
        {

            Vector3 newPos = new Vector3(
                (((int)((transform.position.x) / incrementMoveL)) * incrementMoveL - obj.transform.position.x) * Mathf.Clamp01(Time.deltaTime * 50),
                transform.position.y - obj.transform.position.y,
                (((int)((transform.position.z) / incrementMoveL)) * incrementMoveL - obj.transform.position.z) * Mathf.Clamp01(Time.deltaTime * 50));

            obj.position += newPos;
        }
    */
    void UpdateObjectPos3(float incrementMoveL)
    {
        Vector3 newPos = new Vector3(
            (transform.position.x - (transform.position.x % incrementMoveL) - obj.transform.position.x + incrementOffset) * Mathf.Clamp01(Time.deltaTime * controller.unitMaxSPeed * 20),
            transform.position.y - obj.transform.position.y,
            (transform.position.z - (transform.position.z % incrementMoveL) - obj.transform.position.z + incrementOffset) * Mathf.Clamp01(Time.deltaTime * controller.unitMaxSPeed * 20));

        obj.position += newPos;
    }
    /*
    void UpdateObjectPos2()
    {
        timer += Time.deltaTime;
        if (timer > moveTime)
        {
            moveHere = transform.position;
            currentRottation = transform.rotation * Quaternion.Euler(-90, 0, 0);
            timer = Random.Range(moveTime/5,moveTime);
        }

        Vector3 newPos = new Vector3(
            (moveHere.x - obj.transform.position.x) * Mathf.Clamp01(Time.deltaTime * 50),
            transform.position.y - obj.transform.position.y,
            (moveHere.z - obj.transform.position.z) * Mathf.Clamp01(Time.deltaTime * 50));

        obj.position += newPos;
        obj.rotation = Quaternion.LookRotation(transform.position) * Quaternion.Euler(-90, 0, 0);
    }*/

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
        trackPositions.Dequeue();
    }

    public void SetUnitApperence(Mesh m, Material mat, Vector3 scale)
    {
        mesh.mesh = m;
        rend.material = mat;
        obj.localScale = scale;
        markerRend.transform.localScale = Vector3.one * (30 / scale.x);
    }

    public void SetColorVariant(int t)
    {
        materalTeamTextureOffset = 1f / rend.material.mainTexture.height;
        rend.material.mainTextureOffset = new Vector2(0, -materalTeamTextureOffset * t);
        markerRend.material.mainTextureOffset = new Vector2(0, -materalTeamTextureOffset * t);
    }


}


