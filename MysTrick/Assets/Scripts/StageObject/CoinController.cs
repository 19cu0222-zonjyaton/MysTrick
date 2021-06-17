using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    public float perRadian;         //  毎回変化の弧度
    public float radius;
    public float radian;            //  弧度
    private Vector3 startPos;
    private new Rigidbody rigidbody;
    private ActorController ac;
    private bool getByPlayer;       //  プレイヤーと当たったflag
    private float rotateSpeed = 2.0f;

    void Awake()
    {
        startPos = transform.position;        //  最初の位置    

        rigidbody = gameObject.GetComponent<Rigidbody>();

        ac = GameObject.Find("PlayerHandle").GetComponent<ActorController>();
    }

    void Update()
    {
        if (Time.deltaTime != 0)
        {
            transform.Rotate(0, rotateSpeed, 0);
            if (!getByPlayer)
            {
                radian += perRadian;                //  毎回弧度を0.01をプラスする
                float dy = Mathf.Cos(radian) * radius;
                transform.position = startPos + new Vector3(0, dy, 0);

                if (radian >= 360.0f)
                {
                    radian = 0.0f;
                }
            }
        }

        if (getByPlayer && transform.position.y < startPos.y - 0.7f)
        {
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.tag == "Player" && !getByPlayer)     //プレイヤーと当たる処理
        {
            getByPlayer = true;

            rigidbody.AddForce(0.0f, 500.0f, 0.0f);

            rigidbody.useGravity = true;

            ac.coinUIAction = true;

            ac.coinCount++;

            rotateSpeed = 12.0f;
        }
    }
}
