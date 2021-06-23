using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public float rotateSpeed;       //  回転速度
    public float speed;             //  スタートの移動速度
    public GameObject rightHand;    //  プレイヤーの右手
    public bool backToHand = true;
    public GameObject playerPos;   //  プレイヤーの位置を獲得するため
    public GameObject model;       //  プレイヤーの回転方向を獲得するため
    public Vector3[] otherPos;
    public Vector3[] startPos;

    private new Rigidbody rigidbody;
    private PlayerInput pi;         //  攻撃ができるかどうかの判断
    private ActorController ac;
    private Animator anim;
    private Vector3 tempVec;
    private float speedDown;        //  戻る時の速度
    private float timeCount;
    private CameraController cc;

    void Awake()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();

        pi = playerPos.GetComponent<PlayerInput>();

        ac = playerPos.GetComponent<ActorController>();

        anim = model.GetComponent<Animator>();

        startPos[0] = transform.localPosition;

        startPos[1] = transform.localEulerAngles;

        cc = GameObject.Find("Main Camera").GetComponent<CameraController>();
    }

    void Start()
    {
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    void FixedUpdate()
    {
        if (cc.canThrowWeapon && !pi.canThrow)
        {
            backToHand = false;
            timeCount += Time.fixedDeltaTime;
            anim.SetLayerWeight(anim.GetLayerIndex("Throw"), 1.0f);

            if (timeCount >= 0.1f)
            {
                transform.parent = null;
                rigidbody.constraints = RigidbodyConstraints.None;

                transform.Rotate(0.0f, rotateSpeed, 0.0f);

                if (speed > 0.0f)
                {
                    gameObject.transform.tag = "Weapon";

                    rigidbody.position += tempVec * speed * Time.fixedDeltaTime;     //  プレイヤーの正方向に一定距離を移動する

                    speed -= 1.5f;
                }
                else
                {
                    gameObject.transform.tag = "Untagged";

                    speedDown += 0.2f;

                    transform.position = Vector3.Lerp(transform.position, playerPos.transform.position + new Vector3(0.0f, 1.0f, 0.0f), speedDown * Time.fixedDeltaTime);     //  プレイヤーの位置に戻る
                }
            }
            else
            {
                transform.localEulerAngles = new Vector3(0, 81.0f, 0);
                tempVec = model.transform.forward;
            }
        }
        else if (ac.isJumping)      //  ジャンプと登る状態は武器を収める
        {
            transform.localPosition = new Vector3(-1.256f, -0.043f, -0.541f);
            transform.localEulerAngles = new Vector3(-36.247f, 62.479f, 126.3f);
            otherPos[0] = transform.localPosition;
            otherPos[1] = transform.localEulerAngles;
        }
        else if (ac.isClimbing)
        {
            transform.localPosition = new Vector3(-0.91f, -0.16f, 0.81f);
            transform.localEulerAngles = new Vector3(-29.24f, 200.724f, 104.937f);
            otherPos[0] = transform.localPosition;
            otherPos[1] = transform.localEulerAngles;
        }
        else
        {
            transform.localPosition = startPos[0];                     //  親に相対の座標を設定する
            transform.localEulerAngles = startPos[1];                  //  親に相対の角度を設定する   
        }


        if (backToHand)             //  持っている武器を隠す処理
        {
            if (pi.isAimStatus)
            {
                gameObject.GetComponent<MeshRenderer>().enabled = false;
            }
            else
            {
                gameObject.GetComponent<MeshRenderer>().enabled = true;
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.tag == "Player" && speed <= 0.0f)
        {
            backToHand = true;
            pi.canThrow = true;
            cc.canThrowWeapon = true;

            transform.parent = rightHand.transform;

            speedDown = 0.0f;
            speed = 40.0f;
            timeCount = 0.0f;
            anim.SetLayerWeight(anim.GetLayerIndex("Throw"), 0.0f);
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }
    }
}
