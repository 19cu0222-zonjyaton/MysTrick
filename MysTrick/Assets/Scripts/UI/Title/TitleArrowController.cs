using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleArrowController : MonoBehaviour
{
    public TitleButtonController tbc;
    public float perRadian;             //  毎回変化の弧度
    public float radius;                //  半径
    public float radian;                //  弧度
    public Vector3 startPos;              //  初期位置
    public bool leftArror;

    void Awake()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (tbc.gameStart)
        {
            transform.position = startPos;
        }
        else
        {
            radian += perRadian;                //  弧度をプラスする
            float dx = Mathf.Cos(radian) * radius;
            if (leftArror)
            {
                transform.position = startPos + new Vector3(dx, 0, 0);
            }
            else
            {
                transform.position = startPos - new Vector3(dx, 0, 0);
            }
        }

        if (radian >= 360.0f)
        {
            radian = 0.0f;
        }
    }
}
