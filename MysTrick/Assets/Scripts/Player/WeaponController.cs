using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public float rotateSpeed;       //  回転速度
    public float speed;             //  スタートの移動速度
    public GameObject playerNeck;   //  平常時の親オブジェクト
    public GameObject playerHand;   //  投げる時の親オブジェクト
    public bool backToHand = true;
    public GameObject playerPos;    //  プレイヤーの位置を獲得するため
    public GameObject model;        //  プレイヤーの回転方向を獲得するため
    public Animator throwAnim;      //  投げるアニメ
    public Vector3[] otherPos;
    public Vector3[] startPos;
    public GameObject distanceCheck;

    private new Rigidbody rigidbody;
    private PlayerInput pi;         //  攻撃ができるかどうかの判断
    private Vector3 throwRot;
    private float speedDown;        //  戻る時の速度
    private float timeCount;
    private CameraController cc;
    
    //  操作感を上げるためのパラメータ
    private bool canBack;           
    private Vector3 distanceCheckPos;
    private Vector3 distanceCheckRot;

    void Awake()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();

        pi = playerPos.GetComponent<PlayerInput>();

        startPos[0] = transform.localPosition;

        startPos[1] = transform.localEulerAngles;

        cc = GameObject.Find("Main Camera").GetComponent<CameraController>();

        distanceCheckPos = distanceCheck.transform.localPosition;

        distanceCheckRot = distanceCheck.transform.localEulerAngles;
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
            throwAnim.SetLayerWeight(throwAnim.GetLayerIndex("Throw"), 1.0f);
            distanceCheck.transform.SetParent(null);

            if (timeCount >= 0.1f)
            {
                rigidbody.constraints = RigidbodyConstraints.None;

                transform.Rotate(0.0f, rotateSpeed, 0.0f);

                transform.SetParent(null);

                if (speed > 0.0f)
                {
                    gameObject.transform.tag = "Weapon";

                    rigidbody.position += throwRot * speed * Time.fixedDeltaTime;     //  プレイヤーの正方向に一定距離を移動する

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
                transform.SetParent(playerHand.transform);
                transform.localPosition = new Vector3(-0.229f, 0.019f, 1.117f);
                transform.localEulerAngles = new Vector3(0, 81.0f, 0);
                throwRot = model.transform.forward;
            }
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

    void OnTriggerStay(Collider collider)
    {
        if (collider.transform.tag == "Player" && speed <= 0.0f && canBack)
        {
            backToHand = true;
            pi.canThrow = true;
            cc.canThrowWeapon = true;
            distanceCheck.transform.tag = "GapCheck";
            canBack = false;

            //  チェックオブジェクトの位置を戻す
            distanceCheck.transform.SetParent(model.transform);   
            distanceCheck.transform.localPosition = distanceCheckPos;
            distanceCheck.transform.localEulerAngles = distanceCheckRot;

            transform.parent = playerNeck.transform;

            speedDown = 0.0f;
            speed = 40.0f;
            timeCount = 0.0f;
            throwAnim.SetLayerWeight(throwAnim.GetLayerIndex("Throw"), 0.0f);
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }

        if (collider.gameObject.layer == LayerMask.NameToLayer("Wall") && canBack)
        {
            speed = 0.0f;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.transform.tag == "GapCheck")
        {
            canBack = true;
            distanceCheck.transform.tag = "Untagged";
        }
    }
}
