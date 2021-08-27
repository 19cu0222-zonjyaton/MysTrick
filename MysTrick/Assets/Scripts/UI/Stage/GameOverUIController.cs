using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverUIController : MonoBehaviour
{
    private ActorController ac;
    private CameraController cc;
    private CanvasGroup canvasGroup;
    private bool playAnim;
    private bool doOnce;

    void Awake()
    {
        ac = GameObject.Find("PlayerHandle").GetComponent<ActorController>();

        cc = GameObject.Find("Main Camera").GetComponent<CameraController>();

        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        if (ac.isDead || ac.isFallDead)
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            playAnim = true;
        }

        if (playAnim && !doOnce)
        {
            gameObject.GetComponent<Animation>().Play();
            doOnce = true;
        }

        if (cc.deadMoveStart)
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}
