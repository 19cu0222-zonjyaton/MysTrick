using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TutorialButtonController1 : MonoBehaviour
{
    public GameObject menuPanel;
    public AudioClip[] sounds;              //	SEオブジェクト
    public Button btn;

    private AudioSource au;                 //	SEのコンポーネント
    private Animator animator_Menu;
    private Animator animator_Mask;
    private TutorialImageController smc;

    void Awake()
    {
        au = gameObject.GetComponent<AudioSource>();

        animator_Menu = menuPanel.GetComponent<Animator>();

        animator_Mask = GameObject.Find("Mask").GetComponent<Animator>();

        smc = menuPanel.GetComponent<TutorialImageController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (animator_Mask.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f && animator_Mask.GetCurrentAnimatorStateInfo(0).IsName("Mask_W_B"))      //  指定する動画が終わったらStageSelect画面に入る
        {
            TitleBGMController.tbc.GetComponent<AudioSource>().Play();
            SceneManager.LoadScene("StageSelect");
        }

        if (smc)
        {
            if (EventSystem.current.currentSelectedGameObject == this.gameObject && smc.isOpenMenu)
            {
                au.clip = sounds[0];
                au.Play();
            }
        }

        if (Time.timeScale == 0 && (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("action")))
        {
            smc.animIsOver = false;

            au.clip = sounds[1];
            au.Play();

            if (EventSystem.current.currentSelectedGameObject.name == "Close" && gameObject.name == "Close")
            {
                Time.timeScale = 1;
                //animator_Menu.SetTrigger("Cancel");
                animator_Menu.SetBool("Menu", false);
            }
            else if (EventSystem.current.currentSelectedGameObject.name == "Next" && gameObject.name == "Next")
            {
                // Time.timeScale = 1;
                // animator_Mask.SetTrigger("WhiteToBlack");
                // btn.enabled = false;
            }
        }
    }
}
