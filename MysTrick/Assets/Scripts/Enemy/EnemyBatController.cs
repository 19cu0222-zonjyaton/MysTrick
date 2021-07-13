//-------------------------------------------------
// ファイル名		：EnemyBatController.cs
// 概要				：バットエネミーの制御
// 作成者			：鍾家同
// 更新内容			：2021/06/28 作成
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

	[Header("===監視用===")]
	[SerializeField]
	private float rotateTimeCount = 0.0f;	// 回転経過時間
	[SerializeField]
	private bool canTurn = false;			// 回転フラグ
	[SerializeField]
	private bool canPreATK = false;			// 攻撃の下準備フラグ
	[SerializeField]
	private bool canATK = false;			// 攻撃フラグ
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
	private Vector3 returnPosition;			// 戻り位置
	private new Rigidbody rigidbody;
	private new Collider collider;
	private Animator animator;

	void Awake()
	{
		rigidbody = GetComponent<Rigidbody>();
		collider = GetComponent<Collider>();
		animator = GetComponent<Animator>();
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
				// 移動ルートが指定されてない場合、壁まで往復移動
				if (moveData.targetA == null || moveData.targetB == null)
				{
					// Ray(Vector3 (world)origin, Vector3 direction)
					Ray DWRay = new Ray(this.transform.position, -this.transform.up);			// 下向きにレイを描く
					Ray FWRay = new Ray(this.transform.position, this.transform.forward);		// 前向きにレイを描く
					RaycastHit DWHit;
					RaycastHit FWHit;

					// 攻撃の下準備状態、攻撃状態と回転状態ではないなら常にレイを放つ
					if (Physics.Raycast(DWRay, out DWHit, Mathf.Infinity) && !canPreATK && !canATK && !canTurn && !hitGround && !isDamage)
					{
						// 前に障害物があり、あるいは下に“壁”があれば真っ先に回転する
						if (Physics.Raycast(FWRay, out FWHit, 3.0f) || DWHit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
						{
							canTurn = true;
						}
						// 下にプレイヤーがあれば攻撃する
						else if (DWHit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
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
						//Debug.Log("Hit Collider: " + DWHit.collider.name);
						// レイがシーンに見られるようにする
						Debug.DrawLine(DWRay.origin, DWHit.point, Color.red);
					}
				}
				if (isDead) Dead();
				else if (isDamage && canHurt) Damage();
				else if (canTurn) Turn();
				else if (canRest) Rest();
				else if ((canPreATK || canATK) && !hitGround) Attack();

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
		Debug.Log("Straight");
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
		//Debug.Log("Turn");
		statement = Statement.General;
		// 回転時間開始
		rotateTimeCount += Time.deltaTime;
		// タイマーの時間切れすると、回転する
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
		//Debug.Log("Rest");
		if (canRest)
		{
			// 休憩開始
			if (attackData.restTimeCountMax >= 0.0f)
			{
				attackData.restTimeCountMax -= Time.deltaTime;
			}
			// 休憩停止
			else if (attackData.restTimeCountMax < 0.0f)
			{
				statement = Statement.Return;
				hitGround = false;
				rigidbody.useGravity = false;
				this.transform.position = Vector3.MoveTowards(this.transform.position, returnPosition, returnSpeed * Time.deltaTime);
			}
			// 初期値に戻る
			//if (Mathf.Abs(this.transform.position.magnitude - returnPosition.magnitude) < 0.01f)
			if (this.transform.position == returnPosition)
			{
				attackData.restTimeCountMax = attackData.restTimeCountReset;
				canRest = false;
			}
		}
	}

	// 攻撃
	void Attack()
	{
		//Debug.Log("Attack");
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
			this.transform.position = Vector3.MoveTowards(this.transform.position, attackData.atkPosition, attackData.atkSpeed * Time.deltaTime);
			//rigidbody.velocity = new Vector3(0.0f, -attackData.atkSpeed, 0.0f);
			//rigidbody.AddForce(0.0f, -attackData.atkSpeed / Time.deltaTime, 0.0f);
			//Debug.Log("Attacking!");
		}
	}

	// ダメージ受け
	void Damage()
	{
		//Debug.Log("Damage");
		// ダメージ受け開始
		if (isDamage)
		{
			if (hitGround)
			{
				damaging = true;
				rigidbody.useGravity = false;
			}

			if (damaging)
			{
				if (damageData.timeCountMax >= 0.0f && hitGround) damageData.timeCountMax -= Time.deltaTime;
				else if (damageData.timeCountMax <= 0.0f && this.transform.position != damageData.backPosition)
				{
					statement = Statement.Return;
					this.transform.position = Vector3.MoveTowards(
						this.transform.position, damageData.backPosition, returnSpeed * Time.deltaTime);
					isHurt = false;
				}
				else if (this.transform.position == damageData.backPosition)
				{
					isDamage = false;
					damageData.timeCountMax = damageData.timeCountReset;
					damaging = false;
					isHurt = false;
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
		Debug.Log("Death");
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

	void OnTriggerEnter(Collider other)
	{
		if (other.transform.tag == "Weapon")
		{
			if (timeController.isFinish && canHurt)
			{
				// タイマー起動（二重ダメージ受けを防ぐため）
				//timeController.TimeDelay(0.0f, 1.5f, true);

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

				// HPがある限り、ダメージ受けたら上方向に力をかけ且つ墜落
				if (!isDead)
				{
					rigidbody.AddForce(0.0f, damageData.damageForce, 0.0f);
					rigidbody.useGravity = true;
					damageData.timeCountMax = damageData.timeCountReset;
					isDamage = true;
					canRest = false;
				}
				// ダメージ状態でさらにダメージ受けるとフラグを立つ
				if (damaging) isHurt = true;
			}
		}
	}

	void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
		{
			hitGround = true;
			canATK = false;
			if(!isDamage) canRest = true;
		}
	}
	private void OnCollisionExit(Collision other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
		{
			hitGround = false;
		}
	}
}
