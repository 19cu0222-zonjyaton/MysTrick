using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathControl : MonoBehaviour
{
    public float speed = 1.5f;
    public float field = 20.0f;
    public GameObject player;
    public Transform[] pathPositions;
    public Transform head;
    public EnemyDamageController edc;
    public CameraController cc;

    private ActorController ac;
    private int index = 1;
    private bool islock = false;
    private bool isAttackedByPlayer;
    private float countTime;
    private bool targetIsLost;
    private Ray ray;
    private RaycastHit hit;

    void Awake()
    {
        ac = player.GetComponent<ActorController>();
    }

    void Update()
    {
        if (edc.isDamage)
        {
            isAttackedByPlayer = true;
        }

        if (cc.cameraStatic == "Idle")
        {
            if (LockOn())
            {
                ray = new Ray(transform.position, (player.transform.position - head.transform.position).normalized);
                Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 0.1f);
            }
            else if(!ac.isDead || !ac.isFall)
            {
                ray = new Ray(transform.position, head.forward);
                Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 0.1f);
            }

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                //もしrayとhitが衝突した場合の処理内容
                if (hit.collider.gameObject.transform.tag == "Player")
                {
                    //視線はもしプレイヤーと当たったら処理内容
                    Move();
                }
                else
                {
                    Patrol();
                }
            }
        }
    }

    bool LockOn()
    {
        if (System.Math.Abs(player.transform.position.y - transform.position.y) >= 2.0f)
            islock = false;
        else
        {
            Vector3 targetPosition = player.transform.position;
            if (Vector3.Distance(targetPosition, transform.position) >= field)
                islock = false;
            else
            {
                float angle = 0.0f;
                float headAngle;
                float oppositeAngle = 0.0f;
                headAngle = head.transform.localEulerAngles.y;
                if (targetPosition.z < transform.position.z)
                {
                    oppositeAngle = Mathf.Acos((targetPosition.x - transform.position.x) / Vector3.Distance(targetPosition, transform.position)) * Mathf.Rad2Deg;
                }
                else if (targetPosition.z > transform.position.z)
                {
                    oppositeAngle = -Mathf.Acos((targetPosition.x - transform.position.x) / Vector3.Distance(targetPosition, transform.position)) * Mathf.Rad2Deg;
                }
                if (oppositeAngle <= 0 && oppositeAngle >= -180.0f)
                {
                    oppositeAngle = 360 + oppositeAngle;
                }
                angle = oppositeAngle - headAngle + 90.0f;
                if (angle <= 0.0f && angle >= -60.0f || angle <= 60.0f && angle > 0.0f || angle <= 360.0f && angle >= 300.0f)
                {
                    islock = true;
                }
                else
                { 
                    islock = false;
                }
            }
        }
        return islock;
    }
    void Patrol()
    {
        //targetIsLost = false;
        transform.Translate((pathPositions[index].position - transform.position).normalized * Time.deltaTime * speed );
        Vector3 targetPosition = pathPositions[index].transform.position;
        targetPosition.y = head.position.y;
        head.LookAt(targetPosition);
        if (Vector3.Distance(pathPositions[index].position, transform.position) < 0.2f)
            index++;
        if (index > pathPositions.Length - 1)
            index = 0;
    }
    void Move()
    {
        transform.Translate((player.transform.position - transform.position).normalized * Time.deltaTime * speed);
        Vector3 targetPosition = player.transform.position;
        targetPosition.y = head.position.y;
        head.LookAt(targetPosition);
    }

    void OnCollisionEnter(Collision collision)      
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isAttackedByPlayer = false;
        }
    }

    //void OnCollisionStay(Collision collision)
    //{
    //    if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") && LockOn())
    //    {
    //        countTime += Time.deltaTime;
    //        if (countTime > 1.0f)
    //        {
    //            print(targetIsLost);
    //            targetIsLost = true;
    //            countTime = 0.0f;
    //        }
    //    }
    //}

    //void OnCollisionExit(Collision collision)
    //{
    //    if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
    //    {
    //        targetIsLost = false;
    //        countTime = 0.0f;
    //    }
    //}
}

