//-------------------------------------------------
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
	public GameObject model;
	public GameObject weapon;				//	武器
	public GameObject playerHand;			//  攻撃時武器の親オブジェクト
	public GameObject playerNeck;			//	通常時武器の親オブジェクト
	public SkinnedMeshRenderer modelMesh;	//	プレイヤーモデルのmesh
	public MeshRenderer weaponMesh;			//	武器のmesh
	public ClimbCheck climbCheck;			//	昇るチェック
	public PlayerInput pi;
	public int hp;							//	プレイヤーHP
	public int coinCount;					//	獲得したコイン数
	public bool coinUIAction;				//  コインUIを動くための信号
	public bool climbEnd;
	public bool isDead;                //	プレイヤーが死亡flag
	public bool isFall;                //	外に落ちるflag

	//============================
	// 作成者：鍾家同
	// Trigger用変数宣言
	public bool[] keys;
	public bool[] devices;
	//============================

	public float moveSpeed = 5.0f;
	public bool isInTrigger;
	public bool isUnrivaled;        //	無敵Time
	public bool shootStart;         //	武器発射flag
	public bool isJumping;			//	ジャンプflag
	public bool isClimbing;			//	登るflag

	[SerializeField]
	private Animator anim;
	private new Animation attack_anim;
	private Rigidbody rigid;
	private Vector3 movingVec;
	private GoalController gc;
	private int shortTimeCount;			//	点滅用タイムカウント
	private Vector3 weaponStartPos;		//	武器の位置保存用
	private Vector3 weaponStartRot;
	private Vector3 damageRot;			//	ダメージを受ける時の回転角度
	private float timeCount;
	private bool doOnce;

	// Start is called before the first frame update
	void Awake()
	{
		pi = GetComponent<PlayerInput>();

		anim = model.GetComponent<Animator>();

		attack_anim = GameObject.Find("R_Shoulder").GetComponent<Animation>();

		rigid = GetComponent<Rigidbody>();

		gc = GameObject.Find("Goal").GetComponent<GoalController>();

		weaponStartPos = weapon.transform.localPosition;

		weaponStartRot = weapon.transform.localEulerAngles;
	}

	// Update is called once per frame
	void Update()
	{
		anim.SetFloat("Forward", pi.Dmag);
		if (!isDead)
		{
			if (pi.Dmag > 0.1f && !pi.isAimStatus)      //	移動方向と移動速度を設定
			{
				model.transform.forward = Vector3.Slerp(model.transform.forward, pi.Dvec, 10.0f * Time.deltaTime);
				movingVec = pi.Dmag * model.transform.forward;
			}
			else if (pi.Dmag > 0.1f && pi.isAimStatus)
			{
				movingVec = pi.Dmag * pi.Dvec;
			}
			else
			{
				movingVec = pi.Dmag * model.transform.forward;
			}

			if (pi.isAttacking)                     //	近戦攻撃処理
			{
				attack_anim.Play();

				pi.isAttacking = false;
			}
			else if (pi.isThrowing && !pi.isAimStatus && !attack_anim.isPlaying)   //	第三視点の投げる処理
			{
				anim.SetTrigger("Throw");

				pi.canThrow = false;

				pi.isThrowing = false;
			}

			if (attack_anim.isPlaying)                //	攻撃する時tagを有効にする
			{
				weapon.transform.SetParent(playerHand.transform);
				weapon.transform.localPosition = new Vector3(-0.146f, 0.091f, 1.137f);
				weapon.transform.localEulerAngles = new Vector3(22.826f, -291.228f, 167.892f);
				weapon.transform.tag = "Weapon";
				doOnce = true;
			}
			else if (doOnce)
			{
				weapon.transform.SetParent(playerNeck.transform);
				weapon.transform.localPosition = weaponStartPos;                     //  親に相対の座標を設定する
				weapon.transform.localEulerAngles = weaponStartRot;                  //  親に相対の角度を設定する   
				weapon.transform.tag = "Untagged";
				doOnce = false;
			}

			if (isClimbing)                         //	登る処理
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

	private void checkIsUnderDamage()   //	敵と当たると時間内で無敵状態になる
	{
		if (isUnrivaled && !isDead)
		{
			timeCount += Time.deltaTime;
			
			if (timeCount >= 0.15f && timeCount < 0.3f)		//	点滅処理
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

			if (shortTimeCount >= 6 && timeCount >= 0.15f)                //	点滅が終わったら
			{
				shortTimeCount = 0;
				modelMesh.enabled = true;
				weaponMesh.enabled = true;
				isUnrivaled = false;
			}
			else if (shortTimeCount > 2) //	プレイヤー操作を解禁
			{
				pi.inputEnabled = true;
			}
		}
	}

	private void checkPlayerIsDead()	//	死亡処理
	{
		if (hp <= 0)
		{
			isDead = true;
		}

		if (isDead)
		{
			Time.timeScale = 0.4f;
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
			if (hp >= 0)
			{
				pi.inputEnabled = false;
				modelMesh.enabled = false;
				weaponMesh.enabled = false;
				if (hp == 0)
				{
					rigid.AddForce(0.0f, 200.0f, 0.0f);
				}
				else
				{
					rigid.AddForce(0.0f, 50.0f, 0.0f);
				}		
				rigid.AddExplosionForce(500.0f, collision.transform.position - new Vector3(0.0f, 1.5f, 0.0f), 5.0f, 3.0f);		//	爆発の位置を矯正
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
