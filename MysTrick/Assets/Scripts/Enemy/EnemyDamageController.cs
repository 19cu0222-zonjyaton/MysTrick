using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageController : MonoBehaviour
{
    public bool isDamage;                   //  ダメージをを受けたフラグ
    public int enemyHp;                     //  エネミーのHP
    public bool canMove;                    //  プレイヤーに攻撃されたら時間内で -> false
    public bool hitWithPlayer;
    public bool isStun;
    public Animator anim;                   //  アニメコントローラー
    public GameObject coin;                 //  コインオブジェクト
    public ActorController ac;              //  プレイヤーコントローラー
    public GameObject stunStar;
    public AudioSource[] au;                 //  敵のSEオブジェクト

    private Rigidbody rigid;                //  プレイヤーに攻撃された処理用
    private CapsuleCollider capsuleCollider;//  衝突判定用
    private float timeCount;                //  コインを時間ごとにでる
    private int coinCount;                  //  コインの個数
    private float deadPosY;                 //  消滅されたY座標保存用

    //	初期化
    void Awake()
    {
        rigid = gameObject.GetComponent<Rigidbody>();

        capsuleCollider = gameObject.GetComponent<CapsuleCollider>();
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

        if (isStun)
        {
            timeCount += Time.deltaTime;
            if (timeCount >= 2.0f)
            {
                timeCount = 0.0f;
                stunStar.gameObject.SetActive(false);
                isStun = false;
                canMove = true;
            }
        }

        //  敵の死亡処理
        if (enemyHp <= 0)           
        {
            anim.SetBool("IsDead", true);
            capsuleCollider.isTrigger = true;
            timeCount += Time.deltaTime;
            if (coinCount < 3 && timeCount > 0.3f)                     
            {
                Instantiate(coin, new Vector3(transform.position.x, deadPosY, transform.position.z), Quaternion.identity);      //  消滅されたらコインを排除する
                ac.coinUIAction = true;
                ac.coinCount++;
                coinCount++;
                timeCount = 0.0f;
            }

            if (coinCount > 2.0f)  //  敵01を消滅したら3枚のコインを獲得する
            {
                Destroy(this.gameObject);
            }
        }
    }

    //  敵の衝突処理
    void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.tag == "Slash1" && enemyHp > 0)
        {
            enemyHp--;
            au[0].Play();

            if (enemyHp <= 0)   //  コインを排除するY座標を記録する
            {
                deadPosY = transform.position.y;
                rigid.AddForce(0, 700.0f, 0);
                stunStar.gameObject.SetActive(false);
                timeCount = 0.0f;
            }
            else if(!isStun)
            {
                au[1].Play();
                stunStar.gameObject.SetActive(true);
                isStun = true;
            }   
            anim.SetTrigger("IsDamage");
            canMove = false;
        }

        //  プレイヤーの武器と当たる処理
        if (collider.transform.tag == "Weapon" && enemyHp > 0)
        {
            enemyHp--;
            au[0].Play();

            if (enemyHp <= 0)   //  コインを排除するY座標を記録する
            {
                deadPosY = transform.position.y;
                rigid.AddForce(0, 700.0f, 0);
            }
            else
            {
                rigid.AddExplosionForce(600.0f, collider.transform.position, 3.0f, 3.0f);
            }
            timeCount = 0.0f;
            anim.SetTrigger("IsDamage");
            stunStar.gameObject.SetActive(false);
            isStun = false;
            isDamage = true;
            canMove = false;
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
        if (collision.transform.tag == "Player" && enemyHp > 0)
        {
            hitWithPlayer = true;

            canMove = false;

            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;           //	
        }
    }
}
