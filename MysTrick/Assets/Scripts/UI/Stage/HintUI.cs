using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintUI : MonoBehaviour
{
    public float perRadian;         //  毎回変化の弧度
    public float radius;
    public MeshRenderer mesh;
    public Material newMaterial;
    public bool meshCanChange;
    public string lockUIName;

    private ActorController actorController;
    private GameObject cameraPos;
    private PlayerInput pi;
    private ActorController ac;
    private float radian = 0;               //  弧度
    private Vector3 oldPos;


    void Awake()
    {
        actorController = GameObject.Find("PlayerHandle").GetComponent<ActorController>();

        cameraPos = GameObject.Find("Main Camera");

        pi = GameObject.Find("PlayerHandle").GetComponent<PlayerInput>();

        oldPos = transform.localPosition;        //  最初の位置   

        if (meshCanChange)
        {
            ac = GameObject.Find("PlayerHandle").GetComponent<ActorController>();
        }
    }

    void Update()
    {
        if (actorController.isInTrigger && !pi.lockJumpStatus)
        {
            radian += perRadian;                //  毎回弧度を0.01をプラスする
            float dy = Mathf.Cos(radian) * radius;
            transform.localPosition = oldPos + new Vector3(0, dy, 0);

            transform.forward = (cameraPos.transform.position - transform.position).normalized;     //  いつでもカメラに向けさせる

            if (meshCanChange)
            {
                if (ac.haveKeys.BlueKey && lockUIName == "BlueDoor")
                {
                    mesh.material.CopyPropertiesFromMaterial(newMaterial);     //  materialを変更するメソッド
                }

                if (!ac.haveKeys.GreenKey && lockUIName == "GreenDoor")
                {
                    mesh.material.CopyPropertiesFromMaterial(newMaterial);
                }
            }
        }
    }
}

