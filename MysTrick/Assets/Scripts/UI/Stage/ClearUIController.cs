using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearUIController : MonoBehaviour
{
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
        if (cc.cameraStatic == "GameClear")
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
    }
}
