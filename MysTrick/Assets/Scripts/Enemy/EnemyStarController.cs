using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStarController : MonoBehaviour
{
    //public float perRadian;             //  毎回変化の弧度
    //public float radius;                //  半径
    //public float radian;                //  弧度
    public float rotateSpeed;           //  回転スピード

    private Vector3 nowPos;             //  今の位置
    private AudioSource sound;          //  SEコンポーネント

    //  初期化
    void Awake()
    {
        sound = gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        nowPos = transform.position;

        if (Time.deltaTime != 0)
        {
            transform.Rotate(0, rotateSpeed, 0);
            //radian += perRadian;                //  弧度をプラスする
            //float dy = Mathf.Cos(radian) * radius;
            //transform.position = nowPos + new Vector3(0, dy, 0);

            //if (radian >= 360.0f)
            //{
            //    radian = 0.0f;
            //}
        }
    }
}
