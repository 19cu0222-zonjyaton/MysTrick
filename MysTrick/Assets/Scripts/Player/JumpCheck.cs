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
    //  毎回ジャンプのPower
    public float jumpPowerX;
    public float jumpPowerY;
    public float jumpPowerZ;

    public bool isJumpStart;        //  ジャンプスタートフラグ
    public bool isJump;             //  ジャンプ状態フラグ

    public float jumpTime;          //  ジャンプの間隔時間
    public int jumpCount;           //  ジャンプの回数

    public float playerRotation;    //  階段の正方向に向く

    public GameObject playerHandle; //  プレイヤーハンドルオブジェクト
    public GameObject playerModule; //  プレイヤーモデルオブジェクト
    public GameObject hintUI;       //  仕掛けのヒントUIオブジェクト

    private Animator anim;          //  プレイヤーアニメコントローラー
    private PlayerInput pi;         //  プレイヤーの入力コントローラー
    private ActorController ac;     //  プレイヤーの挙動コントローラー
    private Rigidbody rigid;        //  プレイヤーの鋼体コンポーネント
    private new AudioSource audio;  //  SEコンポーネント

    private int tempJumpCount;      //  階段数を保存するため
    private float tempJumpTime;     //	ジャンプタイムを保存するため
    private float timeCount = 0.3f; //  タイムカウント

    // 初期化
    void Awake()
    {
        pi = playerHandle.GetComponent<PlayerInput>();

        ac = playerHandle.GetComponent<ActorController>();

        rigid = playerHandle.GetComponent<Rigidbody>();

        anim = playerModule.GetComponent<Animator>();

        audio = gameObject.GetComponent<AudioSource>();

        tempJumpCount = jumpCount;

        tempJumpTime = jumpTime;
    }

    void FixedUpdate()
    {
        if (isJumpStart)
        {
            ac.isJumping = true;
            Physics.IgnoreLayerCollision(11, 13, true);
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

                    audio.Play();

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

                    Physics.IgnoreLayerCollision(11, 13, false);

                    ac.isJumping = false;

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
                pi.ResetSignal();

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

