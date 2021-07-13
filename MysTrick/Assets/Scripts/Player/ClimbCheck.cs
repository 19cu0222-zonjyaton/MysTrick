using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbCheck : MonoBehaviour
{
    public GameObject player;               //  プレイヤーオブジェクト
    public LadderController lc;             //  梯子オブジェクト
    public GameObject cameraStartPos;       //  登る前カメラに位置
    public GameObject cameraTargetPos;      //  登る時カメラの位置
    public GameObject startPos;             //  登る始点
    public GameObject endPos;               //  登る終点
    public GameObject[] landPos;            //  到着位置
    public GameObject checkPos;             //  梯子から降ろす位置
    public bool climbStart;                 //  梯子スタートフラグ
    public int nowLayer = 2;                //  プレイヤー今の階層
    public GameObject hintUI;               //  仕掛けのヒントUIオブジェクト
    public GameObject playerModule;         //  プレイヤーモデルオブジェクト

    private PlayerInput pi;                 //  プレイヤー入力コントローラー
    private ActorController ac;             //  プレイヤー挙動コントローラー
    private CameraController cc;            //  カメラコントローラー

    //  初期化
    void Awake()
    {
        pi = player.GetComponent<PlayerInput>();

        ac = player.GetComponent<ActorController>();

        cc = cameraStartPos.GetComponent<CameraController>();
    }

    void Update()
    {
        if (climbStart)     //  登り始める
        {
            cc.cameraStatic = "LookAtPlayer";
            playerModule.transform.rotation = transform.rotation;
            pi.inputEnabled = false;
            ac.isClimbing = true;
            cameraStartPos.transform.position = Vector3.Slerp(cameraStartPos.transform.position, cameraTargetPos.transform.position, 5.0f * Time.deltaTime);
            cameraStartPos.transform.LookAt(player.transform);
            player.GetComponent<Rigidbody>().useGravity = false;
            if (nowLayer == 1 || nowLayer == 3)
            {
                player.transform.position = Vector3.MoveTowards(player.transform.position, startPos.transform.position, 2.0f * Time.deltaTime);
            }
            else
            {
                player.transform.position = Vector3.MoveTowards(player.transform.position, endPos.transform.position, 2.0f * Time.deltaTime);
            }
        }

        if (ac.climbEnd)    //  登り終わる
        {
            if (player.transform.position.y < -6.0f)
            {
                nowLayer = 1;
                player.transform.position = landPos[0].transform.position;
                checkPos.transform.position = landPos[0].transform.position;
            }
            else if (player.transform.position.y > -4.0f && player.transform.position.y < 5.0f)
            {
                nowLayer = 2;
                player.transform.position = landPos[1].transform.position;
                checkPos.transform.position = landPos[1].transform.position;
            }
            else
            {
                nowLayer = 3;
                player.transform.position = landPos[2].transform.position;
                checkPos.transform.position = landPos[2].transform.position;
            }
            pi.inputEnabled = true;
            ac.isClimbing = false;
            cc.cameraStatic = "Idle";
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
                if (nowLayer == 2)
                {
                    player.transform.position = startPos.transform.position;
                }
                else
                {
                    player.transform.position = endPos.transform.position;
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
