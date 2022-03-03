using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFade : MonoBehaviour
{
    public float fadeTime = 5.0f;               // フェイドタイム
    public float blackScreenDuration = 1.0f;    // 黒画面の時間

    public Color fadeColor;                     // フェイドカラー

    public float desiredAlpha = 1.0f;           // 目標アルファ値
    public float startAlpha = 0.0f;           // 目標アルファ値
    public float currentAlpha = 0.0f;              // 現在のアルファ値

    public bool isFadeOut = true;               // フェイドアウトするのか
    public bool isFading = false;               // 現在はフェードしていますか

    private Texture2D _texture;                 // 描画するテクスチャ
    private float _passedBlackScreenTime;       // 経過した時間

    // Start is called before the first frame update
    private void Start()
    {
        _texture = new Texture2D(1, 1);
        _texture.SetPixel(0, 0, new Color(fadeColor.r, fadeColor.g, fadeColor.b, currentAlpha));
        _texture.Apply();

        currentAlpha = startAlpha;
    }

    public void OnGUI()
    {
        if (isFading == false)
        {
            return;
        } // end if()

        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _texture);

        if (_passedBlackScreenTime < blackScreenDuration)
        {
            _passedBlackScreenTime += Time.deltaTime;
            return;
        } // end if()

        CalculateTexture();
    }

    private void CalculateTexture()
    {
        //currentAlpha = Mathf.MoveTowards(currentAlpha, desiredAlpha, 2.0f * Time.deltaTime);

        if (isFadeOut == false)
        {
            currentAlpha -= Time.deltaTime / fadeTime;

            if (currentAlpha <= desiredAlpha)
                ResetAlpha();
        } // end if()
        else
        {
            currentAlpha += Time.deltaTime / fadeTime;

            if (currentAlpha >= desiredAlpha)
                ResetAlpha();
        } // end else

        _texture.SetPixel(0, 0, new Color(fadeColor.r, fadeColor.g, fadeColor.b, currentAlpha));
        _texture.Apply();
    }

    private void ResetAlpha()
    {
        currentAlpha = startAlpha;
        isFading = false;
        _passedBlackScreenTime = 0;
    }

}
