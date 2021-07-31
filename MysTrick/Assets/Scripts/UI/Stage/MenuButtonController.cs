using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuButtonController : MonoBehaviour
{
    public GameObject menuPanel;
    public AudioClip[] sounds;              //	SEオブジェクト
    private AudioSource au;                 //	SEのコンポーネント
    private Button btn;
    private Animator animator_Menu;
    private Animator animator_Mask;
    private StageMenuController smc;

    void Awake()
    {
        au = gameObject.GetComponent<AudioSource>();

        btn = gameObject.GetComponent<Button>();

        btn.onClick.AddListener(MenuListener);      //  監視メソッド

        animator_Menu = menuPanel.GetComponent<Animator>();

        animator_Mask = GameObject.Find("Mask").GetComponent<Animator>();

        smc = menuPanel.GetComponent<StageMenuController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (animator_Mask.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f && animator_Mask.GetCurrentAnimatorStateInfo(0).IsName("Mask_W_B"))      //  指定する動画が終わったらStageSelect画面に入る
        {
            SceneManager.LoadScene("StageSelect");
        }

        if (EventSystem.current.currentSelectedGameObject == this.gameObject && smc.isOpenMenu)
        {
            au.clip = sounds[0];
            au.Play();
        }
    }

    public void MenuListener()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            smc.animIsOver = false;
            smc.isOpenMenu = false;
            au.clip = sounds[1];
            au.Play();

            if (gameObject.name == "Continue")
            {
                animator_Menu.SetTrigger("Cancel");
                animator_Menu.SetBool("Menu", false);
            }
            else if (gameObject.name == "Back")
            {
                animator_Mask.SetTrigger("WhiteToBlack");
            }
        }
    }
}
