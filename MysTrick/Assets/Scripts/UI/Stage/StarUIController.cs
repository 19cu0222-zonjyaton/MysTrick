using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarUIController : MonoBehaviour
{
    public Sprite[] sprite;             //  画像オブジェクト
    public GameObject starUI;           //  starのUI
    public GameObject[] star;           //  作るUIの用意場所
    public StarBoxController sbc;
    public StarController[] sc;
    public GameObject starParent;       //  親のオブジェクト

    private int uiPos;
    private bool[] doOnce = { false, false, false };

    void Awake()
    {
    }

    void Update()
    {
        for (int i = 0; i < 2; i++)
        {
            if (sbc.uiAnimStart && !doOnce[2])
            {
                star[2] = Instantiate(starUI, transform.position + new Vector3(uiPos * 100.0f, 0.0f, 0.0f), Quaternion.identity);
                star[2].transform.SetParent(starParent.transform);
                uiPos++;
                doOnce[2] = true;
            }

            if (sc[i].uiAnimStart && !doOnce[i])
            {
                star[i] = Instantiate(starUI, transform.position + new Vector3(uiPos * 100.0f, 0.0f, 0.0f), Quaternion.identity);
                star[i].transform.SetParent(starParent.transform);
                uiPos++;
                doOnce[i] = true;
            }
        }
    }
}
