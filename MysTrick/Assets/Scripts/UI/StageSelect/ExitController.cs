using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExitController : MonoBehaviour
{
    public GameObject exitPanel;
    public GameObject maskPanel;
    public AudioClip[] sounds;              //	SEオブジェクト
    public Button[] btn;
    public static bool exitPanelIsOpen;
    public static bool animIsStart;

    private AudioSource au;                 //	SEのコンポーネント
    private Animator anim;
    private bool enableInput;
    private bool returnTitle;
    private float timeCount = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        anim = exitPanel.GetComponent<Animator>();

        au = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == this.gameObject && !returnTitle)
        {
            au.Play();
        }

        if (gameObject.name == "Canvas")
        {
            if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("menu")) && !StageSelectButtonController.confirmMenuIsOpen)
            {
                if (!exitPanelIsOpen)
                {
                    btn[0].enabled = true;
                    btn[1].enabled = true;
                    exitPanel.SetActive(true);
                    exitPanelIsOpen = true;
                }
                else
                {
                    if (!animIsStart)
                    {
                        au.PlayOneShot(sounds[1]);
                    }
                    animIsStart = true;
                    anim.SetBool("Menu", false);
                }
            }

            if (exitPanelIsOpen)
            {
                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 2.0f && anim.GetCurrentAnimatorStateInfo(0).IsName("StageSelect_Exit_Plus") && !enableInput)
                {
                    EventSystem.current.SetSelectedGameObject(GameObject.Find("Yes"));       //  Yesボタンを選択状態にする
                    enableInput = true;
                }
                else if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && anim.GetCurrentAnimatorStateInfo(0).IsName("StageSelect_Exit_Minus"))
                {
                    exitPanelIsOpen = false;
                    animIsStart = false;
                    enableInput = false;
                    exitPanel.SetActive(false);
                    EventSystem.current.SetSelectedGameObject(GameObject.Find(StageSelectButtonController.selectStageName));       //  前に選択したボタンを選択状態にする
                }
            }
        }
        else
        {
            if ((Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("action")))
            {

                if (EventSystem.current.currentSelectedGameObject.name == "Yes" && gameObject.name == "Yes" && !returnTitle)
                {
                    maskPanel.SetActive(true);
                    returnTitle = true;
                    au.clip = sounds[1];
                    au.Play();
                    btn[0].enabled = false;
                }
                else if(EventSystem.current.currentSelectedGameObject.name == "No" && gameObject.name == "No" && !animIsStart)
                {
                    anim.SetBool("Menu", false);
                    au.PlayOneShot(sounds[1]);
                    animIsStart = true;
                    btn[0].enabled = false;
                }
            }

            if (returnTitle)
            {
                timeCount -= Time.deltaTime;
                if (timeCount < -0.3f)
                {
                    exitPanelIsOpen = false;
                    SceneManager.LoadScene("Title");
                }
            }
        }
    }
}
