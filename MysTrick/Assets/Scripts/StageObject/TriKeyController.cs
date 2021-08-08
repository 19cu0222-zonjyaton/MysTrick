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
    public bool uiAnimStart;

    private Vector3 nowPos;             //  初期位置
    private Vector3 startPos;
    private Rigidbody rigid;            //  鋼体コンポーネント
    private AudioSource sound;          //  SEコンポーネント
    private MeshRenderer mesh;
    private Animation anim;
    private bool getByPlayer;           //  プレイヤーと当たったflag
    private bool animStart;
    private bool animOver;

    //  初期化
    void Awake()
    {
        startPos = transform.position;

        rigid = gameObject.GetComponent<Rigidbody>();

        sound = gameObject.GetComponent<AudioSource>();

        mesh = gameObject.GetComponent<MeshRenderer>();

        anim = gameObject.GetComponent<Animation>();
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

            if (animStart && !animOver)
            {
                rotateSpeed -= 1.5f;

                if (rotateSpeed <= 0.0f)
                {
                    rigid.constraints = RigidbodyConstraints.FreezePosition;
                    transform.eulerAngles = finalRot;

                    anim.Play();
                    animOver = true;
                }
            }
            else if (animOver && !anim.isPlaying)
            {
                mesh.material.color -= new Color32(0, 0, 0, 10);
                uiAnimStart = true;
                if (mesh.material.color.a < 0.0f)
                {
                    Destroy(this.gameObject);
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

            if (transform.tag == "RedPiece" || transform.tag == "GreenPiece" || transform.tag == "BluePiece")
            {
                rigid.AddForce(0, 30.0f, 0);
            }

            rotateSpeed = 60.0f;

            animStart = true;
        }
    }
}
