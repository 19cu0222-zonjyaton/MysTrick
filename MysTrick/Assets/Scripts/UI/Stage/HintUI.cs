using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintUI : MonoBehaviour
{
    public float perRadian = 0.01f;         //  毎回変化の弧度
    public float radius = 0.1f;
    private float radian = 0;               //  弧度
    private Vector3 oldPos;

    private ActorController actorController;
    private PlayerInput pi;
    

    void Awake()
    {
        actorController = GameObject.Find("PlayerHandle").GetComponent<ActorController>();

        pi = GameObject.Find("PlayerHandle").GetComponent<PlayerInput>();

        oldPos = transform.position;        //  最初の位置     
    }

    void Update()
    {
        if (actorController.isInTrigger && !pi.lockJumpStatus)
        {
            radian += perRadian;                //  毎回弧度を0.01をプラスする
            float dy = Mathf.Cos(radian) * radius;
            transform.position = oldPos + new Vector3(0, dy, 0);
        }
    }
}
