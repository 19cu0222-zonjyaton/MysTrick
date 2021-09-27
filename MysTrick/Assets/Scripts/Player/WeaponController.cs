using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public float rotateSpeed;           //  回転速度
    public float speed;                 //  スタートの移動速度
    public GameObject playerNeck;       //  平常時の親オブジェクト
    public GameObject playerHand;       //  投げる時の親オブジェクト
    public bool backToHand = true;      //  投げる武器は手に戻るフラグ
    public GameObject playerPos;        //  プレイヤーの位置を獲得するため
    public GameObject model;            //  プレイヤーの回転方向を獲得するため
    public Animator throwAnim;          //  投げるアニメ
    public Vector3[] startPos;          //  武器の初期位置
    public GameObject distanceCheck;    //  壁との距離検査用オブジェクト

    private Rigidbody rigid;            //	武器の鋼体コンポーネント
    private BoxCollider weaponCollider;
    private PlayerInput pi;             //  攻撃ができるかどうかの判断
    private Vector3 throwRot;           //  投げる方向
    private float speedDown;            //  戻る時の速度
    private float timeCount;            //  タイムカウント
    private CameraController cc;        //  カメラコントローラー
    private Animator anim;
    
    //  操作感を上げるためのパラメータ
    private bool canBack;               //  一定距離を超えるから武器が戻れるフラグ(近距離で武器が壁に当たった瞬間に戻れないように)
    private Vector3 distanceCheckPos;   //  距離検査オブジェクトの初期位置
    private Vector3 distanceCheckRot;   //  距離検査オブジェクトの初期回転

    //  初期化
    void Awake()
    {
        rigid = gameObject.GetComponent<Rigidbody>();

        weaponCollider = gameObject.GetComponent<BoxCollider>();

        pi = playerPos.GetComponent<PlayerInput>();

        startPos[0] = transform.localPosition;

        startPos[1] = transform.localEulerAngles;

        anim = model.GetComponent<Animator>();

        cc = GameObject.Find("Main Camera").GetComponent<CameraController>();

        distanceCheckPos = distanceCheck.transform.localPosition;

        distanceCheckRot = distanceCheck.transform.localEulerAngles;
    }

    void Start()
    {
        rigid.constraints = RigidbodyConstraints.FreezeAll;
    }

    void FixedUpdate()
    {
        if (cc.canThrowWeapon && !pi.canThrow)
        {
            backToHand = false;
            timeCount += Time.fixedDeltaTime;
            distanceCheck.transform.SetParent(null);

            if (timeCount > 0.1f)
            {
                rigid.constraints = RigidbodyConstraints.None;

                transform.localEulerAngles += new Vector3(0, rotateSpeed, 0);
                transform.SetParent(null);

                weaponCollider.size = new Vector3(0.5f, weaponCollider.size.y, weaponCollider.size.z);

                if (speed > 0.0f)
                {
                    gameObject.transform.tag = "Weapon";

                    rigid.position += throwRot * speed * Time.fixedDeltaTime;     //  プレイヤーの正方向に一定距離を移動する

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
                transform.localEulerAngles = new Vector3(-0.053f, 81.762f, 0.687f);
                throwRot = model.transform.forward;
            }
        }
        //else
        //{
        //    transform.localPosition = startPos[0];                     //  親に相対の座標を設定する
        //    transform.localEulerAngles = startPos[1];                  //  親に相対の角度を設定する   
        //}

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
        if (collider.transform.tag == "Player" && speed <= 0.0f)
        {
            backToHand = true;
            pi.canThrow = true;
            cc.canThrowWeapon = true;
            distanceCheck.transform.tag = "GapCheck";
            //canBack = false;
            weaponCollider.size = new Vector3(1.65f, weaponCollider.size.y, weaponCollider.size.z);

            //  チェックオブジェクトの位置を戻す
            distanceCheck.transform.SetParent(model.transform);   
            distanceCheck.transform.localPosition = distanceCheckPos;
            distanceCheck.transform.localEulerAngles = distanceCheckRot;

            transform.parent = playerNeck.transform;
            transform.localPosition = startPos[0];                     //  親に相対の座標を設定する
            transform.localEulerAngles = startPos[1];                  //  親に相対の角度を設定する   
            speedDown = 0.0f;
            speed = 40.0f;
            timeCount = 0.0f;
            throwAnim.SetLayerWeight(throwAnim.GetLayerIndex("Attack"), 0.0f);
            rigid.constraints = RigidbodyConstraints.FreezeAll;
        }

        if (collider.gameObject.layer == LayerMask.NameToLayer("Wall") && gameObject.transform.tag == "Weapon")
        {
            speed = 0.0f;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        //if (collider.transform.tag == "GapCheck")
        //{
        //    canBack = true;
        //    distanceCheck.transform.tag = "Untagged";
        //}
    }
}
