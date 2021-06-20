using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ActorInStageSelect : MonoBehaviour
{
    public GameObject[] target;
    public Button[] btn;
    public Animator animator;
    public static int selectBtn = 1;      //  選択しているボタン標記
    private bool goLeft;
    private bool goRight;
    private bool isMove;

    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (GoalController.clearStageName == "")        //  StageからStageSelectに飛びるではない場合
        {
            if (!isMove && !StageSelectButtonController.outputConfirmUI)
            {
                if (selectBtn < 4)
                {
                    if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetAxis("axisX") > 0)
                    {
                        goRight = true;
                        selectBtn++;
                    }
                }

                if (selectBtn > 1)
                {
                    if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetAxis("axisX") < 0)
                    {
                        goLeft = true;
                        selectBtn--;
                    }
                }
            }
        }
        else                                            //  StageをクリアしてStageから飛びる場合
        {
            if (selectBtn < 4)
            {
                goRight = true;
                selectBtn++;
                btn[selectBtn - 1].GetComponent<StageSelectButtonController>().canSelected = true;
                GoalController.clearStageName = "";
            }
        }

        if (goLeft || goRight)
        {
            isMove = true;
            animator.SetFloat("Forward", 1.0f);
            EventSystem.current.SetSelectedGameObject(null);

            if (goLeft)     //  StageSelect画面のプレイヤー移動処理
            {
                if (selectBtn != 3)
                {
                    transform.position += new Vector3(-0.1f, 0.0f, 0.0f);
                }

                if (selectBtn == 1)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, target[0].transform.rotation, 5.0f * Time.deltaTime);
                }
                else if (selectBtn == 2)
                {
                     transform.rotation = Quaternion.Lerp(transform.rotation, target[5].transform.rotation, 5.0f * Time.deltaTime);
                }
                else if (selectBtn == 3)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, target[4].transform.rotation, 5.0f * Time.deltaTime);
                }
            }
            else if (goRight)
            {               
                if (selectBtn != 4)
                {
                    transform.position += new Vector3(0.1f, 0.0f, 0.0f);
                }

                if (selectBtn == 2)
                {                    
                    transform.rotation = Quaternion.Lerp(transform.rotation, target[1].transform.rotation, 5.0f * Time.deltaTime);
                }
                else if (selectBtn == 3)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, target[2].transform.rotation, 5.0f * Time.deltaTime);
                }
                else if (selectBtn == 4)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, target[3].transform.rotation, 5.0f * Time.deltaTime);
                }
            }
        }
        
    }

    void OnTriggerEnter(Collider collider)      //  止まる判定
    {
        if (collider.transform.tag == "RightPoint" && goRight)
        {
            isMove = false;
            goRight = false;
            animator.SetFloat("Forward", 0.0f);

            for (int i = 0; i < btn.Length; i++)
            {
                btn[i].enabled = true;
            }

            EventSystem.current.SetSelectedGameObject(btn[selectBtn - 1].gameObject);
        }

        if (collider.transform.tag == "LeftPoint" && goLeft)
        {
            isMove = false;
            goLeft = false;
            animator.SetFloat("Forward", 0.0f);

            for (int i = 0; i < btn.Length; i++)
            {
                btn[i].enabled = true;
            }

            EventSystem.current.SetSelectedGameObject(btn[selectBtn - 1].gameObject);
        }
    }
}
