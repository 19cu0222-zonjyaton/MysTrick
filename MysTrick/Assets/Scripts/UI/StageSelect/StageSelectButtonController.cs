using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StageSelectButtonController : MonoBehaviour
{
    public GameObject confirmPanel;
    public Sprite[] stageImage;
    public int stageNum;

    private Image img;
    private Vector3 startPos;                   //  始点の位置
    private ActorInStageSelect aiss;

    void Awake()
    {
        img = gameObject.GetComponent<Image>();

        startPos = new Vector3(0, 0, 0);

        aiss = GameObject.Find("PlayerHandle").GetComponent<ActorInStageSelect>();

        if (ActorInStageSelect.selectBtn == 4)
        {
            gameObject.transform.parent.position = new Vector3(-250, 0, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //  Panelの移動処理
        if (ActorInStageSelect.selectBtn == 3)      
        {
            gameObject.transform.parent.position = Vector3.MoveTowards(transform.parent.position, startPos, 75.0f * Time.deltaTime);
        }
        else if(ActorInStageSelect.selectBtn == 4)
        {
            gameObject.transform.parent.position = Vector3.MoveTowards(transform.parent.position, new Vector3(-250.0f, 0.0f, 0.0f), 75.0f * Time.deltaTime);
        }

        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("action")) && StaticController.stageCanSelect[ActorInStageSelect.selectBtn - 1] && !StaticController.confirmMenuIsOpen && !StaticController.exitPanelIsOpen && !aiss.isMove)
        {
            StaticController.confirmMenuIsOpen = true;
            confirmPanel.SetActive(true);
        }

        for (int i = 0; i < 4; i++)
        {
            if (stageNum == i)
            {
                if (StaticController.stageIsClear[i] || stageNum == 0)
                {
                    img.sprite = stageImage[StaticController.highScore[i] + 1];
                }
                else if (i > 0)
                {
                    if (!StaticController.stageIsClear[i] && StaticController.stageIsClear[i - 1] && stageNum == i)
                    {
                        img.sprite = stageImage[1];
                    }
                }
                else if(!StaticController.stageIsClear[i])
                {
                    img.sprite = stageImage[1];
                }
            }
        }
    }
}
