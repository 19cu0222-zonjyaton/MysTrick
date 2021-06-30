﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowWeaponController : MonoBehaviour
{
    public float rotateSpeed;       //  回転速度
    public float speed;             //  スタートの移動速度
    
    private new Rigidbody rigidbody;
    private GameObject playerPos;   //  プレイヤーの位置を獲得するため
    private PlayerInput pi;         //  攻撃ができるかどうかの判断
    private GameObject playerCamera;
    private WeaponController wc;
    private float speedDown;        //  戻る時の速度

    // Start is called before the first frame update
    void Awake()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();

        playerPos = GameObject.Find("PlayerHandle");

        pi = GameObject.Find("PlayerHandle").GetComponent<PlayerInput>();

        playerCamera = GameObject.Find("Main Camera");

        wc = GameObject.Find("UsingWeapon").GetComponent<WeaponController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(0.0f, rotateSpeed, 0.0f);
        wc.backToHand = false;
        if (speed > 0.0f)
        {
            gameObject.transform.tag = "Weapon";

            rigidbody.position += playerCamera.transform.forward * speed * Time.fixedDeltaTime;     //  カメラの正方向に一定距離を移動する

            speed -= 1.5f;
        }
        else
        {
            gameObject.transform.tag = "Untagged";

            speedDown += 0.2f;

            transform.position = Vector3.Lerp(transform.position, playerPos.transform.position, speedDown * Time.fixedDeltaTime);     //  プレイヤーの位置に戻る
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.tag == "Player")
        {
            pi.canThrow = true;

            wc.backToHand = true;

            playerCamera.GetComponent<CameraController>().canThrowWeapon = true;

            Destroy(this.gameObject);
        }

        if (collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            speed = 0.0f;
        }
    }
}
