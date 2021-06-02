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

    public float playerRotation;    //  階段の正方向に向く

    public GameObject playerHandle;
    public GameObject playerModule;

    private Animator anim;
    private GameObject hintUI;
    private PlayerInput pi;
    private Rigidbody rigid;

    private int tempJumpCount;  //  階段数を保存するため
    private float tempJumpTime; //	ジャンプタイムを保存するため
    private float timeCount = 0.3f;

    // Start is called before the first frame update
    void Awake()
    {
        pi = GameObject.Find("PlayerHandle").GetComponent<PlayerInput>();

        rigid = GameObject.Find("PlayerHandle").GetComponent<Rigidbody>();

        anim = GameObject.Find("PlayerModule").GetComponent<Animator>();

        hintUI = transform.Find("hintUI").gameObject;

        playerModule = GameObject.Find("PlayerModule");

        tempJumpCount = jumpCount;

        tempJumpTime = jumpTime;
    }

    // Update is called once per frame
    void FixedUpdate()
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

                playerHandle.transform.localPosition = new Vector3((float)Math.Round((double)transform.position.x, 1), playerHandle.transform.position.y, (float)Math.Round((double)transform.position.z, 1));        //  ジャンプ始点を固定する
            }
        }

        if (isJumpStart)
        {
            hintUI.SetActive(false);
        }
    }

    private void jumpStart()
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
                    anim.SetTrigger("Jump");

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

                    pi.moveToTargetTime = 0.1f;

                    pi.lockJumpStatus = false;

                    pi.inputEnabled = true;

                    isJumpStart = false;
                }
            }
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.transform.tag == "Player")
        {
            hintUI.SetActive(true);

            if (pi.isTriggered && pi.isJumping && !pi.lockJumpStatus)
            {
                pi.Dup = 0.0f;          //	シングルを0にする
                pi.Dright = 0.0f;
                pi.Dmag = 0.0f;
                pi.moveToTargetTime = 0.0f;
                pi.inputEnabled = false;

                collider.transform.localPosition = new Vector3((float)Math.Round((double)transform.position.x, 1), collider.transform.position.y, (float)Math.Round((double)transform.position.z, 1));

                playerHandle = collider.transform.gameObject;

                playerModule.transform.rotation = Quaternion.Euler(0, playerRotation, 0);

                jumpCount = tempJumpCount;

                isJumpStart = true;

                isJump = true;

                pi.lockJumpStatus = true;
            }
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.transform.tag == "Player")
        {
            hintUI.SetActive(false);
        }
    }
}

