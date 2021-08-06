using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageController : MonoBehaviour
{
    public bool isDamage;                   //  ダメージをを受けたフラグ
    public int enemyHp;                     //  エネミーのHP
    public bool canMove;                    //  プレイヤーに攻撃されたら時間内で -> false
    public bool hitWithPlayer;
    public Animator anim;                   //  アニメコントローラー
    public GameObject coin;                 //  コインオブジェクト
    public ActorController ac;              //  プレイヤーコントローラー

    private Rigidbody rigid;                //  プレイヤーに攻撃された処理用
    private CapsuleCollider capsuleCollider;//  衝突判定用
    private AudioSource au;              //  敵のSEオブジェクト
    private float timeCount;                //  コインを時間ごとにでる
    private int coinCount;                  //  コインの個数
    private float deadPosY;                 //  消滅されたY座標保存用

    //	初期化
    void Awake()
    {
        rigid = gameObject.GetComponent<Rigidbody>();

        capsuleCollider = gameObject.GetComponent<CapsuleCollider>();

        au = gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        //  プレイヤーに攻撃された処理
        if (isDamage)               
        {
            //  ダメージ動画が終わるまで二回目のダメージを受けられないにする
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && anim.GetCurrentAnimatorStateInfo(0).IsName("Damage"))   
            {
                isDamage = false;
                canMove = true;
            }
        }

        //  敵の死亡処理
        if (enemyHp <= 0)           
        {
            anim.SetBool("IsDead", true);
            capsuleCollider.isTrigger = true;
            capsuleCollider.tag = "Untagged";       //  Tagを無効化

            timeCount += Time.deltaTime;
            if (timeCount > 0.2f)                     
            {
                Instantiate(coin, new Vector3(transform.position.x, deadPosY, transform.position.z), Quaternion.identity);      //  消滅されたらコインを排除する
                ac.coinUIAction = true;
                ac.coinCount++;
                coinCount++;
                timeCount = 0.0f;
            }

            if (coinCount > 2)  //  敵01を消滅したら3枚のコインを獲得する
            {
                Destroy(this.gameObject);
            }
        }
    }

    //  敵の衝突処理
    void OnTriggerEnter(Collider collider)
    {
        //  プレイヤーの武器と当たる処理
        if (collider.transform.tag == "Weapon" && !isDamage)
        {
            enemyHp--;
            au.Play();

            if (enemyHp <= 0)   //  コインを排除するY座標を記録する
            {
                deadPosY = transform.position.y;
            }

            anim.SetTrigger("IsDamage");
            isDamage = true;
            canMove = false;
            rigid.AddExplosionForce(500.0f, collider.transform.position, 3.0f, 3.0f);
        }

        //  マップから落ちる処理
        if (collider.transform.tag == "DeadCheck")
        {
            ac.coinUIAction = true;
            ac.coinCount += 3;
            Destroy(this.gameObject);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            hitWithPlayer = true;

            canMove = false;

            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;           //	
        }
    }
}
