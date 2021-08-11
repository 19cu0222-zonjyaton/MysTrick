using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGroundSensor : MonoBehaviour
{
    public CapsuleCollider capcol;      //  colliderオブジェクト
    public GoalController goal;         //  ゴールコントローラー
    public CameraController ca;         //  カメラコントローラー

    private PlayerInput pi;             //  プレイヤーの入力コントローラー
    private ActorController ac;         //  プレイヤーの挙動コントローラー
    private Vector3 point1;             //  当たり判定最高点
    private Vector3 point2;             //  当たり判定最低点
    private float radius;               //  プレイヤーの半径

    //  初期化
    void Awake()
    {
        radius = capcol.radius;

        goal = GameObject.Find("Goal").GetComponent<GoalController>();

        ca = GameObject.Find("Main Camera").GetComponent<CameraController>();

        pi = gameObject.GetComponentInParent<PlayerInput>();    //  親のComponentを獲得する

        ac = GameObject.Find("PlayerHandle").GetComponent<ActorController>();
    }

    void FixedUpdate()
    {
        point1 = transform.position + transform.up * radius;      //  最高点
        point2 = transform.position - transform.up * radius;      //  最低点
        
        Collider[] outputCols = Physics.OverlapCapsule(point1, point2 ,radius, LayerMask.GetMask("Ground"));        //  Groundという名前の階層と当てますか

        if (outputCols.Length == 0)
        {
            pi.ResetSignal();
        }
        else if (outputCols.Length != 0 && !pi.lockJumpStatus && !goal.gameClear && ca.cameraStatic == "Idle" && !ac.isUnrivaled && !ac.isDead)
        {
            pi.inputEnabled = true;
        }
    }
}
