using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearUIController : MonoBehaviour
{
    private PlayerInput pi;
    private CameraController cc;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        pi = GameObject.Find("PlayerHandle").GetComponent<PlayerInput>();

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
        }
        else
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}
