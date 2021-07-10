using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalController : MonoBehaviour
{
    public bool gameClear;
    public ParticleSystem ps;
    public GameObject clearMask;
    public ActorController ac;
    public float perRadian;         //  毎回変化の弧度
    public float radius;
    public float radian;               //  弧度
    public static string clearStageName = "";
    public static int[] getCount = new int[4];
    private Vector3 oldPos;
    private PlayerInput pi;
    private AudioSource audio;
    private int[] tempGetCount = new int[4];
    private float timeCount = 10.0f;

    // Start is called before the first frame update
    void Awake()
    {
        pi = GameObject.Find("PlayerHandle").GetComponent<PlayerInput>();

        oldPos = transform.position;        //  最初の位置 

        audio = gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!gameClear && Time.deltaTime != 0)
        {
            transform.Rotate(0, 2.0f, 0);

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
            clearMask.GetComponent<CanvasGroup>().alpha = 1;

            //  コインの獲得率を更新する処理
            for (int i = 0; i < getCount.Length; i++)
            {
                if (StageSelectButtonController.selectStageName == "Stage0" + (i + 1))
                {
                    if (getCount[i] == 0)
                    {
                        getCount[i] = ac.coinCount;
                        tempGetCount[i] = getCount[i];
                    }
                    else if (ac.coinCount > tempGetCount[i])
                    {
                        getCount[i] = ac.coinCount;
                    }
                }
            }

            timeCount -= Time.deltaTime;
            if (timeCount >= 0.0f && timeCount < 3.0f)
            {
                clearMask.GetComponent<Animation>().Play();
            }
            else if (timeCount < 0.0f)
            {
                clearStageName = StageSelectButtonController.selectStageName;
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

            pi.inputEnabled = false;

            audio.Play();

            ps.Play();
        }
    }
}
