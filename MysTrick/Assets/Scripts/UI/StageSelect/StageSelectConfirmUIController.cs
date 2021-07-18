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
    private Button btn;
    private bool isCancel;
    private bool isOK;
    private float timeCount = 0.5f;     //  ステージに移動するまで暗いマスクの時間

    void Awake()
    {
        btn = gameObject.GetComponent<Button>();

        btn.onClick.AddListener(StageConfirm);      //  監視メソッド
    }

    // Update is called once per frame
    void Update()
    {
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

        //  ステージに移動処理
        if (isOK)       
        {
            timeCount -= Time.deltaTime;
            if (timeCount < -0.3f)
            {
                StageSelectButtonController.confirmMenuIsOpen = false;
                SceneManager.LoadScene(StageSelectButtonController.selectStageName);
            }        
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
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
        }
        else
        {
            animator.SetBool("Menu", false);
            isCancel = true;
        }
    }
}
