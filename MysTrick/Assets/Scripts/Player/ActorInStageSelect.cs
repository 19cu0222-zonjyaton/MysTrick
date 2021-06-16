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
    private static int selectBtn = 1;      //  選択しているボタン標記
    private bool goLeft;
    private bool goRight;
    private bool isMove;

    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (GoalController.clearStageName == "" )
        {
            if (!isMove && !StageSelectButtonController.outputConfirmUI)
            {
                if (selectBtn < 4)
                {
                    if (Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        goRight = true;
                        selectBtn++;
                    }
                }

                if (selectBtn > 1)
                {
                    if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        goLeft = true;
                        selectBtn--;
                    }
                }
            }
        }
        else
        {
            if (selectBtn < 4)
            {
                goRight = true;
                selectBtn++;
                GoalController.clearStageName = "";
            }
        }

        if (goLeft || goRight)
        {
            isMove = true;
            animator.SetFloat("Forward", 1.0f);

            if (goLeft)     //  StageSelect画面のプレイヤー移動処理
            {
                if (selectBtn != 3)
                {
                    transform.position += new Vector3(-0.1f, 0.0f, 0.0f);
                }

                if (selectBtn == 1)
                {
                    btn[0].enabled = false;
                    transform.rotation = Quaternion.Lerp(transform.rotation, target[0].transform.rotation, 100.0f * Time.deltaTime);
                }
                else if (selectBtn == 2)
                {
                    btn[1].enabled = false;
                    transform.rotation = Quaternion.Lerp(transform.rotation, target[4].transform.rotation, 100.0f * Time.deltaTime);
                }
                else if (selectBtn == 3)
                {
                    btn[2].enabled = false;
                    transform.rotation = Quaternion.Lerp(transform.rotation, target[3].transform.rotation, 100.0f * Time.deltaTime);
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
                    btn[1].enabled = false;
                    transform.rotation = Quaternion.Lerp(transform.rotation, target[1].transform.rotation, 100.0f * Time.deltaTime);
                }
                else if (selectBtn == 3)
                {
                    btn[2].enabled = false;
                    transform.rotation = Quaternion.Lerp(transform.rotation, target[2].transform.rotation, 100.0f * Time.deltaTime);
                }
                else if (selectBtn == 4)
                {
                    btn[3].enabled = false;
                }
            }
        }
        
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.tag == "MovePoint")
        {
            isMove = false;
            goLeft = false;
            goRight = false;
            animator.SetFloat("Forward", 0.0f);

            for (int i = 0; i < btn.Length; i++)
            {
                btn[i].enabled = true;
            }

            //EventSystem.current.firstSelectedGameObject = btn[selectBtn - 1].gameObject;
            EventSystem.current.SetSelectedGameObject(btn[selectBtn - 1].gameObject);
        }
    }
}
