using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalController : MonoBehaviour
{
    public bool gameClear;
    public ParticleSystem ps;
    public GameObject clearMask;

    public float perRadian;         //  毎回変化の弧度
    public float radius;
    public float radian;               //  弧度
    public static string clearStageName = "";
    private Vector3 oldPos;
    private PlayerInput pi;
    private CanvasGroup canvasGroup;
    private float timeCount = 10.0f;
    private bool doOnce;

    // Start is called before the first frame update
    void Awake()
    {
        pi = GameObject.Find("PlayerHandle").GetComponent<PlayerInput>();

        oldPos = transform.position;        //  最初の位置 

        canvasGroup = GetComponent<CanvasGroup>();
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
       
        if (gameClear)
        {
            clearMask.GetComponent<CanvasGroup>().alpha = 1;

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

            ps.Play();
        }
    }
}
