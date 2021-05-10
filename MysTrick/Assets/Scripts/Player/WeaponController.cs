using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public float rotateSpeed;       //  回転速度
    public float speed;             //  スタートの移動速度

    private Rigidbody rigidbody;
    private GameObject playerPos;   //  プレイヤーの位置を獲得するため
    private GameObject model;       //  プレイヤーの回転方向を獲得するため
    private Vector3 moveVec;        //  武器の回転方向
    private float speedDown;        //  戻る時の速度

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();

        playerPos = GameObject.Find("PlayerHandle");

        model = GameObject.Find("PlayerModule");

        moveVec = model.transform.forward;
    }

    void Update()
    {
        transform.Rotate(0.0f, rotateSpeed, 0.0f);

        speed -= 0.4f;

        if (speed > 0.0f)
        {
            rigidbody.position += moveVec * speed * Time.deltaTime;     //  プレイヤーの正方向に一定距離を移動する
        }
        else
        {
            speedDown += 0.001f;

            transform.position = Vector3.Lerp(transform.position, playerPos.transform.position + new Vector3(0.0f, 1.0f, 0.0f), speedDown);     //  プレイヤーの位置に戻る
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.tag == "Player")
        {
            Destroy(this.gameObject);
        }
    }
}
