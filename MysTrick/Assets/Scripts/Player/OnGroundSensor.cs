using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGroundSensor : MonoBehaviour
{
    public CapsuleCollider capcol;      //  colliderオブジェクト
    public GoalController goal;         //  ゴールコントローラー
    public CameraController cc;         //  カメラコントローラー

    private PlayerInput pi;             //  プレイヤーの入力コントローラー
    private ActorController ac;         //  プレイヤーの挙動コントローラー
    private Animator anim;				//	アニメコントローラーコンポーネント
    private Vector3 point1;             //  当たり判定最高点
    private Vector3 point2;             //  当たり判定最低点
    private float radius;               //  プレイヤーの半径
    public bool isJumpStatic;

    //  初期化
    void Awake()
    {
        radius = capcol.radius;

        goal = GameObject.Find("Goal").GetComponent<GoalController>();

        cc = GameObject.Find("Main Camera").GetComponent<CameraController>();

        pi = gameObject.GetComponentInParent<PlayerInput>();    //  親のComponentを獲得する

        anim = GameObject.Find("PlayerModule").GetComponent<Animator>();

        ac = GameObject.Find("PlayerHandle").GetComponent<ActorController>();
    }

    void FixedUpdate()
    {
        point1 = transform.position + transform.up * radius;      //  最高点
        point2 = transform.position - transform.up * radius;      //  最低点
        
        Collider[] outputCols = Physics.OverlapCapsule(point1, point2 ,radius, LayerMask.GetMask("Ground"));        //  Groundという名前の階層と当てますか

        if (outputCols.Length == 0 && !ac.isClimbing)
        {
            if (ac.isJumping)
            {
                isJumpStatic = true;
            }

            if (!isJumpStatic)
            {
                ac.isFall = true;
                anim.SetBool("Fall", true);
                pi.ResetSignal();
            }
        }
        else if (outputCols.Length != 0)
        {
            isJumpStatic = false;
            if (ac.isFall)
            {
                anim.SetBool("Fall", false);
                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && anim.GetCurrentAnimatorStateInfo(0).IsName("Land"))
                {
                    ac.isFall = false;
                }
            }

            if (ac.PlayerCanMove())
            {
                pi.inputEnabled = true;
            }
        }
    }
}
