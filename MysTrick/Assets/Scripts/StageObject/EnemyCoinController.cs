using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCoinController : MonoBehaviour
{
    public float perRadian;             //  毎回変化の弧度
    public float radius;                //  半径
    public float radian;                //  弧度

    private Vector3 oldPos;             //  初期位置
    private Rigidbody rigid;            //  鋼体コンポーネント
    private ActorController ac;         //  プレイヤーの挙動コントローラー
    private bool getByPlayer;           //  プレイヤーと当たったflag
    private float rotateSpeed = 12.0f;  //  回転スピード

    //  初期化
    void Awake()
    {
        oldPos = transform.position; 

        rigid = gameObject.GetComponent<Rigidbody>();

        ac = GameObject.Find("PlayerHandle").GetComponent<ActorController>();

        rigid.AddForce(0.0f, 500.0f, 0.0f);
    }

    void Update()
    {
        transform.Rotate(0, rotateSpeed, 0);

        if (transform.position.y < oldPos.y - 0.7f)
        {
            Destroy(this.gameObject);
        }
    }
}
