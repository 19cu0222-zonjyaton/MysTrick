﻿//-------------------------------------------------
// ファイル名		：EnemyBatController.cs
// 概要				：バットエネミーの制御
// 作成者			：鍾家同
// 更新内容			：2021/06/28 作成
//					：2021/09/09 当たり判定の修正
//-------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBatController : MonoBehaviour
{
	// 各移動方法
	public enum Movement
	{
		Straight,		// 直線移動
		Spiral,			// 旋回移動
		None			// 移動なし
	}
	// 各状態
	public enum Statement
	{
		General,		// 通常
		Attack,			// 攻撃
		Return,			// 回復
		Damage,			// ダメージ受け
		Dead			// 死亡
	}

	[Header("===調整用===")]
	public Movement movement;		// 各移動方法
	[HideInInspector]
	public Statement statement;		// 各状態(アニメーション用)

	// 各移動データ
	[System.Serializable]
	public struct MoveData
	{
		[Tooltip("For StraightMovement. (Not necessary.)")]
		public Transform targetA;		// 指定位置A
		[Tooltip("For StraightMovement. (Not necessary.)")]
		public Transform targetB;		// 指定位置B
		[Tooltip("For StraightMovement.")]
		public bool moveAlongXAxle;		// X軸に沿って移動するフラグ
		[Tooltip("For StraightMovement.")]
		public bool moveAlongZAxle;		// Z軸に沿って移動するフラグ
		[Tooltip("For All.")]
		public float moveSpeed;			// 移動スピード
		[Tooltip("For All.")]
		public float rotateSpeed;		// 回転スピード
		[Tooltip("For All.")]
		public float rotateTimeMax;		// 回転所要時間
	}
	public MoveData moveData;

	// 各攻撃データ
	[System.Serializable]
	public struct AttackData
	{
		[Tooltip("The maximum time before attacking.")]
		public float preTimeCountMax;	// 攻撃前の最大時間
		[Tooltip("To Reset the maximun time after attacking.")]
		[HideInInspector]
		public float preTimeCountReset;	// 攻撃前の最初時間
		[Tooltip("The speed before attacking.")]
		public float preATKSpeed;		// 攻撃前のスピード
		[Tooltip("The attacking speed.")]
		public float atkSpeed;			// 攻撃スピード
		[Tooltip("The maximun time after attacking.")]
		public float restTimeCountMax;	// 攻撃完了の休憩最大時間
		[Tooltip("The maximun time after attacking.")]
		[HideInInspector]
		public float restTimeCountReset;// 攻撃完了の休憩最初時間
		[Tooltip("The attacking (world)position.")]
		[HideInInspector]
		public Vector3 atkPosition;		// 攻撃位置

		[Tooltip("The range for the attacking detection.")]
		public float detectRange;		// 攻撃の検索するの範囲  2022/01/25 - 林雲暉

	}
	public AttackData attackData;

	// 各ダメージ受けデータ
	[System.Serializable]
	public struct DamageData
	{
		public float timeCountMax;		// ダメージ受けてから回復するまで経過時間
		[HideInInspector]
		public float timeCountReset;	// 初期時間
		[HideInInspector]
		public Vector3 backPosition;	// 戻る位置
		public float damageForce;		// ダメージ受け力
	}
	public DamageData damageData;

	// 各死亡データ
	[System.Serializable]
	public struct DeathData
	{
		public float timeCountMax;		// 消失前経過時間
	}
	public DeathData deathData;

	public int health;						// HP
	public float returnSpeed;				// 戻るスピード
	public TimeController timeController;
	public SkinnedMeshRenderer mesh;
	public MeshRenderer bodypart1;
	public MeshRenderer bodypart2;
	public MeshRenderer bodypart3;
	public MeshRenderer bodypart4;
	public MeshRenderer bodypart5;
	public SkinnedMeshRenderer leftwing;
	public SkinnedMeshRenderer rightwing;
	public BoxCollider untriggeredCol;
	public BoxCollider triggeredCol;
	public GameObject stunHint;

	[Header("===監視用===")]
	[SerializeField]
	private float rotateTimeCount = 0.0f;	// 回転経過時間
	[SerializeField]
	private bool canTurn = false;			// 回転フラグ
	[SerializeField]
	private bool canPreATK = false;			// 攻撃の下準備フラグ
	[SerializeField]
	private bool canATK = false;				// 攻撃フラグ
	[SerializeField]
	private bool canRest = false;			// 休憩フラグ
	[SerializeField]
	private bool hitGround = false;			// 地に当たるかフラグ
	[SerializeField]
	private bool isDamage = false;			// ダメージ受けたかフラグ
	[SerializeField]
	private bool canHurt = true;			// ダメージ受けられるかフラグ
	[SerializeField]
	private bool damaging = false;			// ダメージ受けているかフラグ
	[SerializeField]
	private bool isHurt = false;			// ダメージ受けられているかフラグ
	[SerializeField]
	private bool isDead = false;			// 死亡フラグ

	private Vector3 curRotation;			// 現在位置
	[SerializeField]
	private Vector3 returnPosition;			// 戻り位置
	private new Rigidbody rigidbody;
	private new Collider collider;
	private Animator animator;
	private new AudioSource audio;			//	Audioコンポーネント
	private CameraController cameraController;
	private bool playOnce;					//	一回だけSEを流すフラグ
	private float unrivaledTime;			// 無敵時間

	void Awake()
	{
		rigidbody = GetComponent<Rigidbody>();
		collider = GetComponent<Collider>();
		animator = GetComponent<Animator>();
		audio = GetComponent<AudioSource>();
		cameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();
		unrivaledTime = 0.3f;
	}

	// 初期化
	void Start()
	{
		statement = Statement.General;
		attackData.preTimeCountReset = attackData.preTimeCountMax;
		attackData.restTimeCountReset = attackData.restTimeCountMax;
		damageData.timeCountReset = damageData.timeCountMax;

		// オブジェクトの向きを修正(必要であれば)
		if (movement == Movement.Straight)
		{
			if (moveData.moveAlongXAxle && !moveData.moveAlongZAxle && this.transform.forward.x != 1.0f) this.transform.forward = new Vector3(1.0f, 0.0f, 0.0f);
			else if (!moveData.moveAlongXAxle && moveData.moveAlongZAxle && this.transform.forward.z != 1.0f) this.transform.forward = new Vector3(0.0f, 0.0f, 1.0f);
			else if ((moveData.moveAlongXAxle && moveData.moveAlongZAxle) || (moveData.moveAlongXAxle && moveData.moveAlongZAxle)) 
				Debug.Log("Please check the flag either \"MoveAlongXAxle\" or \"MoveAlongZAxle\" by the enemy name: " + this.gameObject.name);
		}
	}

	void Update()
	{
		switch (movement)
		{
			case Movement.Straight:
				if (isDead) Dead();
				else if (isDamage)
				{
					Damage();
					Unrivaled();
				}
				else if (canTurn) Turn();
				else if (canRest) Rest();
				else if ((canPreATK || canATK) && !hitGround) Attack();
				else
				{
					bodypart1.enabled = true;
					bodypart2.enabled = true;
					bodypart3.enabled = true;
					bodypart4.enabled = true;
					bodypart5.enabled = true;
					leftwing.enabled = true;
					rightwing.enabled = true;

					// 移動ルートが指定されてない場合、壁まで往復移動
					if (moveData.targetA == null || moveData.targetB == null)
					{
						// Ray(Vector3 (world)origin, Vector3 direction)
						Ray DWRay = new Ray(this.transform.position, -this.transform.up);           // 下向きにレイを描く
						Ray FWRay = new Ray(this.transform.position, this.transform.forward);       // 前向きにレイを描く
						RaycastHit FWHit;
						RaycastHit boxHitInfo;
						int layerInfo = 1 << 11 | 1 << 25 | 1 << 8;                                 // 検索するレイヤーを指定する  2022/01/25 - 林雲暉

						// 前のRaycastのバージョン
						// RaycastHit DWHit;
						// Physics.Raycast(DWRay, out DWHit, Mathf.Infinity) 

						// Debug用　衝突をチェック  2022/01/25 - 林雲暉
						// Debug.Log(":" + Physics.BoxCast(this.transform.position, new Vector3(attackData.detectRange, 3.0f, attackData.detectRange), -this.transform.up, out boxHitInfo, new Quaternion(), 15.0f, layerInfo));

						// 攻撃の下準備状態、攻撃状態と回転状態ではないなら常にレイを放つ
						// BoxCastに変更、PrefabのdetectRangeで検索範囲を指定できます  2022/01/25 - 林雲暉
						if (Physics.BoxCast(this.transform.position, new Vector3(attackData.detectRange, 3.0f, attackData.detectRange), -this.transform.up, out boxHitInfo, this.transform.rotation, 15.0f, layerInfo)
							/*&& !canPreATK && !canATK && !canTurn && !hitGround && !isDamage && !isDead && !canRest*/)
						{
							//Debug.Log("StraightFunction");
							// 前に障害物があり、あるいは下に“壁”があれば真っ先に回転する
							if (Physics.Raycast(FWRay, out FWHit, 3.0f) || boxHitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Fence"))
							{
								canTurn = true;
							}
							// 下にプレイヤーがあれば攻撃する
							else if (boxHitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Player") && cameraController.cameraStatic == "Idle")
							{
								canPreATK = true;
								returnPosition = this.transform.position;
								attackData.atkPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
							}
							// 下に指定されてないオブジェクトがあれば移動する
							else
							{
								if (!canTurn)
								{
									//returnPosition = this.transform.position;
									damageData.backPosition = this.transform.position;
									curRotation = this.transform.rotation.eulerAngles;
									Straight();
								}
							}
							// Debug.Log("Hit Collider: " + boxHitInfo.collider.name);
							// レイがシーンに見られるようにする
							Debug.DrawLine(this.transform.position, boxHitInfo.point, Color.red);
						}
					}
				}

				AnimStatement();
				break;
			case Movement.Spiral:
				Spiral();
				break;
			case Movement.None:
				Rest();
				break;
			default:
				break;
		}
	}

	// 直線往復移動
	void Straight()
	{
		statement = Statement.General;
		this.transform.Translate(Vector3.forward * Time.deltaTime * moveData.moveSpeed);
	}

	// 旋回して移動
	void Spiral()
	{
		statement = Statement.General;
	}

	// 回転する
	void Turn()
	{
		statement = Statement.General;
		// 回転時間開始
		rotateTimeCount += Time.deltaTime;
		// タイマーの時間切れすると、回転する
		// +0.5　回転待つ用　2022/01/25 - 林雲暉
		if (rotateTimeCount <= moveData.rotateTimeMax)
		{
			//this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(0.0f, 180.0f, 0.0f), Time.deltaTime * 10.0f);
			this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.Euler(curRotation + new Vector3(0.0f, 180.0f, 0.0f)), Time.deltaTime * moveData.rotateSpeed);
		}
		// 初期値に戻る
		else
		{
			rotateTimeCount = 0.0f;
			canTurn = false;
			Straight();
		}
	}

	// 休憩
	void Rest()
	{
		//Debug.Log("RestFunction");
		if (canRest)
		{
			// 休憩開始
			if (attackData.restTimeCountMax >= 0.0f)
			{
				attackData.restTimeCountMax -= Time.deltaTime;
				canHurt = true;
			}
			// 休憩停止
			else if (attackData.restTimeCountMax < 0.0f)
			{
				statement = Statement.Return;
				hitGround = false;
				rigidbody.useGravity = false;
				//canHurt = true;
				this.transform.position = Vector3.MoveTowards(this.transform.position, returnPosition, returnSpeed * Time.deltaTime);
			}
			// 初期値に戻る
			//if (Mathf.Abs(this.transform.position.magnitude - returnPosition.magnitude) < 0.01f)
			if (this.transform.position == returnPosition)
			{
				attackData.restTimeCountMax = attackData.restTimeCountReset;
				canRest = false;
				canHurt = true;
			}
		}
	}

	// 攻撃
	void Attack()
	{
		//Debug.Log("AttackFunction");
		rigidbody.isKinematic = true;
		canHurt = false;
		statement = Statement.Attack;
		if (canPreATK && attackData.preTimeCountMax > 0.0f)
		{
			// 攻撃の下準備時間開始
			attackData.preTimeCountMax -= Time.deltaTime;
			//this.transform.Translate(-Vector3.forward * Time.deltaTime * attackData.preATKSpeed);
			this.transform.Translate(Vector3.up * Time.deltaTime * attackData.preATKSpeed);
		}
		// 初期値に戻る
		else if (attackData.preTimeCountMax < 0.0f)
		{
			canPreATK = false;
			canATK = true;
			attackData.preTimeCountMax = attackData.preTimeCountReset;
		}
		// 攻撃開始
		if (canATK)
		{
			// 攻撃状態に切り替える
			statement = Statement.Attack;
			this.transform.position = Vector3.MoveTowards(this.transform.position, attackData.atkPosition + new Vector3(0.0f, 2.0f, 0.0f), attackData.atkSpeed * Time.deltaTime);
			//rigidbody.velocity = new Vector3(0.0f, -attackData.atkSpeed, 0.0f);
			//rigidbody.AddForce(0.0f, -attackData.atkSpeed / Time.deltaTime, 0.0f);
			if(this.transform.position== attackData.atkPosition + new Vector3(0.0f, 2.0f, 0.0f))
			{
				canATK = false;
				canRest = true;
			}
		}
	}

	// ダメージ受け
	void Damage()
	{
		canRest = false;
		canTurn = false;
		canPreATK = false;
		canATK = false;

		// ダメージ受け開始
		if (isDamage)
		{
			//Debug.Log("DamageFunction");
			stunHint.SetActive(true);
			if (hitGround)
			{
				damaging = true;
				rigidbody.useGravity = false;
				canHurt = false;
			}

			if (damaging)
			{
				if (damageData.timeCountMax >= 0.0f && hitGround)
				{
					damageData.timeCountMax -= Time.deltaTime;
				}
				// ダメージ受けた且元の位置に戻していない場合
				else if (damageData.timeCountMax <= 0.0f && this.transform.position != damageData.backPosition)
				{
					statement = Statement.Return;
					// 元の位置に戻す
					this.transform.position = Vector3.MoveTowards(
						this.transform.position, damageData.backPosition, returnSpeed * Time.deltaTime);
					isHurt = false;
					stunHint.SetActive(false);
				}
				// 元の位置に戻した場合
				else if (this.transform.position == damageData.backPosition)
				{
					damageData.timeCountMax = damageData.timeCountReset;
					attackData.restTimeCountMax = attackData.restTimeCountReset;
					isDamage = false;
					damaging = false;
					isHurt = false;
					canHurt = true;
					stunHint.SetActive(false);
				}
				if (isHurt) rigidbody.useGravity = true;
				else rigidbody.useGravity = false;
			}
		}
	}

	// 死亡
	void Dead()
	{
		statement = Statement.Dead;
		collider.enabled = false;
		if (deathData.timeCountMax >= 0.0f) deathData.timeCountMax -= Time.deltaTime;
		else Destroy(this.gameObject);
		//Debug.Log("Death");
	}

	// 無敵時間
	void Unrivaled()
	{
		unrivaledTime -= Time.deltaTime;
		if (unrivaledTime < 0.3f && unrivaledTime > 0.15f)
		{
			bodypart1.enabled = false;
			bodypart2.enabled = false;
			bodypart3.enabled = false;
			bodypart4.enabled = false;
			bodypart5.enabled = false;
			leftwing.enabled = false;
			rightwing.enabled = false;
		}
		else if (unrivaledTime > 0.0f)
		{
			bodypart1.enabled = true;
			bodypart2.enabled = true;
			bodypart3.enabled = true;
			bodypart4.enabled = true;
			bodypart5.enabled = true;
			leftwing.enabled = true;
			rightwing.enabled = true;
		}
		else
		{
			unrivaledTime = 0.3f;
		}


	}

	// アニメーション制御
	void AnimStatement()
	{
		switch (statement)
		{
			// 通常状態（飛行）
			case Statement.General:
				animator.SetBool("Move", true);
				animator.SetBool("PreAttack", false);
				animator.SetBool("Attack", false);
				animator.SetBool("Hurt", false);
				animator.SetBool("Return", false);
				break;
			// 攻撃状態
			case Statement.Attack:
				if (canPreATK)
				{
					animator.SetBool("PreAttack", true);
					animator.SetBool("Attack", false);
				}
				else if (canATK)
				{
					animator.SetBool("Attack", true);
					animator.SetBool("PreAttack", false);
				}				
				animator.SetBool("Move", false);
				animator.SetBool("Hurt", false);
				animator.SetBool("Return", false);
				break;
			case Statement.Return:
				animator.SetBool("Return", true);
				animator.SetBool("Move", false);
				animator.SetBool("PreAttack", false);
				animator.SetBool("Attack", false);
				animator.SetBool("Hurt", false);
				break;
			// ダメージ受け状態
			case Statement.Damage:
				animator.SetBool("Hurt", true);
				animator.SetBool("Move", false);
				animator.SetBool("Attack", false);
				animator.SetBool("Return", false);
				break;
			// 死亡状態
			case Statement.Dead:
				animator.SetTrigger("Dead");
				animator.SetBool("Move", false);
				animator.SetBool("Attack", false);
				animator.SetBool("Hurt", false);
				animator.SetBool("Return", false);
				break;
			default:
				Debug.LogWarning("There is no statement in the enemy name: " + this.gameObject.name);
				break;
		}
	}

	// 当たり判定
	//----------------------------------
	void OnTriggerEnter(Collider other)
	{
		if (other.transform.tag == "Weapon" || other.transform.tag == "Slash1")
		{
			rigidbody.isKinematic = false;

			if (timeController.isFinish && canHurt)
			{
				// タイマー起動（二重ダメージ受けを防ぐため）
				timeController.TimeDelay(0.0f, 1.5f);

				// 音を出す
				/*if (!playOnce)
				{
					audio.Play();
					playOnce = true;
				}*/
				audio.Play();

				// HP計算
				if (health > 1)
				{
					statement = Statement.Damage;
					health -= 1;
				}
				else
				{
					statement = Statement.Dead;
					isDead = true;
					health = 0;
				}
				// Debug.Log("DamageCount");

				// HPがある限り、ダメージ受けたら上方向に力をかけ且つ墜落
				if (!isDead)
				{
					rigidbody.useGravity = true;
					rigidbody.AddForce(0.0f, damageData.damageForce, 0.0f);
					damageData.timeCountMax = damageData.timeCountReset;
					isDamage = true;
				}
				// ダメージ状態でさらにダメージ受けるとフラグを立つ
				if (damaging) isHurt = true;

				//canHurt = false;
			}
		}
		if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
		{
			hitGround = true;
			canATK = false;
			if (!isDamage) canRest = true;
		}
	}
	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.layer == LayerMask.NameToLayer("Fence"))
		{
			hitGround = false;
		}
	}
	void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.layer == LayerMask.NameToLayer("Fence"))
		{
			hitGround = true;
			canATK = false;
		}
	}
	/*void OnCollisionExit(Collision other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
		{
			hitGround = false;
		}
	}*/
	//----------------------------------
}
