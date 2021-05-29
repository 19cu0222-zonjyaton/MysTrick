using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGroundSensor : MonoBehaviour
{
    public CapsuleCollider capcol;
    public PlayerInput pi;
    public GoalController goal;
    public CameraController ca;

    private Vector3 point1;
    private Vector3 point2;
    private float radius;
    private ActorController ac;

    // Start is called before the first frame update
    void Awake()
    {
        radius = capcol.radius;

        pi = gameObject.GetComponentInParent<PlayerInput>();    //  親のComponentを獲得する

        goal = GameObject.Find("Goal").GetComponent<GoalController>();

        ca = GameObject.Find("Main Camera").GetComponent<CameraController>();

        ac = GameObject.Find("PlayerHandle").GetComponent<ActorController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        point1 = transform.position + transform.up * radius;                                            //  最高点
        point2 = transform.position + transform.up * capcol.height - transform.up * radius * 3.5f;      //  最低点

        Collider[] outputCols = Physics.OverlapCapsule(point1, point2 ,radius, LayerMask.GetMask("Ground"));        //  Groundという名前の階層と当てますか

        if (outputCols.Length == 0)
        {
            pi.inputEnabled = false;
        }
        else if (outputCols.Length != 0 && !pi.lockJumpStatus && !goal.gameClear && ca.cameraStatic == "Idle" && !ac.isUnrivaled && !ac.isDead)
        {
            pi.inputEnabled = true;
        }
    }
}
