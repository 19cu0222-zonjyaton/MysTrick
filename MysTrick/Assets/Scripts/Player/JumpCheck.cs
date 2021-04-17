//-------------------------------------------------
// ファイル名	：JumpCheck.cs
// 概要			：
// 作成者		：
// 更新内容		：
//-------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpCheck : MonoBehaviour
{
    public float jumpPowerX;
    public float jumpPowerY;
    public float jumpPowerZ;

    public bool isJumpStart;
    public bool isJump;

    public float jumpTime;
    public int jumpCount;

    private int tempJumpCount;  //  階段数を保存するため
    private float tempJumpTime; //	ジャンプタイムを保存するため
    private PlayerInput pi;
    private Rigidbody rigid;
    public GameObject playerModel;
    private float timeCount = 0.3f;

    // Start is called before the first frame update
    void Awake()
    {
        pi = GameObject.Find("PlayerHandle").GetComponent<PlayerInput>();

        rigid = GameObject.Find("PlayerHandle").GetComponent<Rigidbody>();

        tempJumpCount = jumpCount;

        tempJumpTime = jumpTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (isJumpStart)
        {            
            if (timeCount < 0.0f)   //  データ精度のためにディレイする
            {
                jumpStart();
            }
            else
            {
                timeCount -= Time.deltaTime;

                playerModel.transform.localPosition = new Vector3((float)Math.Round((double)transform.position.x, 1), playerModel.transform.position.y, (float)Math.Round((double)transform.position.z, 1));        //  ジャンプ始点を固定する
            }
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.transform.tag == "Player" && pi.isTriggered && pi.isJumping && !pi.lockJumpStatus)
        {
            pi.Dup = 0.0f;          //	シングルを0にする
            pi.Dright = 0.0f;
            pi.Dmag = 0.0f;
            pi.moveSpeed = 0.0f;
            pi.inputEnabled = false;

            collider.transform.localPosition = new Vector3((float)Math.Round((double)transform.position.x, 1), collider.transform.position.y, (float)Math.Round((double)transform.position.z, 1));

            playerModel = collider.transform.gameObject;

            //collider.transform.rotation = Quaternion.Euler(0, 0, 0);	//	プレイヤーモデルの方向を正す

            jumpCount = tempJumpCount;

            isJumpStart = true;

            isJump = true;

            pi.lockJumpStatus = true;
        }
    }

    void jumpStart()
    {
        if (jumpCount > 0)
        {
            if (isJumpStart)                //  ジャンプをできるか
            {
                pi.Dup = 0.0f;          //	シングルを0にする
                pi.Dright = 0.0f;
                pi.Dmag = 0.0f;

                jumpTime -= Time.deltaTime;

                if (isJump)
                {
                    rigid.AddForce(jumpPowerX, jumpPowerY, jumpPowerZ);

                    isJump = false;
                }
            }
            if (jumpTime <= 0.0f)   //  何秒ごとにジャンプ一回をやるか
            {
                isJump = true;

                jumpTime = tempJumpTime;

                jumpCount--;    //  階段の段数

                if (jumpCount == 0)
                {
                    timeCount = 0.3f;

                    pi.moveSpeed = 0.1f;

                    pi.lockJumpStatus = false;

                    pi.inputEnabled = true;

                    isJumpStart = false;
                }
            }
        }
    }
}

