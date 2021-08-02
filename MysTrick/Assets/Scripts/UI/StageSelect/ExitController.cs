using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ExitController : MonoBehaviour
{
    public GameObject exitPanel;
    public GameObject maskPanel;
    public AudioClip[] sounds;				//	SEオブジェクト
    public bool exitPanelIsOpen;

    private AudioSource au;                 //	SEのコンポーネント
    private Animator anim;
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
            if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("menu") && !StageSelectButtonController.confirmMenuIsOpen))
            {
                if (!exitPanelIsOpen)
                {
                    exitPanel.SetActive(true);
                    exitPanelIsOpen = true;
                    EventSystem.current.SetSelectedGameObject(GameObject.Find("Yes"));       //  Yesボタンを選択状態にする
                }
                else
                {
                    au.PlayOneShot(sounds[1]);
                    anim.SetBool("Menu", false);
                }
            }

            if (exitPanelIsOpen)
            {
                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && anim.GetCurrentAnimatorStateInfo(0).IsName("StageSelect_Exit_Minus"))
                {
                    exitPanel.SetActive(false);
                    exitPanelIsOpen = false;
                    EventSystem.current.SetSelectedGameObject(GameObject.Find(StageSelectButtonController.selectStageName));       //  前に選択したボタンを選択状態にする
                }
            }
        }
        else
        {
            if (returnTitle)
            {
                timeCount -= Time.deltaTime;
                if (timeCount < -0.3f)
                {
                    SceneManager.LoadScene("Title");
                }
            }
        }
    }

    public void StageExit()
    {
        if (gameObject.name == "Yes")
        {
            maskPanel.SetActive(true);
            returnTitle = true;
            au.clip = sounds[1];
            au.Play();
        }
        else
        {
            anim.SetBool("Menu", false);
            au.PlayOneShot(sounds[1]);
        }
    }
}
