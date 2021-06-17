using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    public float perRadian;         //  毎回変化の弧度
    public float radius;
    public float radian;            //  弧度
    private Vector3 oldPos;
    private new Rigidbody rigidbody;
    private ActorController ac;
    private bool getByPlayer;       //  プレイヤーと当たったflag

    void Awake()
    {
        oldPos = transform.position;        //  最初の位置    

        rigidbody = gameObject.GetComponent<Rigidbody>();

        ac = GameObject.Find("PlayerHandle").GetComponent<ActorController>();
    }

    void Update()
    {
        transform.Rotate(0, 2.0f, 0);
        if (!getByPlayer)
        {
            radian += perRadian;                //  毎回弧度を0.01をプラスする
            float dy = Mathf.Cos(radian) * radius;
            transform.position = oldPos + new Vector3(0, dy, 0);

            if (radian >= 360.0f)
            {
                radian = 0.0f;
            }
        }

        if (getByPlayer && transform.position.y < oldPos.y - 1.0f)
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
        }
    }
}
