using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathControl : MonoBehaviour
{
    public float speed;                 //  敵の移動スピード  
    public float field;                 //  敵の捜査範囲
    public GameObject player;           //  プレイヤーオブジェクト
    public Transform[] pathPositions;   //  敵の移動ルート
    public Transform head;              //  敵の正方向を取るためのオブジェクト
    public EnemyDamageController edc;   //  プレイヤーのカメラコントローラー
    public GameObject[] patrolPos;
    public GameObject warning;
    public AudioClip sound;				//	SEオブジェクト

    private PlayerInput pi;             //  プレイヤーの入力コントローラー
    private CameraController cc;
    private AudioSource au;
    private Ray ray;                    //  正方向から発射する光線(プレイヤーが捜査範囲に入っても壁に遮ったら元のルートに戻るように使う)
    private RaycastHit hit;             //  光線にヒットしたオブジェクト
    private Vector3 targetPosition;
    private bool backToPatrol;
    private int index = 0;              //  今向けて移動する位置
    private bool islock = false;        //  プレイヤーが捜査範囲に入るフラグ
    private bool isAttackedByPlayer;    //  プレイヤーに攻撃されたフラグ
    private float timeCount;
    private bool hitWithWall;
    private bool warningActive;

    //	初期化
    void Awake()
    {
        if (player != null)
        {
            pi = player.GetComponent<PlayerInput>();

            cc = GameObject.Find("Main Camera").GetComponent<CameraController>();
        }

        au = gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        //  プレイヤーに攻撃された処理
        if (edc.isDamage)
        {
            isAttackedByPlayer = true;
        }

        if (pi != null)
        {
            if (cc.cameraStatic == "Idle" && !edc.isDamage && !warning.GetComponent<Animation>().isPlaying)
            {
                edc.canMove = true;
            }

            if (!LockOn() && !isAttackedByPlayer && !edc.hitWithPlayer)
            {
                warningActive = false;
            }
            else if ((LockOn() || isAttackedByPlayer || edc.hitWithPlayer) && !warningActive && (edc.enemyHp > 0))
            {
                au.PlayOneShot(sound);
                edc.canMove = false;
                warning.SetActive(true);
                warningActive = true;
            }
            
            if (!warning.GetComponent<Animation>().isPlaying)
            {
                warning.SetActive(false);
            }

            if (!backToPatrol)
            {
                //  プレイヤーが捜査範囲に入った光線を出せる処理
                if (LockOn() || isAttackedByPlayer || edc.hitWithPlayer)
                {
                    ray = new Ray(transform.position - new Vector3(0.5f, 0, 0), ((player.transform.position + new Vector3(0, 1.5f, 0)) - head.transform.position).normalized);
                }
                else
                {
                    ray = new Ray(transform.position, head.forward);
                }
                Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 0.1f);
                Physics.Raycast(ray, out hit, Mathf.Infinity);
            }
            else
            {
                for (int i = 0; i < patrolPos.Length; i++)
                {
                    ray = new Ray(transform.position, (patrolPos[i].transform.position - transform.position).normalized);
                    Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 0.1f);
                    Physics.Raycast(ray, out hit, Mathf.Infinity);
                    if (hit.collider.gameObject == patrolPos[i])
                    {
                        index = i;

                        backToPatrol = false;
                    }
                }
            }

            //  敵のAI処理
            if (edc.canMove && cc.cameraStatic == "Idle")
            {
                if (hit.collider != null)       //  光線が何も当たっていない時の対策
                {
                    hit.collider.enabled = true;

                    //  1.光線が壁に当たってない   2.捜査範囲に入った  3.攻撃された  -> プレイヤーの位置に移動する
                    if ((!(hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall")) && LockOn()) || isAttackedByPlayer || edc.hitWithPlayer)
                    {
                        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
                        hitWithWall = true;

                        Move();
                        //  光線が壁に当たったら攻撃AIをキャンセル
                        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
                        {
                            timeCount += Time.deltaTime;

                            //  timeCount -> 敵が大幅に回転する時でhitが壁に当たる防止ため
                            if (timeCount >= 0.5f)
                            {
                                isAttackedByPlayer = false;

                                edc.hitWithPlayer = false;

                                backToPatrol = true;

                                timeCount = 0.0f;
                            }
                        }
                        else
                        {
                            hitWithWall = false;
                            timeCount = 0.0f;
                        }
                    }
                    else
                    {
                        Patrol();
                    }
                }
                else if (edc.canMove && !isAttackedByPlayer && !edc.hitWithPlayer)
                {
                    Patrol();
                }
            }
        }
        else        //  タイトル画面
        {
            Patrol();
        }

    }

    //  プレイヤーを捜査範囲を設定する処理
    bool LockOn()
    {
        //  Y軸の捜査範囲
        if (System.Math.Abs(player.transform.position.y - transform.position.y) >= 2.0f && edc.canMove)
        {
            islock = false;

            isAttackedByPlayer = false;

            edc.hitWithPlayer = false;
        }
        else
        {
            Vector3 targetPosition = player.transform.position;
            if (Vector3.Distance(targetPosition, transform.position) >= field)
                islock = false;
            else
            {
                float angle = 0.0f;
                float headAngle;
                float oppositeAngle = 0.0f;
                headAngle = head.transform.localEulerAngles.y;

                //  捜査範囲を設定する
                if (targetPosition.z < transform.position.z)
                {
                    oppositeAngle = Mathf.Acos((targetPosition.x - transform.position.x) / Vector3.Distance(targetPosition, transform.position + new Vector3(0, 1.5f, 0))) * Mathf.Rad2Deg;
                }
                else if (targetPosition.z > transform.position.z)
                {
                    oppositeAngle = -Mathf.Acos((targetPosition.x - transform.position.x) / Vector3.Distance(targetPosition, transform.position + new Vector3(0, 1.5f, 0))) * Mathf.Rad2Deg;
                }

                if (oppositeAngle <= 0 && oppositeAngle >= -180.0f)
                {
                    oppositeAngle = 360 + oppositeAngle;
                }

                angle = oppositeAngle - headAngle + 90.0f;

                if ((angle <= 0.0f && angle >= -60.0f || angle <= 60.0f && angle > 0.0f || angle <= 360.0f && angle >= 300.0f && !hitWithWall) || isAttackedByPlayer || edc.hitWithPlayer)
                {
                    islock = true;
                }
                else
                {
                    islock = false;
                }
            }
        }
        return islock;
    }

    //  ルートに沿って移動処理
    void Patrol()
    {
        if (player == null)
        {
            transform.Translate((pathPositions[index].localPosition - transform.localPosition).normalized * Time.deltaTime * speed);
        }
        else
        {
            transform.Translate((pathPositions[index].position - transform.position).normalized * Time.deltaTime * speed);
        }

        targetPosition = pathPositions[index].transform.position;
        targetPosition.y = head.position.y;
        head.LookAt(targetPosition);
    }

    //  プレイヤーに移動する処理
    void Move()
    {
        transform.Translate((player.transform.position - transform.position).normalized * Time.deltaTime * speed);
        transform.eulerAngles = (player.transform.localPosition - transform.localPosition).normalized;
        targetPosition = player.transform.position;
        targetPosition.y = head.position.y;
        head.LookAt(targetPosition);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.tag == "Enemy_Ghost")
        {
            index++;
            //  次の点に移動する
            if (index > pathPositions.Length - 1)
                index = 0;
        }
    }
}

