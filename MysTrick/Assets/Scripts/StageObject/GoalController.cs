using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalController : MonoBehaviour
{
    public bool gameClear;                      //  ゲームクリアフラグ
    public ParticleSystem ps;                   //  
    public GameObject clearMask;                //  
    public ActorController ac;                  //  
    public PlayerInput pi;
    public float perRadian;                     //  毎回変化の弧度
    public float rotateSpeed;                   //  回転スピード
    public float radius;
    public float radian;                        //  弧度
    public bool isTitleGoal;                    //  タイトル画面のゴールフラグ
    public AudioClip sound;				        //	SEオブジェクト

    private Vector3 oldPos;

    private AudioSource au;
    private int[] tempGetCount = new int[4];
    private float timeCount = 10.0f;

    private CameraFade fadeControl;

    // 初期化
    void Awake()
    {
        if (!isTitleGoal)
        {
            oldPos = transform.position;
        }

        au = gameObject.GetComponent<AudioSource>();

        fadeControl = GameObject.Find("Main Camera").GetComponent<CameraFade>();
        
    }

    void Update()
    {
        if (isTitleGoal)
        {
            oldPos = transform.position;
        }

        if (!gameClear && Time.deltaTime != 0)
        {
            transform.Rotate(0, rotateSpeed, 0);

            radian += perRadian;                //  毎回弧度を0.01をプラスする
            float dy = Mathf.Cos(radian) * radius;
            transform.position = oldPos + new Vector3(0, dy, 0);

            if (radian >= 360.0f)
            {
                radian = 0.0f;
            }
        }
       
        //  ゲームクリア処理
        if (gameClear)
        {
            //  コインの獲得率を更新する処理
            for (int i = 0; i < 4; i++)
            {
                if (StaticController.selectStageName == "Stage0" + (i + 1))
                {
                    StaticController.stageIsClear[i] = true;
                    if (ac.starCount > StaticController.highScore[i])
                    {
                        StaticController.highScore[i] = ac.starCount;
                    }

                    //if (getCount[i] == 0)
                    //{
                    //    getCount[i] = ac.coinCount;
                    //    tempGetCount[i] = getCount[i];
                    //}
                    //else if (ac.coinCount > tempGetCount[i])
                    //{
                    //    getCount[i] = ac.coinCount;
                    //}
                }
            }

            timeCount -= Time.deltaTime;
            if (timeCount >= 1.0f && timeCount < 3.0f)
            {
                //clearMask.GetComponent<Animation>().Play();
                fadeControl.isFading = true;
            }
            else if ( timeCount <1.0f && timeCount >= 0.0f)
                clearMask.GetComponent<CanvasGroup>().alpha = 1;
            else if (timeCount < 0.0f)
            {
                if (TitleBGMController.tbc)
                    TitleBGMController.tbc.GetComponent<AudioSource>().Play();

                StaticController.clearStageName = StaticController.selectStageName;
                SceneManager.LoadScene("StageSelect");
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.tag == "Player")
        {
            transform.SetParent(collider.transform);

            gameClear = true;

            pi.ResetSignal();

            au.loop = false;
            au.clip = sound;
            au.Play();

            ps.Play();
        }
    }
}
