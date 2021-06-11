using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowWeaponController : MonoBehaviour
{
    public float rotateSpeed;       //  回転速度
    public float speed;             //  スタートの移動速度
    
    private new Rigidbody rigidbody;
    private GameObject playerPos;   //  プレイヤーの位置を獲得するため
    private PlayerInput pi;         //  攻撃ができるかどうかの判断
    private GameObject playerCamera;
    private float speedDown;        //  戻る時の速度

    // Start is called before the first frame update
    void Awake()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();

        playerPos = GameObject.Find("PlayerHandle");

        pi = GameObject.Find("PlayerHandle").GetComponent<PlayerInput>();

        playerCamera = GameObject.Find("Main Camera");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(0.0f, rotateSpeed, 0.0f);
        if (speed > 0.0f)
        {
            gameObject.transform.tag = "Weapon";

            rigidbody.position += playerCamera.transform.forward * speed * Time.fixedDeltaTime;     //  カメラの正方向に一定距離を移動する

            speed -= 1.5f;
        }
        else
        {
            gameObject.transform.tag = "Untagged";

            speedDown += 0.004f;

            transform.position = Vector3.Lerp(transform.position, playerPos.transform.position, speedDown);     //  プレイヤーの位置に戻る
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.tag == "Player")
        {
            pi.canThrow = true;

            playerCamera.GetComponent<CameraController>().canThrowWeapon = true;

            Destroy(this.gameObject);
        }
    }
}
