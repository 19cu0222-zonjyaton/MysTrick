using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathControl : MonoBehaviour
{
    public float speed;
    public float field;
    public GameObject player;
    public Transform[] pathPositions;
    public Transform head;
    public EnemyDamageController edc;
    public CameraController cc;

    private PlayerInput pi;
    private ActorController ac;
    private int index = 1;
    private bool islock = false;
    private bool isAttackedByPlayer;
    private Ray ray;
    private RaycastHit hit;
    void Awake()
    {
        pi = player.GetComponent<PlayerInput>();

        ac = player.GetComponent<ActorController>();
    }

    void Update()
    {
        if (edc.isDamage)
        {
            isAttackedByPlayer = true;
        }

        if (pi.inputEnabled)
        {
            if (LockOn())
            {
                ray = new Ray(transform.position, ((player.transform.position + new Vector3(0, 1.5f, 0)) - head.transform.position).normalized);
                Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 0.1f);
            }
            else
            {
                ray = new Ray(transform.position, head.forward);
                Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 0.1f);
            }

            Physics.Raycast(ray, out hit, Mathf.Infinity);

            if (hit.collider != null && edc.canMove)       //  空を見てフリーズの対策
            {
                hit.collider.enabled = true;
                if (!(hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall")) && LockOn() || isAttackedByPlayer)
                {
                    Move();
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
                    {
                        isAttackedByPlayer = false;
                    }
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
                    oppositeAngle = Mathf.Acos((targetPosition.x - transform.position.x) / Vector3.Distance(targetPosition, transform.position + new Vector3(0, 1.5f, 0))) * Mathf.Rad2Deg;
                }
                else if (targetPosition.z > transform.position.z)
                {
                    oppositeAngle = -Mathf.Acos((targetPosition.x - transform.position.x) / Vector3.Distance(targetPosition, transform.position + new Vector3(0, 1.5f, 0))) * Mathf.Rad2Deg;
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
        transform.Translate((pathPositions[index].position - transform.position).normalized * Time.deltaTime * speed);
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
}

