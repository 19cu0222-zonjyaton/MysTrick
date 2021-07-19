using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    public float perRadian;             //  毎回変化の弧度
    public float radius;                //  半径
    public float radian;                //  弧度
    public float rotateSpeed;           //  回転スピード
    public bool isTitleCoin;            //  タイトル画面のコインフラグ

    private Vector3 oldPos;             //  初期位置
    private Rigidbody rigid;            //  鋼体コンポーネント
    private ActorController ac;         //  プレイヤーのコントローラー
    private AudioSource sound;          //  SEコンポーネント
    private bool getByPlayer;           //  プレイヤーと当たったflag

    //  初期化
    void Awake()
    {
        if (!isTitleCoin)
        {
            oldPos = transform.position;

            ac = GameObject.Find("PlayerHandle").GetComponent<ActorController>();
        }

        rigid = gameObject.GetComponent<Rigidbody>();

        sound = gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isTitleCoin)
        {
            oldPos = transform.position;
        }

        if (Time.deltaTime != 0)
        {
            transform.Rotate(0, rotateSpeed, 0);
            if (!getByPlayer)
            {
                radian += perRadian;                //  弧度をプラスする
                float dy = Mathf.Cos(radian) * radius;
                transform.position = oldPos + new Vector3(0, dy, 0);

                if (radian >= 360.0f)
                {
                    radian = 0.0f;
                }
            }
        }

        if (getByPlayer && transform.position.y < oldPos.y - 0.7f)
        {
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.tag == "Player" && !getByPlayer)     //プレイヤーと当たる処理
        {
            getByPlayer = true;

            sound.Play();

            rigid.AddForce(0.0f, 500.0f, 0.0f);

            rigid.useGravity = true;

            ac.coinUIAction = true;

            ac.coinCount++;

            rotateSpeed = 12.0f;
        }
    }
}
