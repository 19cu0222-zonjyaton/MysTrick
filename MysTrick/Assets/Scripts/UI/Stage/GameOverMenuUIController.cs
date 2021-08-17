using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameOverMenuUIController : MonoBehaviour
{
    public GameObject selectButton;

    private CameraController cc;
    private CanvasGroup canvasGroup;
    private bool playAnim;
    private bool doOnce;

    void Awake()
    {
        cc = GameObject.Find("Main Camera").GetComponent<CameraController>();

        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        if (cc.deadMoveStart)
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            playAnim = true;
        }

        if (playAnim && !doOnce)
        {
            EventSystem.current.SetSelectedGameObject(selectButton);
            gameObject.GetComponent<Animation>().Play();
            doOnce = true;
        }
    }
}
