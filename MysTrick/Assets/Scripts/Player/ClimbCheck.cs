using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbCheck : MonoBehaviour
{
    public GameObject player;               //  プレイヤーオブジェクト
    public LadderController lc;             //  梯子オブジェクト
    public GameObject[] startPos;             //  登る始点           
    public GameObject checkPos;             //  梯子から降ろす位置
    public bool climbStart;                 //  梯子スタートフラグ
    public int nowLayer = 2;                //  プレイヤー今の階層
    public GameObject hintUI;               //  仕掛けのヒントUIオブジェクト
    public GameObject playerModule;         //  プレイヤーモデルオブジェクト

    private PlayerInput pi;                 //  プレイヤー入力コントローラー
    private ActorController ac;             //  プレイヤー挙動コントローラー

    public LadderType ladderType;
    public enum LadderType
    {
        rotateLadder,
        normalLadder
    }

    //  初期化
    void Awake()
    {
        pi = player.GetComponent<PlayerInput>();

        ac = player.GetComponent<ActorController>();
    }

    void Update()
    {
        if (climbStart)     //  登り始める
        {
            playerModule.transform.rotation = transform.rotation;
            pi.inputEnabled = false;
            ac.isClimbing = true;
            player.GetComponent<Rigidbody>().useGravity = false;
            if (Input.GetKey(pi.keyUp))
            {
                player.transform.position += new Vector3(0, 0.1f, 0);
            }
            else if(Input.GetKey(pi.keyDown))
            {
                player.transform.position -= new Vector3(0, 0.1f, 0);
            }
        }

        if (ac.climbEnd && climbStart)    //  登り終わる
        {
            player.transform.position = ac.climbLandPos + new Vector3(0, 0, 3.0f);
            pi.inputEnabled = true;
            ac.isClimbing = false;
            player.GetComponent<Rigidbody>().useGravity = true;            
            climbStart = false;
            ac.climbEnd = false;
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.transform.tag == "Player" && (lc.i == 1 || lc.i == 3))
        {
            hintUI.SetActive(true);
            ac.isInTrigger = true;
            if (pi.isTriggered && lc.rotateFinish)
            {
                ac.climbEnd = false;
                if (lc.i != 3)
                {
                    player.transform.position = startPos[0].transform.position;
                }
                else {
                    player.transform.position = startPos[1].transform.position;
                }

                climbStart = true;
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.transform.tag == "Player")
        {
            hintUI.SetActive(false);
        }
    }
}
