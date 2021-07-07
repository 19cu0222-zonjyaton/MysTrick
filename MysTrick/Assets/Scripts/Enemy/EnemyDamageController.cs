﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageController : MonoBehaviour
{
    public bool isDamage;                   //  ダメージをを受けたフラグ
    public int enemyHp;                     //  エネミーのHP
    public bool canMove;                    //  プレイヤーに攻撃されたら時間内で -> false
    public Animator anim;                   //  アニメコントローラー
    public GameObject coin;                 //  コインオブジェクト
    public ActorController ac;              //  プレイヤーコントローラー
    private Rigidbody rigid;                //  プレイヤーに攻撃された処理用
    private CapsuleCollider capsuleCollider;//  衝突判定用
    private float timeCount;                //  コインを時間ごとにでる
    private int coinCount;                  //  コインの個数
    private float deadPosY;                 //  消滅されたY座標保存用

    void Awake()
    {
        rigid = gameObject.GetComponent<Rigidbody>();

        capsuleCollider = gameObject.GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        if (isDamage)       //  プレイヤーに攻撃された処理
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && anim.GetCurrentAnimatorStateInfo(0).IsName("Damage"))   //  ダメージ動画が終わるまで二回目のダメージを受けられないにする
            {
                isDamage = false;
                canMove = true;
            }
        }

        if (enemyHp <= 0)           //  死亡処理
        {
            anim.SetBool("IsDead", true);
            capsuleCollider.isTrigger = true;
            capsuleCollider.tag = "Untagged";

            timeCount += Time.deltaTime;
            if (timeCount > 0.2f)
            {
                Instantiate(coin, new Vector3(transform.position.x, deadPosY, transform.position.z), Quaternion.identity);
                ac.coinUIAction = true;
                ac.coinCount++;
                coinCount++;
                timeCount = 0.0f;
            }

            if (coinCount > 2)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider collider)  //  衝突処理
    {
        if (collider.transform.tag == "Weapon" && !isDamage)
        {
            enemyHp--;

            if (enemyHp <= 0)
            {
                deadPosY = transform.position.y;
            }

            anim.SetTrigger("IsDamage");
            isDamage = true;
            canMove = false;
            //rigid.AddForce(0.0f, 300.0f, 0.0f);
            rigid.AddExplosionForce(500.0f, collider.transform.position, 3.0f, 3.0f);
        }

        if (collider.transform.tag == "DeadCheck")
        {
            ac.coinUIAction = true;
            ac.coinCount += 3;
            Destroy(this.gameObject);
        }
    }
}
