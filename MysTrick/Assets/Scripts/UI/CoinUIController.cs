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

        textFrame = gameObject.GetComponent<Text>();

        oldPos = transform.localPosition;
    }

    void Update()
    {
        textFrame.text = "Coin×" + ac.coinCount;

        if (ac.coinUIAction)    //  UIの振れ処理
        {
            countTime += Time.deltaTime;
            if (countTime <= 0.08f)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition + new Vector3(0.0f, 4.0f, 0.0f), 1.5f);
            }
            else if (countTime > 0.08f && countTime <= 0.16f)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, oldPos, 3.0f);
            }
            else if (countTime > 0.16f && countTime <= 0.18f)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition + new Vector3(0.0f, 2.5f, 0.0f), 4.0f);
            }
            else if (countTime > 0.18f && countTime <= 0.2f)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, oldPos, 8.0f);
            }
            else
            {
                countTime = 0.0f;
                ac.coinUIAction = false;
            }
        }      
    }
}
