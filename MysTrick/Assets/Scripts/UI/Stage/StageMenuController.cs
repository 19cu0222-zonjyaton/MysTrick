using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StageMenuController : MonoBehaviour
{
    public bool isOpenMenu;                //   UIメニューは呼び出されたか
    public GameObject selectButton;        //   UIを出たらデフォルト選択するボタン
    public bool animIsOver = true;        //   UIのアニメ終了フラグ
    private Animator animator;

    void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Menu")) && animIsOver)       
        {
            if (isOpenMenu)           //  もう一度ESC或いはXBoxのMenuボタンでゲーム画面に戻る
            {
                Time.timeScale = 1;
                animIsOver = false;
                animator.SetTrigger("Cancel");
                animator.SetBool("Menu", false);
                isOpenMenu = false;            
            }
            else　                 //   ESC或いはXBoxのMenuボタンで呼び出せる
            {
                EventSystem.current.SetSelectedGameObject(selectButton);
                animator.enabled = true;
                animator.SetBool("Menu", true);
                isOpenMenu = true;
            }
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
}
