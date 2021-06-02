using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public float rotateSpeed;       //  回転速度
    public float speed;             //  スタートの移動速度
    public GameObject rightHand;    //  プレイヤーの右手

    private new Rigidbody rigidbody;
    private GameObject playerPos;   //  プレイヤーの位置を獲得するため
    private PlayerInput pi;         //  攻撃ができるかどうかの判断
    public GameObject model;       //  プレイヤーの回転方向を獲得するため
    private GameObject playerCamera;
    private Animator anim;
    private Vector3 tempVec;
    private float speedDown;        //  戻る時の速度
    private float timeCount;

    void Awake()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();

        playerPos = GameObject.Find("PlayerHandle");

        model = GameObject.Find("PlayerModule");

        pi = GameObject.Find("PlayerHandle").GetComponent<PlayerInput>();

        playerCamera = GameObject.Find("Main Camera");

        anim = GameObject.Find("PlayerModule").GetComponent<Animator>();
    }

    void Start()
    {
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    void FixedUpdate()
    {
        if (!pi.canAttack)
        {
            timeCount += Time.fixedDeltaTime;

            anim.SetLayerWeight(anim.GetLayerIndex("Throw"), 1.0f);

            if (timeCount >= 0.1f)
            {
                transform.parent = null;
                rigidbody.constraints = RigidbodyConstraints.None;

                transform.Rotate(0.0f, rotateSpeed, 0.0f);

                speed -= 1.5f;

                if (!pi.isAimStatus)
                {
                    if (speed > 0.0f)
                    {
                        gameObject.transform.tag = "Weapon";

                        rigidbody.position += tempVec * speed * Time.fixedDeltaTime;     //  プレイヤーの正方向に一定距離を移動する
                    }
                    else
                    {
                        gameObject.transform.tag = "Untagged";

                        speedDown += 0.004f;

                        transform.position = Vector3.Lerp(transform.position, playerPos.transform.position + new Vector3(0.0f, 1.0f, 0.0f), speedDown);     //  プレイヤーの位置に戻る
                    }
                }
                else
                {
                    if (speed > 0.0f)
                    {
                        gameObject.transform.tag = "Weapon";

                        rigidbody.position += playerCamera.transform.forward * speed * Time.fixedDeltaTime;     //  カメラの正方向に一定距離を移動する
                    }
                    else
                    {
                        gameObject.transform.tag = "Untagged";

                        speedDown += 0.004f;

                        transform.position = Vector3.Lerp(transform.position, playerPos.transform.position, speedDown);     //  プレイヤーの位置に戻る
                    }
                }
            }
            else
            {
                transform.localEulerAngles = new Vector3(0, 81.0f, 0);
                tempVec = model.transform.forward;
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.tag == "Player" && speed <= 0.0f)
        {
            transform.parent = rightHand.transform;

            pi.canAttack = true;

            timeCount = 0.0f;
            speedDown = 0.0f;
            speed = 40.0f;

            anim.SetLayerWeight(anim.GetLayerIndex("Throw"), 0.0f);
            transform.localPosition = new Vector3(-0.2f, 0.06f, 1.12f);
            transform.localEulerAngles = new Vector3(16.0f, 67.0f, 170.0f);
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }
    }
}
