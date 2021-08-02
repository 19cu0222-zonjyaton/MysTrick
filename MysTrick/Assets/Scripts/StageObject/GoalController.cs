﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalController : MonoBehaviour
{
    public bool gameClear;                      //  ゲームクリアフラグ
    public ParticleSystem ps;                   //  
    public GameObject clearMask;                //  
    public ActorController ac;                  //  
    public PlayerInput pi;
    public float perRadian;                     //  毎回変化の弧度
    public float rotateSpeed;                   //  回転スピード
    public float radius;
    public float radian;                        //  弧度
    public bool isTitleGoal;                    //  タイトル画面のゴールフラグ
    public PlayerInput pi;
    public static string clearStageName = "";
    public static int[] getCount = new int[4];

    private Vector3 oldPos;

    private AudioSource sound;
    private int[] tempGetCount = new int[4];
    private float timeCount = 10.0f;

    // 初期化
    void Awake()
    {
        if (!isTitleGoal)
        {
            oldPos = transform.position;
        }

        sound = gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isTitleGoal)
        {
            oldPos = transform.position;
        }

        if (!gameClear && Time.deltaTime != 0)
        {
            transform.Rotate(0, rotateSpeed, 0);

            radian += perRadian;                //  毎回弧度を0.01をプラスする
            float dy = Mathf.Cos(radian) * radius;
            transform.position = oldPos + new Vector3(0, dy, 0);

            if (radian >= 360.0f)
            {
                radian = 0.0f;
            }
        }
       
        //  ゲームクリア処理
        if (gameClear)
        {
            clearMask.GetComponent<CanvasGroup>().alpha = 1;

            //  コインの獲得率を更新する処理
            for (int i = 0; i < getCount.Length; i++)
            {
                if (StageSelectButtonController.selectStageName == "Stage0" + (i + 1))
                {
                    if (getCount[i] == 0)
                    {
                        getCount[i] = ac.coinCount;
                        tempGetCount[i] = getCount[i];
                    }
                    else if (ac.coinCount > tempGetCount[i])
                    {
                        getCount[i] = ac.coinCount;
                    }
                }
            }

            timeCount -= Time.deltaTime;
            if (timeCount >= 0.0f && timeCount < 3.0f)
            {
                clearMask.GetComponent<Animation>().Play();
            }
            else if (timeCount < 0.0f)
            {
                clearStageName = StageSelectButtonController.selectStageName;
                SceneManager.LoadScene("StageSelect");
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.tag == "Player")
        {
            transform.SetParent(collider.transform);

            gameClear = true;

            pi.inputEnabled = false;

            sound.Play();

            ps.Play();
        }
    }
}
