using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class StageSelectConfirmUIController : MonoBehaviour
{
    public GameObject maskPanel;
    public Animator animator;
    public AudioClip[] sounds;				//	SEオブジェクト
    public Button btn;
    public static bool animIsOver;

    private AudioSource au;                 //	SEのコンポーネント
    private bool isOK;
    private static bool isCancel;
    private float timeCount = 0.5f;         //  ステージに移動するまで暗いマスクの時間

    void Awake()
    {
        au = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == this.gameObject && !isOK)
        {
            au.Play();
        }

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 2.0f && animator.GetCurrentAnimatorStateInfo(0).IsName("Stage_Confirm_Plus") && !animIsOver)
        {
            btn.enabled = true;
            EventSystem.current.SetSelectedGameObject(GameObject.Find("OK"));       //  OKボタンを選択状態にする
            animIsOver = true;
        }

        //  メニューをキャンセルの処理
        if (isCancel)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && animator.GetCurrentAnimatorStateInfo(0).IsName("Stage_Confirm_Minus"))   
            {
                StageSelectButtonController.confirmMenuIsOpen = false;
                EventSystem.current.SetSelectedGameObject(GameObject.Find(StageSelectButtonController.selectStageName));       //  Stage01ボタンを選択状態にする
                isCancel = false;
                animIsOver = false;
                gameObject.transform.parent.gameObject.SetActive(false);
            }
        }

        //  ステージに移動処理
        if (isOK)       
        {
            timeCount -= Time.deltaTime;

            if (timeCount < -0.3f)
            {
                animIsOver = false;
                StageSelectButtonController.confirmMenuIsOpen = false;
                SceneManager.LoadScene(StageSelectButtonController.selectStageName);
            }        
        }

        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("action")))
        {
            if (EventSystem.current.currentSelectedGameObject.name == "OK" && gameObject.name == "OK" && !isOK)
            {
                maskPanel.SetActive(true);
                isOK = true;
                au.clip = sounds[1];
                au.Play();
                btn.enabled = false;
            }
            else if (EventSystem.current.currentSelectedGameObject.name == "Cancel" && gameObject.name == "Cancel" && !isCancel)
            {
                animator.SetBool("Menu", false);
                isCancel = true;
                au.PlayOneShot(sounds[1]);
                btn.enabled = false;
            }
        }

        if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("cancel")) && !isCancel)
        {
            animator.SetBool("Menu", false);
            isCancel = true;
            au.PlayOneShot(sounds[1]);
            btn.enabled = false;
        }
    }
}
