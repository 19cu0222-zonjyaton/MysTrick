using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageController : MonoBehaviour
{
    public bool isDamage;                   //  ダメージをを受けたフラグ
    public int enemyHp;                     //  エネミーのHP
    public bool canMove;                    //  プレイヤーに攻撃されたら時間内で -> false
    public Animator anim;                   //  アニメコントローラー
    private Rigidbody rigid;                //  プレイヤーに攻撃された処理用
    private CapsuleCollider capsuleCollider;//  衝突判定用

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

            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && anim.GetCurrentAnimatorStateInfo(0).IsName("Dead"))
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
            anim.SetTrigger("IsDamage");
            isDamage = true;
            canMove = false;
            rigid.AddForce(0.0f, 300.0f, 0.0f);
            rigid.AddExplosionForce(300.0f, collider.transform.position, 5.0f);
        }
    }
}
