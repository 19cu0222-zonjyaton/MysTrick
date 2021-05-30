using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public float rotateSpeed;       //  回転速度
    public float speed;             //  スタートの移動速度

    private new Rigidbody rigidbody;
    private GameObject playerPos;   //  プレイヤーの位置を獲得するため
    private PlayerInput pi;         //  攻撃ができるかどうかの判断
    private GameObject model;       //  プレイヤーの回転方向を獲得するため
    private GameObject playerCamera;
    private Vector3 moveVec;        //  武器の回転方向
    private float speedDown;        //  戻る時の速度

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();

        playerPos = GameObject.Find("PlayerHandle");

        model = GameObject.Find("PlayerModule");

        pi = GameObject.Find("PlayerHandle").GetComponent<PlayerInput>();

        playerCamera = GameObject.Find("Main Camera");

        moveVec = model.transform.forward;
    }

    void FixedUpdate()
    {
        transform.Rotate(0.0f, rotateSpeed, 0.0f);

        speed -= 1.5f;

        if (!pi.isAimStatus)
        {
            if (speed > 0.0f)
            {
                rigidbody.position += moveVec * speed * Time.fixedDeltaTime;     //  プレイヤーの正方向に一定距離を移動する
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

    void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.tag == "Player")
        {
            pi.canAttack = true;

            Destroy(this.gameObject);
        }
    }
}
