using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class TitleButtonController : MonoBehaviour
{
    public GameObject titleLogo;        //  タイトルロゴオブジェクト
    public GameObject maskPanel;
    public GameObject arrow;
    public float perRadian;             //  毎回変化の弧度
    public float radius;                //  半径
    public float radian;                //  弧度
    public Vector3 oldPos;              //  初期位置
    public bool gameStart;

    private Button btn;
    private float timeCount_GameStart;
    private float timeCount_Text;

    void Awake()
    {
        btn = gameObject.GetComponent<Button>();

        btn.onClick.AddListener(buttonListener);      //  監視メソッド
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == this.gameObject)
        {
            arrow.SetActive(true);
        }
        else
        {
            arrow.SetActive(false);
        }

        if (gameStart)
        {
            timeCount_GameStart += Time.deltaTime;
            timeCount_Text += Time.deltaTime;
        }

        //  ゲームスタート
        if (timeCount_GameStart >= 1.8f)
        {
            maskPanel.SetActive(true);

            if (!maskPanel.GetComponent<Animation>().isPlaying)
            {
                SceneManager.LoadScene("StageSelect");
            }
        }

        //  文字の点滅処理
        if (timeCount_Text >= 0.15f && timeCount_Text <= 0.3f)
        {
            gameObject.GetComponent<Image>().enabled = true;
        }
        else if (timeCount_Text > 0.3f)
        {
            gameObject.GetComponent<Image>().enabled = false;
            timeCount_Text = 0.0f;
        }
    }

    //  ボタンを監視メソッド
    public void buttonListener()
    {
        if ((gameObject.name == "Start" || gameObject.name == "Continue") && !gameStart)
        {
            titleLogo.transform.position = new Vector3(0, 35.0f, -30.0f);
            titleLogo.transform.eulerAngles = new Vector3(0, 180.0f, 0);
            titleLogo.GetComponent<Rigidbody>().useGravity = true;


            gameObject.GetComponent<Image>().enabled = false;

            if (gameObject.name == "Start")
            {
                for (int i = 0; i < GoalController.getCount.Length; i++)
                {
                    GoalController.getCount[i] = 0;
                }
                ActorInStageSelect.selectBtn = 1;
            }

            gameStart = true;
        }
        else if (gameObject.name == "Continue")
        { 
        
        }
        else
        {
            Application.Quit();
        }
    }
}
