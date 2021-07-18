using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButtonController : MonoBehaviour
{
    public GameObject menuPanel;
    private Button btn;
    private Animator animator_Menu;
    private Animator animator_Mask;
    private StageMenuController smc;
    private float timeCount;

    void Awake()
    {
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
    }

    public void MenuListener()
    {
        if (gameObject.name == "Continue")
        {
            Time.timeScale = 1;
            animator_Menu.SetTrigger("Cancel");
            animator_Menu.SetBool("Menu", false);
            smc.animIsOver = false;
            smc.isOpenMenu = false;
        }
        else
        {
            Time.timeScale = 1;
            smc.isOpenMenu = false;
            smc.animIsOver = false;
            animator_Mask.SetTrigger("WhiteToBlack");
        }
    }
}
