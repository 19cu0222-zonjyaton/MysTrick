using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriKeyUIController : MonoBehaviour
{
    public Sprite[] triSprite;
    public GameObject triImg;
    public GameObject triKeyUI;        //  親のオブジェクト
    public GameObject[] triKey;        //  作るUIの用意場所
    public TriKeyController[] tkc;

    private ActorController ac;
    private int uiPos;
    private bool[] doOnce = {false, false, false};

    void Awake()
    {
        ac = GameObject.Find("PlayerHandle").GetComponent<ActorController>();

        triKey = new GameObject[triSprite.Length];
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < ac.havePieces.Length; i++)
        {
            if (ac.havePieces[i] && tkc[i].uiAnimStart && !doOnce[i])
            {
                triImg.GetComponent<Image>().sprite = triSprite[i];
                triKey[i] = Instantiate(triImg, transform.position + new Vector3(uiPos * 100.0f, 0.0f, 0.0f), Quaternion.identity);
                triKey[i].transform.SetParent(triKeyUI.transform);
                uiPos++;
                doOnce[i] = true;
            }
        }
    }
}
