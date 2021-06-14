using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class StageSelectConfirmUIController : MonoBehaviour
{
    public Animator animator;
    private Button btn;
    private bool isCancel;
    private float timeCount = 0.5f;

    void Awake()
    {
        btn = gameObject.GetComponent<Button>();

        btn.onClick.AddListener(StageConfirm);      //  監視メソッド
    }

    // Update is called once per frame
    void Update()
    {
        if (isCancel)
        {
            timeCount -= Time.deltaTime;
            if (timeCount < 0.0f)
            {
                gameObject.transform.parent.gameObject.SetActive(false);
                EventSystem.current.SetSelectedGameObject(GameObject.Find("Stage01"));       //  Stage01ボタンを選択状態にする
                isCancel = false;
                timeCount = 0.5f;
            }
        }
    }

    void StageConfirm()
    {
        if (gameObject.name == "OK")
        {
            SceneManager.LoadScene(StageSelectButtonController.selectStageName);
        }
        else
        {
            animator.SetTrigger("Cancel");
            isCancel = true;
        }
    }
}
