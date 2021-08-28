using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StageSelectButtonController : MonoBehaviour
{
    public bool canSelected;                    //  クリアしたかどうか
    public GameObject confirmPanel;
    public Sprite[] stageImage;
    public int stageNum;

    private Image img;
    private Vector3 startPos;                   //  始点の位置

    void Awake()
    {
        img = gameObject.GetComponent<Image>();

        startPos = transform.parent.position;
    }

    // Update is called once per frame
    void Update()
    {
        //  画像の色
        if (!canSelected)       
        {
            img.color = Color.grey;
        }
        else
        {
            img.color = Color.white;
        }

        //  Panelの移動処理
        if (ActorInStageSelect.selectBtn == 3)      
        {
            gameObject.transform.parent.position = Vector3.MoveTowards(transform.parent.position, startPos, 75.0f * Time.deltaTime);
        }
        else if(ActorInStageSelect.selectBtn == 4)
        {
            gameObject.transform.parent.position = Vector3.MoveTowards(transform.parent.position, startPos - new Vector3(250.0f, 0.0f, 0.0f), 75.0f * Time.deltaTime);
        }

        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("action")) && canSelected && !StaticController.confirmMenuIsOpen && !StaticController.exitPanelIsOpen)
        {
            StaticController.confirmMenuIsOpen = true;
            confirmPanel.SetActive(true);
        }

        for (int i = 0; i < stageImage.Length; i++)
        {
            if (i == stageNum)
            {
                if (canSelected)
                {
                    img.sprite = stageImage[1];
                }
                //img.sprite = stageImage[StaticController.imageIndex[stageNum]];
            }
        }
    }
}
