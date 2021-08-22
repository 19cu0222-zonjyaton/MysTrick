using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameovereMenuButtonController : MonoBehaviour
{
    public AudioClip[] sounds;              //	SEオブジェクト
    public Button btn;

    private CameraController cc;
    private AudioSource au;                 //	SEのコンポーネント
    private Animator animator_Mask;
    private string loadSceneName;

    void Awake()
    {
        cc = GameObject.Find("Main Camera").GetComponent<CameraController>();

        au = gameObject.GetComponent<AudioSource>();

        animator_Mask = GameObject.Find("Mask").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameObject.transform.parent.GetComponent<Animation>().isPlaying && cc.deadMoveStart)
        {
            if (animator_Mask.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f && animator_Mask.GetCurrentAnimatorStateInfo(0).IsName("Mask_W_B"))      //  指定する動画が終わったらStageSelect画面に入る
            {
                if (loadSceneName == StaticController.selectStageName)
                {
                    SceneManager.LoadScene(StaticController.selectStageName);
                }
                else
                {
                    SceneManager.LoadScene("Title");
                }
            }

            if (EventSystem.current.currentSelectedGameObject == this.gameObject)
            {
                au.clip = sounds[0];
                au.Play();
            }

            if ((Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("action")))
            {
                au.clip = sounds[1];
                au.Play();

                if (EventSystem.current.currentSelectedGameObject.name == "Yes")
                {
                    animator_Mask.SetTrigger("WhiteToBlack");
                    loadSceneName = StaticController.selectStageName;
                    btn.enabled = false;
                }
                else if (EventSystem.current.currentSelectedGameObject.name == "No")
                {
                    animator_Mask.SetTrigger("WhiteToBlack");
                    loadSceneName = "Title";
                    btn.enabled = false;
                }
            }
        }
    }
}
