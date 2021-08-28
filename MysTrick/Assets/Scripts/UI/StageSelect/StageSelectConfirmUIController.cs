using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class StageSelectConfirmUIController : MonoBehaviour
{
    public GameObject maskPanel;
    public GameObject player;
    public Animator animator;
    public AudioClip[] sounds;				//	SEオブジェクト
    public Button btn;

    private AudioSource au;                 //	SEのコンポーネント
    private bool isOK;

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

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 2.0f && animator.GetCurrentAnimatorStateInfo(0).IsName("Stage_Confirm_Plus") && !StaticController.animIsOver)
        {
            btn.enabled = true;
            EventSystem.current.SetSelectedGameObject(GameObject.Find("OK"));       //  OKボタンを選択状態にする
            StaticController.animIsOver = true;
        }

        //  メニューをキャンセルの処理
        if (StaticController.isCancel)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && animator.GetCurrentAnimatorStateInfo(0).IsName("Stage_Confirm_Minus"))   
            {
                StaticController.confirmMenuIsOpen = false;
                EventSystem.current.SetSelectedGameObject(GameObject.Find(StaticController.selectStageName));       //  Stage01ボタンを選択状態にする
                StaticController.isCancel = false;
                StaticController.animIsOver = false;
                gameObject.transform.parent.gameObject.SetActive(false);
            }
        }

        //  ステージに移動処理
        if (isOK)       
        {
            timeCount -= Time.deltaTime;

            if (timeCount < -0.3f)
            {
                StaticController.playerPos = player.transform.position;
                StaticController.playerRot = player.transform.eulerAngles;
                StaticController.animIsOver = false;
                StaticController.confirmMenuIsOpen = false;
                SceneManager.LoadScene(StaticController.selectStageName);
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
            else if (EventSystem.current.currentSelectedGameObject.name == "Cancel" && gameObject.name == "Cancel" && !StaticController.isCancel)
            {
                animator.SetBool("Menu", false);
                StaticController.isCancel = true;
                au.PlayOneShot(sounds[1]);
                btn.enabled = false;
            }
        }

        if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("cancel")) && !StaticController.isCancel)
        {
            animator.SetBool("Menu", false);
            StaticController.isCancel = true;
            au.PlayOneShot(sounds[1]);
            btn.enabled = false;
        }
    }
}
