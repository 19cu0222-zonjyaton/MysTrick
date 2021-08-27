using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinUIController : MonoBehaviour
{
    public Text textFrame;

    private ActorController ac;
    private Animation anim;
    private int mapCoinCount;

    void Awake()
    {
        ac = GameObject.Find("PlayerHandle").GetComponent<ActorController>();

        anim = gameObject.GetComponent<Animation>();

        mapCoinCount = GameObject.FindGameObjectsWithTag("Coin").Length;
    }

    void Update()
    {
        textFrame.text = ac.coinCount.ToString() + " / " + mapCoinCount;

        if (ac.coinUIAction)    //  UIの振れ処理
        {
            AnimationState state = anim["Coin"];        //  Animation動画の状態を最初から戻る
            anim.Play("Coin");
            state.time = 0;
            ac.coinUIAction = false;
        }
    }
}
