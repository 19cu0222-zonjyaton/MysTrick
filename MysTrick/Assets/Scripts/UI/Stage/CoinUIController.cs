using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinUIController : MonoBehaviour
{
    private ActorController ac;
    private Text textFrame;
    private float countTime;
    private Vector3 oldPos;

    void Awake()
    {
        ac = GameObject.Find("PlayerHandle").GetComponent<ActorController>();

        textFrame = GameObject.Find("CoinText2").GetComponent<Text>();

        oldPos = transform.localPosition;
    }

    void Update()
    {
        textFrame.text = ac.coinCount.ToString();

        if (ac.coinUIAction)    //  UIの振れ処理
        {
            countTime += Time.deltaTime;
            if (countTime <= 0.08f)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, transform.localPosition + new Vector3(0.0f, 4.0f, 0.0f), 20.0f * Time.deltaTime);
            }
            else if (countTime > 0.08f && countTime <= 0.16f)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, oldPos, 45.0f * Time.deltaTime);
            }
            else if (countTime > 0.16f && countTime <= 0.18f)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, transform.localPosition + new Vector3(0.0f, 2.5f, 0.0f), 60.0f * Time.deltaTime);
            }
            else if (countTime > 0.18f && countTime <= 0.2f)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, oldPos, 100.0f * Time.deltaTime);
            }
            else
            {
                countTime = 0.0f;
                ac.coinUIAction = false;
            }
        }
    }
}
