using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialImageController : MonoBehaviour
{
    public bool isOpenMenu;                 //   UIメニューは呼び出されたか
    public GameObject selectButton;         //   UIを出たらデフォルト選択するボタン
    public bool animIsOver = true;          //   UIのアニメ終了フラグ
    public AudioSource[] buttonAu;
    public Animator animator;
    private AudioSource au;                 //	SEのコンポーネント
    private ActorController ac;
    public bool doneFlag;
    public bool isOpening = false;

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
            if ( animIsOver)
            {
                if (isOpenMenu)
                {
                    if (isOpening == false)
                    {
                        isOpening = true;
                        for (int i = 0; i < buttonAu.Length; i++)
                        {
                            buttonAu[i].enabled = true;
                        } // end for()

                        EventSystem.current.SetSelectedGameObject(selectButton.gameObject);
                        animator.enabled = true;
                        animator.SetBool("Menu", true);
                    } // end if()
                } // end if()
                else 
                {
                    for (int i = 0; i < buttonAu.Length; i++)
                    {
                        buttonAu[i].enabled = false;
                    } // end for()

                    Time.timeScale = 1;
                    animIsOver = false;
                    //animator.SetTrigger("Cancel");
                    animator.enabled = false;
                } // end else()

                au.Play();
            } // end if()

            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && animIsOver)   //  normalizedTime == 0 -> スタート normalizedTime == 1 -> エンド 
            {
                Time.timeScale = 0;
                animIsOver = false;
            } // end if()

            if (isOpenMenu == true && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && animator.GetCurrentAnimatorStateInfo(0).IsName("Tutorial_Img_Minus"))
            {
                // doesnt  excute
                Time.timeScale = 1;
                animIsOver = true;
                isOpenMenu = false;
                doneFlag = true;
                isOpening = false;
            } // end if()
        }
        else
        {
            gameObject.SetActive(false);
        } // end else()
    }

}
