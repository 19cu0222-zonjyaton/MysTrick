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
                StageSelectButtonController.outputConfirmUI = false;
                EventSystem.current.SetSelectedGameObject(GameObject.Find(StageSelectButtonController.selectStageName));       //  Stage01ボタンを選択状態にする
                isCancel = false;
                timeCount = 0.5f;
                gameObject.transform.parent.gameObject.SetActive(false);
            }
        }

        if (isOK)
        {
            timeCount -= Time.deltaTime;
            if (timeCount < -0.3f)
            {
                StageSelectButtonController.outputConfirmUI = false;
                SceneManager.LoadScene(StageSelectButtonController.selectStageName);
            }        
        }
    }

    void StageConfirm()
    {
        if (gameObject.name == "OK")
        {
            maskPanel.SetActive(true);
            isOK = true;
        }
        else
        {
            animator.SetTrigger("Cancel");
            isCancel = true;
        }
    }
}
