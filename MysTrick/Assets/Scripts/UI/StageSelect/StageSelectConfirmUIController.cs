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
    private Button btn;
    private AudioSource au;                 //	SEのコンポーネント
    private bool isCancel;
    private bool isOK;
    private float timeCount = 0.5f;         //  ステージに移動するまで暗いマスクの時間

    void Awake()
    {
        btn = gameObject.GetComponent<Button>();

        btn.onClick.AddListener(StageConfirm);      //  監視メソッド

        au = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == this.gameObject && !isOK)
        {
            au.Play();
        }

        //  メニューをキャンセルの処理
        if (isCancel)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && animator.GetCurrentAnimatorStateInfo(0).IsName("Stage_Confirm_Minus"))   
            {
                StageSelectButtonController.confirmMenuIsOpen = false;
                EventSystem.current.SetSelectedGameObject(GameObject.Find(StageSelectButtonController.selectStageName));       //  Stage01ボタンを選択状態にする
                isCancel = false;
                gameObject.transform.parent.gameObject.SetActive(false);
            }
        }
        print(isOK);
        //  ステージに移動処理
        if (isOK)       
        {
            timeCount -= 10 * Time.deltaTime;
            StageSelectButtonController.confirmMenuIsOpen = false;
            SceneManager.LoadScene(StageSelectButtonController.selectStageName);
            if (timeCount < -0.3f)
            {

            }        
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("cancel") && !isCancel)
        {
            //au.PlayOneShot(sounds[1]);
            animator.SetBool("Menu", false);
            isCancel = true;
        }
    }

    //  ボタンを監視メソッド
    public void StageConfirm()     
    {
        if (gameObject.name == "OK")
        {
            maskPanel.SetActive(true);
            isOK = true;
            au.clip = sounds[1];
            au.Play();
        }
        else
        {
            animator.SetBool("Menu", false);
            isCancel = true;
            au.PlayOneShot(sounds[1]);
        }
    }
}
