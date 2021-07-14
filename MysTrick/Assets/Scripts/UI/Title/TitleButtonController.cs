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
    public Text buttonText;
    public float perRadian;             //  毎回変化の弧度
    public float radius;                //  半径
    public float radian;                //  弧度
    public Vector3 oldPos;              //  初期位置

    private Button btn;
    private float timeCount_GameStart;
    private float timeCount_Text;
    private bool gameStart;

    void Awake()
    {
        btn = gameObject.GetComponent<Button>();

        btn.onClick.AddListener(buttonListener);      //  監視メソッド
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == this.gameObject && !gameStart)
        {
            radian += perRadian;                //  弧度をプラスする
            float dy = Mathf.Cos(radian) * radius;
            transform.position = oldPos + new Vector3(0, dy, 0);

            if (radian >= 360.0f)
            {
                radian = 0.0f;
            }
        }
        else
        {
            transform.position = oldPos;
            radian = 0.0f;
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
        if (timeCount_Text >= 0.12f && timeCount_Text <= 0.24f)
        {
            buttonText.enabled = true;
        }
        else if (timeCount_Text > 0.24f)
        {
            buttonText.enabled = false;
            timeCount_Text = 0.0f;
        }
    }

    //  ボタンを監視メソッド
    public void buttonListener()
    {
        if (gameObject.name == "Start" && !gameStart)
        {
            titleLogo.transform.position = new Vector3(0, 35.0f, -30.0f);
            titleLogo.transform.eulerAngles = new Vector3(0, 180.0f, 0);
            titleLogo.GetComponent<Rigidbody>().useGravity = true;
            
            buttonText.enabled = false;

            gameStart = true;
        }
        else
        {
            Application.Quit();
        }
    }
}
