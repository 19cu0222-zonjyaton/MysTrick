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
    public float maxFadeInTime;           // フェイドするの時間
    public float maxFadeOutTime;           // フェイドするの時間
    private float curTime;              // フェイド計算用カウンター
    private bool isFadeIn;              // フェイドインするか？
    private bool isFadeOut;             // フェイドアウトするか？
    private bool changeSkyBox;          // SkyBoxマテリアル変更するか？
    private float maxExposure;          // 現在最大のExposure

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
            maxExposure = GetDefaultExposure(curSkyBoxIndex);
        } // end if()
    } // void Start()

    // Update is called once per frame
    void Update()
    {
        if ( aiss != null && curSkyBoxIndex != aiss.nextSelectStageNum)
        {
           isFadeOut = true;
        } // end if()

        // 背景フェイドアウト
        if (isFadeOut == true)
        {
            if (curTime < maxFadeOutTime)
            {
                curTime += Time.deltaTime;
                RenderSettings.skybox.SetFloat("_Exposure", Mathf.Lerp(maxExposure, 0, (curTime / maxFadeOutTime)));         // 画面暗転、Exposure調整
            } // end if()
            else
            {
                changeSkyBox = true;
                isFadeOut = false;
                curTime = 0;
            } // end else
            //    // old one
            //    // public Color colorStart = Color.black;
            //    // public Color colorEnd;
            //    // RenderSettings.skybox.SetColor("_Tint", Color.Lerp(colorEnd, colorStart, (curTime / maxFadeTime)));

        } // end if()

        // 背景交換(SkyBox)
        if (changeSkyBox == true)
        {
            if (aiss != null)
            {
                if (boxs[aiss.nextSelectStageNum] != null)
                {
                    // Skyboxマテリアル変更
                    RenderSettings.skybox.SetFloat("_Exposure", 0);
                    // 次のSkyboxマテリアルのExposureを0に
                    boxs[aiss.nextSelectStageNum].SetFloat("_Exposure", 0);
                    // Skyboxマテリアル変更
                    RenderSettings.skybox = boxs[aiss.nextSelectStageNum];
                    // 前のSkyboxマテリアルのExposureをデフォルトに
                    boxs[curSkyBoxIndex].SetFloat("_Exposure", maxExposure);

                    changeSkyBox = false;
                    isFadeIn = true;
                    curSkyBoxIndex = aiss.nextSelectStageNum;

                    //　現在のSkyboxマテリアルのExposure最大値をゲット
                    maxExposure = GetDefaultExposure(curSkyBoxIndex);
                } // end if
            } // end if
        } // end if()

        // 背景フェイドイン
        if (isFadeIn)
        {
            if (curTime < maxFadeInTime)
            {
                curTime += Time.deltaTime;
                RenderSettings.skybox.SetFloat("_Exposure", Mathf.Lerp(0, maxExposure, (curTime / maxFadeInTime)));         // Exposure調整
            } // end if()
            else
            {
                isFadeIn = false;
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
