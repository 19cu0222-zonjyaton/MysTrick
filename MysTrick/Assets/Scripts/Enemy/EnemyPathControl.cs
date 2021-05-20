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
        if (System.Math.Abs(GameObject.Find("PlayerHandle").GetComponent<Transform>().position.y - transform.position.y) >= 1.0f)
            islock = false;
        else
        {
            if (Vector3.Distance(GameObject.Find("PlayerHandle").GetComponent<Transform>().position , transform.position) >= field)
                islock = false;
            else
                islock = true;
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

