//-------------------------------------------------
// ファイル名		：SkyBoxController.cs
// 概要				：Skyboxマテリアル交換
// 作成者			：曹飛
// 更新内容			：2021/06/28 作成
//-------------------------------------------------
//-------------------------------------------------
// ファイル名		：SkyBoxController.cs
// 概要				：フェイド追加
// 作成者			：林雲暉 
// 更新内容			：2022/02/13 フェイド追加
//-------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBoxController : MonoBehaviour
{
    public Material[] boxs;
    public ActorInStageSelect aiss;

    private int curSkyBoxIndex = 0;     // 現在のSkyBoxの番号
    public float maxFadeTime;           // フェイドするの時間
    private float curTime;              // フェイド計算用カウンター
    private bool isFadeIn;              // フェイドインするか？
    private bool isFadeOut;             // フェイドアウトするか？
    private bool changeSkyBox;          // SkyBoxマテリアル変更するか？
    private float maxExposure;          // 現在最大のExposure
    private bool isMove;
    private bool doOnce;
    private int aaa;

    void Awake()
    {
        curTime = 0;
        isFadeIn = false;
        isFadeOut = false;
        changeSkyBox = false;
        maxExposure = 1;
    }

    private void Start()
    {
        if (aiss != null)
        {
            RenderSettings.skybox = boxs[aiss.skyboxIndex];     // 初期SkyBoxを設定
            isFadeIn = true;
        } // end if()
    } // void Start()

    // Update is called once per frame
    void Update()
    {
        // 
        //if ( aiss != null && curSkyBoxIndex != aiss.skyboxIndex )
        //{
        //    isFadeOut = true;
        //} // end if()


        if ((aiss.goLeft || aiss.goRight) && !doOnce)
        {
            isMove = true;
            isFadeOut = true;
            doOnce = true;
        }

        // 背景フェイドアウト
        if (isFadeOut == true)
        {
            if (curTime < maxFadeTime)
            {
                curTime += Time.deltaTime;
                RenderSettings.skybox.SetFloat("_Exposure", Mathf.Lerp(maxExposure, 0, (curTime / maxFadeTime)));         // 画面暗転、Exposure調整
            }
            else
            {
                changeSkyBox = true;
                isFadeOut = false;
                isMove = false;
                curTime = 0;
            }
            //    // old one
            //    // public Color colorStart = Color.black;
            //    // public Color colorEnd;
            //    // RenderSettings.skybox.SetColor("_Tint", Color.Lerp(colorEnd, colorStart, (curTime / maxFadeTime)));
            //} // end if()
            //else
            //{
            //    changeSkyBox = true;
            //    isFadeOut = false;
            //    curTime = 0;
            //} // end else
        } // end if()

        // 背景交換(SkyBox)
        if (changeSkyBox == true)
        {
            if (aiss != null)
            {
                if (boxs[aiss.skyboxIndex] != null)
                {
                    // Skyboxマテリアル変更
                    RenderSettings.skybox.SetFloat("_Exposure", 0);
                    boxs[curSkyBoxIndex].SetFloat("_Exposure", maxExposure);
                    changeSkyBox = false;
                    isFadeIn = true;
                    curSkyBoxIndex = aiss.skyboxIndex;
                    maxExposure = GetDefaultExposure(curSkyBoxIndex);
                } // end if
            } // end if
        } // end if()

        // 背景フェイドイン
        if (isFadeIn)
        {
            if (curTime < maxFadeTime)
            {
                RenderSettings.skybox = boxs[aiss.skyboxIndex];
                curTime += Time.deltaTime;
                RenderSettings.skybox.SetFloat("_Exposure", Mathf.Lerp(0, maxExposure, (curTime / maxFadeTime)));         // Exposure調整
            } // end if()
            else
            {
                RenderSettings.skybox = boxs[aiss.skyboxIndex];
                isFadeIn = false;
                doOnce = false;
                curTime = 0;
            } // end else
        } // end if()
    } // void Update()

    // 各マテリアルの初期Exposureを取得
    private float GetDefaultExposure( int index )
    {
        if (index == 2)
            return 0.8f;
        else return 1;
    } // GetDefaultExposure()
}
