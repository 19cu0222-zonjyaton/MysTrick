using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCoinController : MonoBehaviour
{
    public float perRadian;         //  毎回変化の弧度
    public float radius;
    public float radian;            //  弧度
    private Vector3 oldPos;
    private new Rigidbody rigidbody;
    private ActorController ac;
    private bool getByPlayer;       //  プレイヤーと当たったflag
    private float rotateSpeed = 12.0f;

    void Awake()
    {
        oldPos = transform.position;        //  最初の位置    

        rigidbody = gameObject.GetComponent<Rigidbody>();

        ac = GameObject.Find("PlayerHandle").GetComponent<ActorController>();

        rigidbody.AddForce(0.0f, 500.0f, 0.0f);
    }

    void Update()
    {
        transform.Rotate(0, rotateSpeed, 0);

        if (transform.position.y < oldPos.y - 0.7f)
        {
            Destroy(this.gameObject);
        }
    }
}
