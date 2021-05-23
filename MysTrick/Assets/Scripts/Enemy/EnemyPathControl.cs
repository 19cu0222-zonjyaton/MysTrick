using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathControl : MonoBehaviour
{
    public float speed = 1.5f;
    public float field = 5.0f;
    public Transform[] pathPositions;
    public Transform head;
    private int index = 1;
    private bool islock = false;

    void Update()
    {
        if (!LockOn()) Patrol();
        else Move();
    }

    bool LockOn()
    {
        if (System.Math.Abs(GameObject.Find("PlayerHandle").GetComponent<Transform>().position.y - transform.position.y) >= 2.0f)
            islock = false;
        else
        {
            Vector3 targetPosition = GameObject.Find("PlayerHandle").GetComponent<Transform>().position;
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
                    islock = true;
                else
                    islock = false;
            }

        }
        return islock;
    }
    void Patrol()
    {
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
        transform.Translate((GameObject.Find("PlayerHandle").GetComponent<Transform>().position - transform.position).normalized * Time.deltaTime * speed);
        Vector3 targetPosition = GameObject.Find("PlayerHandle").GetComponent<Transform>().position;
        targetPosition.y = head.position.y;
        head.LookAt(targetPosition);
    }
}

