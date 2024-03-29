﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StageMenuController : MonoBehaviour
{
    public bool isOpenMenu;                 //   UIメニューは呼び出されたか
    public GameObject selectButton;         //   UIを出たらデフォルト選択するボタン
    public bool animIsOver = true;          //   UIのアニメ終了フラグ
    public AudioSource[] buttonAu;
    private Animator animator;
    private AudioSource au;                 //	SEのコンポーネント
    private ActorController ac;

    public GameObject btn1;
    public GameObject btn2;

    void Awake()
    {
        animator = gameObject.GetComponent<Animator>();

        au = gameObject.GetComponent<AudioSource>();

        ac = GameObject.Find("PlayerHandle").GetComponent<ActorController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!ac.isDead && !ac.isFallDead)
        {
            if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("menu")) && animIsOver)
            {
                if (isOpenMenu)           //  もう一度ESC或いはXBoxのMenuボタンでゲーム画面に戻る
                {
                    if (btn1 && btn2)
                    {
                        btn1.SetActive(false);
                        btn2.SetActive(false);
                    } // end if

                    for (int i = 0; i < buttonAu.Length; i++)
                    {
                        buttonAu[i].enabled = false;
                    }

                    Time.timeScale = 1;
                    animIsOver = false;
                    //animator.SetTrigger("Cancel");
                    animator.SetBool("Menu", false);
                    isOpenMenu = false;
                }
                else                  //   ESC或いはXBoxのMenuボタンで呼び出せる
                {
                    if (btn1 && btn2)
                    {
                        btn1.SetActive(true);
                        btn2.SetActive(true);
                    } // end if

                    for (int i = 0; i < buttonAu.Length; i++)
                    {
                        buttonAu[i].enabled = true;
                    }

                    EventSystem.current.SetSelectedGameObject(selectButton);
                    animator.enabled = true;
                    animator.SetBool("Menu", true);
                    isOpenMenu = true;
                }

                au.Play();
            }

            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && animIsOver)   //  normalizedTime == 0 -> スタート normalizedTime == 1 -> エンド 
            {
                Time.timeScale = 0;
            }

            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && animator.GetCurrentAnimatorStateInfo(0).IsName("Stage_Menu_Minus"))
            {
                Time.timeScale = 1;
                animIsOver = true;
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
