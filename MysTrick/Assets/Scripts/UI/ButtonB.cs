using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonB : MonoBehaviour
{
    private ActorController actorController;
    private CanvasGroup canvasGroup;
    private Animator animator;

    void Awake()
    {
        actorController = GameObject.Find("PlayerHandle").GetComponent<ActorController>();

        animator = GetComponent<Animator>();

        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        if (actorController.isInTrigger)
        {
            //  UIを出す
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            animator.SetBool("isShow", true);
        }
        else
        {
            //  UIを隠す
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            animator.SetBool("isShow", false);
        }
    }
}
