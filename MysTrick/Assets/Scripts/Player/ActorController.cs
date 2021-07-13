﻿//-------------------------------------------------
// ファイル名		：ActorController.cs
// 概要				：
// 作成者			：曹飛
// 更新内容			：2020/04/02 作成
//					；2020/04/11 変数増加(keys,devices)
//-------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    public GameObject model;                //	モデルオブジェクト
    public GameObject weapon;               //	武器オブジェクト
    public GameObject playerHand;           //  攻撃時武器の親オブジェクト
    public GameObject playerNeck;           //	通常時武器の親オブジェクト
    public SkinnedMeshRenderer modelMesh;   //	プレイヤーモデルのmesh
    public MeshRenderer weaponMesh;         //	武器のmesh
    public ClimbCheck climbCheck;           //	昇るチェック
    public PlayerInput pi;                  //	入力コントローラー
    public AudioClip[] sounds;              //	SEオブジェクト
    public int hp;                          //	プレイヤーHP
    public int coinCount;                   //	獲得したコイン数
    public bool coinUIAction;               //  コインUIを動くための信号
    public bool climbEnd;                   //	登るエンドフラグ
    public bool isDead;                     //	プレイヤーが死亡flag
    public bool isFall;                     //	外に落ちるflag

    // 仕掛け用変数宣言
    public bool[] keys;
    public bool[] devices;

    public float moveSpeed = 5.0f;          //	移動スピード
    public bool isInTrigger;                //	仕掛けスイッチを当たるフラグ
    public bool isUnrivaled;                //	無敵Time
    public bool shootStart;                 //	武器発射flag
    public bool isJumping;                  //	ジャンプflag
    public bool isClimbing;                 //	登るflag

    private new AudioSource audio;          //	SEのコンポーネント		
    private Animator anim;                  //	アニメコントローラーコンポーネント
    private Animation attack_anim;          //	アニメーションコントローラー
    private Rigidbody rigid;                //	鋼体コンポーネント
	private Vector3 movingVec;              //	移動方向
    private GoalController gc;              //	ゴールコントローラー
    private int shortTimeCount;             //	点滅用タイムカウント
    private Vector3 weaponStartPos;         //	武器の初期位置座標保存用
    private Vector3 weaponStartRot;         //	武器の初期回転角度保存用
    private Vector3 weaponAttackPos = new Vector3(-0.146f, 0.091f, 1.137f);             //	武器攻撃する時位置座標保存用
    private Vector3 weaponAttackRot = new Vector3(22.826f, -291.228f, 167.892f);        //	武器攻撃する時回転角度保存用
    private Vector3 damageRot;              //	ダメージを受ける時の回転角度
    private float timeCount;                //	タイムカウント
    private bool doOnce;                    //	一回だけ実行するため使うフラグ

    //	初期化
    void Awake()
	{
		pi = GetComponent<PlayerInput>();

		audio = gameObject.GetComponent<AudioSource>();

		anim = model.GetComponent<Animator>();

		attack_anim = GameObject.Find("R_Shoulder").GetComponent<Animation>();

		rigid = GetComponent<Rigidbody>();

		gc = GameObject.Find("Goal").GetComponent<GoalController>();

		weaponStartPos = weapon.transform.localPosition;

		weaponStartRot = weapon.transform.localEulerAngles;
	}

	void Update()
	{
		//	歩くアニメーションの数値
		anim.SetFloat("Forward", pi.Dmag);

		if (!isDead)
		{			
			if (pi.Dmag > 0.1f && !pi.isAimStatus)		//	1.移動の入力値が0.1を超える時	2.狙う状態ではない時	->	 移動方向を設定する
			{
				model.transform.forward = Vector3.Slerp(model.transform.forward, pi.Dvec, 10.0f * Time.deltaTime);
				movingVec = pi.Dmag * model.transform.forward; 
			}
			else if (pi.Dmag > 0.1f && pi.isAimStatus)  //	1.移動の入力値が0.1を超える時	2.狙う状態の時	->	 移動方向を設定する
			{
				movingVec = pi.Dmag * pi.Dvec;
			}
			else										//	以外の状態時
			{
				movingVec = pi.Dmag * model.transform.forward;
			}

			//	近戦攻撃処理
			if (pi.isAttacking)							
			{
				attack_anim.Play();                     //	近戦アニメを流す

				if (!audio.isPlaying)					//	SEを流してない時
				{
					audio.pitch = 2.0f;					//	音の大きさを調整
					audio.PlayOneShot(sounds[0]);       //	近戦SEを流す
				}

				pi.isAttacking = false;
			}
			//	第三視点武器を投げる処理
			else if (pi.isThrowing && !pi.isAimStatus && !attack_anim.isPlaying)   
			{
				anim.SetTrigger("Throw");

				audio.PlayOneShot(sounds[1]);

				pi.canThrow = false;					//	武器を手に戻るまで投げれない設定

				pi.isThrowing = false;					
			}

			if (attack_anim.isPlaying)					//	攻撃する時tagを有効にする
			{				
				weapon.transform.SetParent(playerHand.transform);
				//	武器の位置調整
				weapon.transform.localPosition = weaponAttackPos;
				weapon.transform.localEulerAngles = weaponAttackRot;
				weapon.transform.tag = "Weapon";
				doOnce = true;
			}
			else if (doOnce)							//	一回だけ実行する
			{
				weapon.transform.SetParent(playerNeck.transform);
				//	武器の位置を初期に戻る
				weapon.transform.localPosition = weaponStartPos;
				weapon.transform.localEulerAngles = weaponStartRot;  
				weapon.transform.tag = "Untagged";
				audio.pitch = 1.0f;
				doOnce = false;
			}

			if (isClimbing)                         //	梯子を登る処理
			{
				anim.SetBool("Climb", true);
			}
			else
			{
				anim.SetBool("Climb", false);
			}

		}

		checkIsUnderDamage();

		checkPlayerIsDead();
    }

	//	移動処理
	void FixedUpdate()
	{
		rigid.position += movingVec * moveSpeed * Time.fixedDeltaTime;
		
		if (gc.gameClear)		//	クリア処理
		{
			timeCount -= Time.fixedDeltaTime;
			if (timeCount < 0.0f)
			{
				rigid.AddForce(0, 500.0f, 0);
				timeCount = 2.0f;
			}
		}
	}

	private void checkIsUnderDamage()   //	敵と当たると時間内に無敵状態になる
	{
		if (isUnrivaled && !isDead)
		{
			timeCount += Time.deltaTime;
			
			if (timeCount >= 0.15f && timeCount < 0.3f)		//	メッシュの点滅処理
			{
				modelMesh.enabled = true;
				weaponMesh.enabled = true;
			}
			else if (timeCount >= 0.3f)
			{
				modelMesh.enabled = false;
				weaponMesh.enabled = false;
				timeCount = 0.0f;
				shortTimeCount++;
			}

			if (shortTimeCount >= 6 && timeCount >= 0.15f)	//	点滅が終わったら
			{
				shortTimeCount = 0;
				modelMesh.enabled = true;
				weaponMesh.enabled = true;
				isUnrivaled = false;
			}
			else if (shortTimeCount > 2)					//	プレイヤー入力可能
			{
				pi.inputEnabled = true;
			}
		}
	}

	//	死亡処理
	private void checkPlayerIsDead()	
	{
		if (hp <= 0)
		{
			isDead = true;
		}

		if (isDead)
		{
			Time.timeScale = 0.4f;		//	時間の流すを遅くなるように
			movingVec = Vector3.zero;	
			anim.enabled = false;
			pi.inputEnabled = false;
			model.transform.localRotation = Quaternion.Lerp(model.transform.localRotation, Quaternion.Euler(-90.0f, damageRot.y, damageRot.z), 3.0f * Time.deltaTime);
			transform.tag = "Untagged";
		}
	}

	private void OnTriggerStay(Collider collider)
	{
		if (collider.transform.tag == "Device")		
		{
			isInTrigger = true;
		}

		if (collider.transform.tag == "DeadCheck")
		{
			isFall = true;
		}

		if (climbCheck != null)		//	回転できる梯子の始点と終点処理
		{
			if ((climbCheck.nowLayer == 1 || climbCheck.nowLayer == 3) && collider.transform.name == "ClimbStart1")
			{
				climbEnd = true;
			}
			else if(climbCheck.nowLayer == 2 && collider.transform.name == "ClimbFinish1")
			{
				climbEnd = true;
			}
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (collider.transform.tag == "Device")
		{
			isInTrigger = false;
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		if (collision.transform.tag == "Enemy" && !isUnrivaled)     //	敵と当たる処理
		{
			hp--;
			damageRot = model.transform.localEulerAngles;
			audio.PlayOneShot(sounds[2]);
			
			if (hp >= 0)
			{
				pi.inputEnabled = false;
				modelMesh.enabled = false;
				weaponMesh.enabled = false;
				if (hp == 0)
				{
					rigid.AddExplosionForce(800.0f, collision.transform.position - new Vector3(0.0f, 1.5f, 0.0f), 5.0f, 2.0f);      //	爆発の位置を矯正
				}
				else
				{
					rigid.AddExplosionForce(500.0f, collision.transform.position - new Vector3(0.0f, 1.5f, 0.0f), 5.0f, 1.5f);      //	爆発の位置を矯正
				}		

				isUnrivaled = true;
			}
		}

		if (collision.transform.tag == "Device" || collision.transform.tag == "Key")
		{
			isInTrigger = true;
		}
	}
	private void OnCollisionExit(Collision collision)
	{
		if (collision.transform.tag == "Device" || collision.transform.tag == "Key")
		{
			isInTrigger = false;
		}
	}
}
