using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public float rotateSpeed;       //  回転速度
    public float speed;             //  スタートの移動速度
    public GameObject rightHand;    //  プレイヤーの右手
    public bool backToHand = true;
    public GameObject model;       //  プレイヤーの回転方向を獲得するため

    private new Rigidbody rigidbody;
    private GameObject playerPos;   //  プレイヤーの位置を獲得するため
    private PlayerInput pi;         //  攻撃ができるかどうかの判断
    private Animator anim;
    private Vector3 tempVec;
    private float speedDown;        //  戻る時の速度
    private float timeCount;
    private CameraController cc;

    void Awake()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();

        playerPos = GameObject.Find("PlayerHandle");

        model = GameObject.Find("PlayerModule");

        pi = GameObject.Find("PlayerHandle").GetComponent<PlayerInput>();

        anim = GameObject.Find("PlayerModule").GetComponent<Animator>();

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
            transform.localPosition = new Vector3(-0.2f, 0.06f, 1.12f);             //  親に相対の座標を設定する
            transform.localEulerAngles = new Vector3(16.0f, 67.0f, 170.0f);         //  親に相対の角度を設定する
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }
    }
}
