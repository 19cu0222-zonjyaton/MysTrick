using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StageSelectButtonController : MonoBehaviour
{
    public bool canSelected;                    //  クリアしたかどうか
    public GameObject confirmPanel;
    public static string selectStageName;       //  選択したステージ名(シン―を切り替えてもstatic dataに影響しない)
    public static bool outputConfirmUI;
    private Button btn;
    private Image img;
    private Vector3 startPos;                   //  始点の位置

    void Awake()
    {
        img = gameObject.GetComponent<Image>();

        btn = gameObject.GetComponent<Button>();

        btn.onClick.AddListener(StageSelect);   //  監視メソッド

        startPos = transform.parent.position;

        Application.targetFrameRate = 60;       //	FPSを60に固定する
    }

    // Update is called once per frame
    void Update()
    {
        if (!canSelected)       //  画像の色
        {
            img.color = Color.grey;
        }
        else
        {
            img.color = Color.white;
        }

        if (EventSystem.current.currentSelectedGameObject.name == "Stage04")        //  button位置の移動処理
        {
            gameObject.transform.parent.position = Vector3.MoveTowards(transform.parent.position, startPos - new Vector3(300.0f, 0.0f, 0.0f), 150.0f * Time.deltaTime);
        }
        else if (EventSystem.current.currentSelectedGameObject.name == "Stage03")
        {
            gameObject.transform.parent.position = Vector3.MoveTowards(transform.parent.position, startPos, 150.0f * Time.deltaTime);
        }
    }

    public void StageSelect()
    {
        if (canSelected)        //  確認画面に入る前の処理
        {
            outputConfirmUI = true;
            confirmPanel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(GameObject.Find("OK"));       //  OKボタンを選択状態にする

            selectStageName = gameObject.name;
        }
    }
}
