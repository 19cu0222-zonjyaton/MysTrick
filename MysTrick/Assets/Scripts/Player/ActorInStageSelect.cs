using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ActorInStageSelect : MonoBehaviour
{
    public GameObject[] target;           //  移動するターゲット
    public Button[] btn;                  //  選択しているボタンオブジェクト
    public Animator animator;             //  アニメコントローラーコンポーネント
    public ExitController ec;             //  タイトル画面に戻るUIコントローラー
    public GameObject stage02;
    public static int selectBtn = 1;      //  選択しているボタン標記
    public int skyboxIndex;               //  skyboxオブジェクト
    public bool isMove;                  //  移動しているかどうかフラグ
    private AudioSource au;               //	SEのコンポーネント
    private bool goLeft;                  //  左側に移動するフラグ
    private bool goRight;                 //  右側に移動するフラグ

    //	初期化
    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;                   //	FPSを60に固定する

        au = gameObject.GetComponent<AudioSource>();

        transform.position = StaticController.playerPos;
        transform.eulerAngles = StaticController.playerRot;

        EventSystem.current.SetSelectedGameObject(btn[selectBtn - 1].gameObject);
    }

    void Update()
    {
        //if (StaticController.clearStageName == "")        //  StageからStageSelectに飛びるではない場合
        //{
            if (!isMove && !StaticController.confirmMenuIsOpen && !StaticController.exitPanelIsOpen)
            {
                if (selectBtn < 4)
                {
                    if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetAxis("axisX") > 0)
                    {
                        if(selectBtn == 1)
                        {
                            goRight = true;
                            selectBtn++;
                            au.Play();
                        }
                        else if(Math.Round(stage02.gameObject.transform.position.x, 0) == -250)
                        {
                            goRight = true;
                            selectBtn++;
                            au.Play();
                        }
                    }
                }

                if (selectBtn > 1)
                {
                    if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetAxis("axisX") < 0)
                    {
                        goLeft = true;
                        selectBtn--;
                        au.Play();
                    }
                }

                StaticController.selectStageName = btn[selectBtn - 1].name;
            }
        //}
        //else                                            //  StageをクリアしてStageから飛びる場合
        //{
        //    if (selectBtn < 4)
        //    {
        //        transform.position = StaticController.playerPos;
        //        transform.eulerAngles = StaticController.playerRot;

        //        if (StaticController.stageIsFirstClear[selectBtn - 1])
        //        {
        //            goRight = true;
        //            selectBtn++;
        //            StaticController.stageIsFirstClear[selectBtn - 2] = false;
        //            StaticController.selectStageName = btn[selectBtn - 1].name;

        //            for (int i = 0; i < 4; i++)
        //            {
        //                StaticController.stageCanSelect[selectBtn - 1] = true;
        //            }
        //        }
        //        else if (!StaticController.stageIsFirstClear[selectBtn - 1])
        //        {
        //            StaticController.selectStageName = btn[selectBtn - 1].name;
        //        }

        //        StaticController.clearStageName = "";
        //    }
        //    else if(selectBtn == 4)
        //    {
        //        transform.position = StaticController.playerPos;
        //        transform.eulerAngles = StaticController.playerRot;
        //        StaticController.selectStageName = btn[selectBtn - 1].name;
        //        StaticController.clearStageName = "";
        //    }
        //}

        if (goLeft || goRight)
        {
            isMove = true;
            animator.SetFloat("Forward", 1.0f);
            EventSystem.current.SetSelectedGameObject(null);

            if (goLeft)     //  StageSelect画面のプレイヤー移動処理
            {
                if (selectBtn != 1)
                {
                    transform.position += new Vector3(-0.1f, 0.0f, 0.0f);
                }

                if (selectBtn == 1)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, target[0].transform.rotation, 5.0f * Time.deltaTime);
                }
                else if (selectBtn == 2)
                {
                     transform.rotation = Quaternion.Lerp(transform.rotation, target[4].transform.rotation, 5.0f * Time.deltaTime);
                }
                else if (selectBtn == 3)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, target[3].transform.rotation, 5.0f * Time.deltaTime);
                }
            }
            else if (goRight)
            {               
                if (selectBtn != 2)
                {
                    transform.position += new Vector3(0.1f, 0.0f, 0.0f);
                }

                if (selectBtn == 2)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, target[5].transform.rotation, 5.0f * Time.deltaTime);
                }
                else if (selectBtn == 3)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, target[1].transform.rotation, 5.0f * Time.deltaTime);
                }
                else if (selectBtn == 4)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, target[2].transform.rotation, 5.0f * Time.deltaTime);
                }
            }
        }

        if (!isMove)
        {
            skyboxIndex = selectBtn - 1;
        }
    }

    void OnTriggerEnter(Collider collider)      //  止まる判定
    {
        if (collider.transform.tag == "RightPoint" && goRight)
        {
            isMove = false;
            goRight = false;
            animator.SetFloat("Forward", 0.0f);

            for (int i = 0; i < btn.Length; i++)
            {
                btn[i].enabled = true;
            }

            EventSystem.current.SetSelectedGameObject(btn[selectBtn - 1].gameObject);
        }

        if (collider.transform.tag == "LeftPoint" && goLeft)
        {
            isMove = false;
            goLeft = false;
            animator.SetFloat("Forward", 0.0f);

            for (int i = 0; i < btn.Length; i++)
            {
                btn[i].enabled = true;
            }

            EventSystem.current.SetSelectedGameObject(btn[selectBtn - 1].gameObject);
        }
    }
}
