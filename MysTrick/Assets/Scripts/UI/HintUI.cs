using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintUI : MonoBehaviour
{
    private float radian = 0;               //  弧度
    private float perRadian = 0.04f;         //  毎回変化の弧度
    private float radius = 0.015f;
    private Vector3 oldPos;

    private ActorController actorController;
    private PlayerInput pi;

    void Awake()
    {
        actorController = GameObject.Find("PlayerHandle").GetComponent<ActorController>();

        pi = GameObject.Find("PlayerHandle").GetComponent<PlayerInput>();
    }

    void Update()
    {
        if (actorController.isInTrigger && !pi.lockJumpStatus)
        {
            gameObject.GetComponent<Renderer>().enabled = true;     //  meshを隠す

            oldPos = transform.position;        //  最初の位置

            radian += perRadian;                //  毎回弧度を0.2をプラスする
            float dy = Mathf.Cos(radian) * radius;
            transform.position = oldPos + new Vector3(0, dy, 0);
        }
        else
        {
            gameObject.GetComponent<Renderer>().enabled = false;
        }
    }
}
