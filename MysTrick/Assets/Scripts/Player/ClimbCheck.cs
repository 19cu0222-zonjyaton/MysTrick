using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbCheck : MonoBehaviour
{
    public GameObject player;
    public LadderController lc;
    public GameObject camera;
    public GameObject cameraPos;    //  登る時カメラの位置
    public GameObject startPos;     //  登る始点
    public GameObject endPos;       //  登る終点
    public GameObject[] landPos;      //  到着位置
    public GameObject checkPos;
    public bool climbStart;
    public bool needTrigger;        //  false -> いつでも使えるladder
    public int nowLayer = 2;        //  プレイヤー今の階層

    private GameObject playerModule;
    private PlayerInput pi;
    private ActorController ac;
    private CameraController cc;


    void Awake()
    {
        playerModule = GameObject.Find("PlayerModule");

        pi = player.GetComponent<PlayerInput>();

        ac = player.GetComponent<ActorController>();

        cc = camera.GetComponent<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (climbStart)     //  登り始める
        {
            cc.cameraStatic = "LookAtPlayer";
            playerModule.transform.rotation = transform.rotation;
            pi.inputEnabled = false;
            camera.transform.position = Vector3.Slerp(camera.transform.position, cameraPos.transform.position, 5.0f * Time.deltaTime);
            camera.transform.LookAt(player.transform);
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
            //tempPos = startPos;
            //startPos = endPos;
            //endPos = tempPos;
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
            cc.cameraStatic = "Idle";
            player.GetComponent<Rigidbody>().useGravity = true;            
            climbStart = false;
            ac.climbEnd = false;
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.transform.tag == "Player" && pi.isTriggered && lc.rotateFinish && (lc.i == 1 || lc.i == 3))
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
