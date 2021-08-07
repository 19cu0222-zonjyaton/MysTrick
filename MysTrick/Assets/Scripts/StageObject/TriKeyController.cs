using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriKeyController : MonoBehaviour
{
    public float perRadian;             //  毎回変化の弧度
    public float radius;                //  半径
    public float radian;                //  弧度
    public float rotateSpeed;           //  回転スピード
    public Vector3 finalRot;

    private Vector3 nowPos;             //  初期位置
    private Vector3 startPos;
    private Rigidbody rigid;            //  鋼体コンポーネント
    private AudioSource sound;          //  SEコンポーネント
    private MeshRenderer mesh;
    private bool getByPlayer;           //  プレイヤーと当たったflag
    private bool scaleAddIsOver;
    private bool animStart;
    private float targetScale;

    //  初期化
    void Awake()
    {
        targetScale = 1.5f;

        startPos = transform.position;

        rigid = gameObject.GetComponent<Rigidbody>();

        sound = gameObject.GetComponent<AudioSource>();

        mesh = gameObject.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        nowPos = transform.position;

        if (Time.deltaTime != 0)
        {
            transform.Rotate(0, rotateSpeed, 0);
            if (!getByPlayer)
            {
                radian += perRadian;                //  弧度をプラスする
                float dy = Mathf.Cos(radian) * radius;
                transform.position = nowPos + new Vector3(0, dy, 0);

                if (radian >= 360.0f)
                {
                    radian = 0.0f;
                }
            }

            if (Mathf.Abs(nowPos.y - startPos.y) > 4.0f && getByPlayer)
            {
                rigid.useGravity = false;
                rigid.constraints = RigidbodyConstraints.FreezePosition;
                rotateSpeed = 0.0f;
                transform.eulerAngles = finalRot;

                animStart = true;
            }

            if (animStart)
            {
                if (transform.localScale.x <= targetScale && !scaleAddIsOver)
                {
                    transform.localScale += new Vector3(0.02f, 0.02f, 0.02f);
                }
                else if (transform.localScale.x > targetScale)
                {
                    scaleAddIsOver = true;
                    targetScale = 1.0f;
                    transform.localScale -= new Vector3(0.04f, 0.04f, 0.04f);
                }
                else
                {
                    mesh.material.color -= new Color32(0, 0, 0, 10);
                    if (mesh.material.color.a < 0.0f)
                    {
                        Destroy(this.gameObject);
                    }
                }
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.tag == "Player" && !getByPlayer)     //プレイヤーと当たる処理
        {
            getByPlayer = true;

            sound.Play();

            rigid.AddForce(0.0f, 800.0f, 0.0f);

            rigid.useGravity = true;

            rotateSpeed = 30.0f;
        }
    }
}
