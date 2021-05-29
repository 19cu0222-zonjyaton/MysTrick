using UnityEngine;
using UnityEngine.UI;

public class HPUIController : MonoBehaviour
{
    private ActorController ac;
    private Text textFrame;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        ac = GameObject.Find("PlayerHandle").GetComponent<ActorController>();

        canvasGroup = GetComponent<CanvasGroup>();

        textFrame = gameObject.GetComponent<Text>();
    }

    void Update()
    {
        textFrame.text = "HP:" + ac.hp;

        if (!ac.isDead)
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
