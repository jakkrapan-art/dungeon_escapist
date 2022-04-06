using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Snatcher : NetworkBehaviour
{
    //1ตัวSnatcher หยิบของไม่เข้า parent 
    //2เวลาของcube ไม่หยุด ทำSync
    [SerializeField] public BoardPlayer owner;
    [SyncVar]
    public GameObject carriedBox;
    public GameObject targetBoxObj;
    //private SceneSnatcherScript snatcherScene;
    private new Rigidbody rigidbody;
    [SyncVar]
    private bool Carrying;
    [SerializeField] private Transform hand;
    [SerializeField] private float speedMove = 20f;
    [SerializeField] private float viewRange = 20f;

    [SyncVar(hook = nameof(OnColorChanged))]
    public Color characterColor = Color.white;


    [Server]
    public void setOwner(BoardPlayer player)
    {
        owner = player;
    }

    public void setColor(Color color)
    {
        characterColor = color;
    }
    public bool isCarrying()
    {
        return Carrying;
    }
    void Awake()
    {
        //allow all players to run this
        //snatcherScene = GameObject.FindObjectOfType<SceneSnatcherScript>();
        InvokeRepeating("lockTargetBox", 0f, 0.2f);
        rigidbody = GetComponent<Rigidbody>();

    }

    void OnColorChanged(Color _Old, Color _New)
    {
        transform.Find("Body").GetComponent<Renderer>().material.color = _New;
    }


    public void SnatcherMovement(Vector2 joyValue)//
    {
        float moveX = joyValue.x * speedMove;
        float moveZ = joyValue.y * speedMove;
        Vector3 movement = new Vector3(moveX, rigidbody.velocity.y, moveZ);
        rigidbody.velocity = movement;
        if (joyValue.x != 0 || joyValue.y != 0)
        {
            rigidbody.rotation = Quaternion.LookRotation(-movement);
        }

    }

    private void lockTargetBox()
    {

        GameObject[] targetBoxs = GameObject.FindGameObjectsWithTag("letterBox");
        float shortestDistance = Mathf.Infinity;
        GameObject nearestTarget = null;
        foreach (GameObject target in targetBoxs)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
            if (distanceToTarget < shortestDistance)
            {
                shortestDistance = distanceToTarget;
                nearestTarget = target;
            }
        }
        if (nearestTarget != null && shortestDistance <= viewRange && nearestTarget != carriedBox)
        {
            targetBoxObj = nearestTarget;
            if (targetBoxObj != null && shortestDistance < Vector3.Distance(transform.position, targetBoxObj.transform.position))
            {

                targetBoxObj = nearestTarget;
            }
        }
        else
        {
            targetBoxObj = null;
        }
    }
    [Command]
    public void CmdPlayerDiscardBox()
    {
        if (carriedBox != null)
        {
            carriedBox.GetComponent<Rigidbody>().useGravity = true;
            carriedBox.GetComponent<LetterCube>().Dorpped();
            carriedBox.transform.parent = null;
            carriedBox = null;
            Carrying = false;
        }
    }

    [Command]
    public void CmdPlayerSnatch(GameObject letterCube)
    {
        LetterCube cube = letterCube.GetComponent<LetterCube>();
        if (!cube.IsCarried)
        {
            cube.GetComponent<Rigidbody>().useGravity = false;
            //cube.transform.parent = hand;
            //cube.transform.localPosition = hand.position;//
            cube.Parent = hand.transform;
            carriedBox = letterCube;
            cube.Carried();
            Carrying = true;
        }
    }

    public void sendLetterCube()//method ให้houseสั่ง
    {
        Destroy(carriedBox);
        NetworkServer.Destroy(carriedBox);
        Carrying = false;
    }

}
