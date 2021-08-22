using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CoinCountController : MonoBehaviour
{
    public int[] nowCount;
    public int[] highCount;
    public float sumCount;
    public int stageNum;
    private Text textFrame;

    void Awake()
    {
        textFrame = gameObject.GetComponent<Text>();
    }

    void Start()
    {
        //for (int i = 0; i < 4; i++)
        //{
        //    if (stageNum == i)
        //    {
        //        textFrame.text = (Math.Round((GoalController.getCount[stageNum] / sumCount) * 100.0f, 0)).ToString() + "%";
        //    }
        //}
    }

    // Update is called once per frame
    void Update()
    {

    }
}
