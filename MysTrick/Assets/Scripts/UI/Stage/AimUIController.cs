using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimUIController : MonoBehaviour
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
        if (pi.isAimStatus && cc.cameraStatic == "Idle")
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
