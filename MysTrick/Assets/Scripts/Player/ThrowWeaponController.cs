using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowWeaponController : MonoBehaviour
{
    public float rotateSpeed;       //  回転速度
    public float speed;             //  スタートの移動速度
    
    private Rigidbody rigid;        //	鋼体コンポーネント
    private Animator anim;
    private GameObject playerPos;   //  プレイヤーの位置を獲得するため
    private PlayerInput pi;         //  攻撃ができるかどうかの判断
    private GameObject playerCamera;//  カメラオブジェクト
    private WeaponController wc;    //  武器のコントローラー
    private float speedDown;        //  戻る時の速度

    // 初期化
    void Awake()
    {
        rigid = gameObject.GetComponent<Rigidbody>();

        anim = GameObject.Find("PlayerModule").GetComponent<Animator>();

        playerPos = GameObject.Find("PlayerHandle");

        pi = GameObject.Find("PlayerHandle").GetComponent<PlayerInput>();

        playerCamera = GameObject.Find("Main Camera");

        wc = GameObject.Find("UsingWeapon").GetComponent<WeaponController>();
    }

    void FixedUpdate()
    {
        transform.Rotate(0.0f, rotateSpeed, 0.0f);
        wc.backToHand = false;
        anim.SetLayerWeight(anim.GetLayerIndex("Throw"), 1.0f);
        if (speed > 0.0f)
        {
            gameObject.transform.tag = "Weapon";

            rigid.position += playerCamera.transform.forward * speed * Time.fixedDeltaTime;     //  カメラの正方向に一定距離を移動する

            speed -= 1.5f;
        }
        else
        {
            gameObject.transform.tag = "Untagged";

            speedDown += 0.2f;

            transform.position = Vector3.Lerp(transform.position, playerPos.transform.position + new Vector3(0, 1.8f, 0), speedDown * Time.fixedDeltaTime);     //  プレイヤーの位置に戻る
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.tag == "Player")
        {
            pi.canThrow = true;

            wc.backToHand = true;

            playerCamera.GetComponent<CameraController>().canThrowWeapon = true;

            anim.SetLayerWeight(anim.GetLayerIndex("Throw"), 0.0f);

            Destroy(this.gameObject);
        }

        if (collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            speed = 0.0f;
        }
    }
}
