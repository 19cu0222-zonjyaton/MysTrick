//-------------------------------------------------
// ファイル名		：ActorController.cs
// 概要				：
// 作成者			：曹飛
// 更新内容			：2020/04/02 作成
//					；2020/07/19 更新　鍾家同　構造体宣言(鍵付き扉を開くため)
//-------------------------------------------------

using UnityEngine;

public class ActorController : MonoBehaviour
{
	public GameObject model;				//	モデルオブジェクト
	public GameObject weapon;				//	武器オブジェクト
	public GameObject playerHand;			//	攻撃時武器の親オブジェクト
	public GameObject playerNeck;			//	通常時武器の親オブジェクト
	public SkinnedMeshRenderer modelMesh;	//	プレイヤーモデルのmesh
	public MeshRenderer weaponMesh;			//	武器のmesh
	public ClimbCheck climbCheck;			//	昇るチェック
	public PlayerInput pi;					//	入力コントローラー
	public AudioClip[] sounds;              //	SEオブジェクト
	public ParticleSystem walkDust;			//	歩いてのエフェクト
	public int hp;							//	プレイヤーHP
	public int coinCount;                   //	獲得したコイン数
	public int starCount;                   //	獲得したスター数
	public bool coinUIAction;				//  コインUIを動くための信号
	public bool climbEnd;                   //	登るエンドフラグ
	public bool isFall;
	public bool isLand;
	public bool isDead;						//	プレイヤーが死亡flag
	public bool isFallDead;					//	外に落ちるflag

	public float moveSpeed;					//	移動スピード
	public bool isInTrigger;				//	仕掛けスイッチを当たるフラグ
	public bool isDamageByEnemy;			//	敵と衝突したフラグ
	public bool isUnrivaled;				//	無敵Time
	public bool damageByStick;              //	針と接触するフラグ
	public bool cameraCanMove;				//	ダメージを受けた後カメラ移動可能の時間
	public bool shootStart;					//	武器発射flag
	public bool isJumping;					//	ジャンプflag
	public bool isClimbing;                 //	登るflag
	public bool isPushBox;
	public bool isEntryDoor;
	public bool isInCameraZoom;
	public int cameraZoomIndex;
	public Vector3 climbLandPos;
	public bool enemyCanHurt;

	//---鍾家同(2021/07/19)---
	[System.Serializable]
	public struct HaveKeys
	{
		public bool BlueKey;
		public bool GreenKey;
	}
	public HaveKeys haveKeys;

	public bool[] havePieces = {false, false, false};
	[HideInInspector]
	public bool havePortalKey = false;
	//-------------------------

	private new AudioSource audio;			//	SEのコンポーネント
	private Animator anim;					//	アニメコントローラーコンポーネント
	private Rigidbody rigid;                //	鋼体コンポーネント
	private CameraController cc;
	private Vector3 movingVec;				//	移動方向
	private GoalController gc;              //	ゴールコントローラー
	private BarrierController bc;			//	針オブジェクトコントローラー
	private int shortTimeCount;				//	点滅用タイムカウント
	private Vector3 weaponStartPos;			//	武器の初期位置座標保存用
	private Vector3 weaponStartRot;         //	武器の初期回転角度保存用
	private Vector3 weaponAttackPos = new Vector3(-0.404f, 0.389f, 0.729f);                  //	武器攻撃する時位置座標保存用
	private Vector3 weaponAttackRot = new Vector3(20.763f, 43.697f, 145.891f);          //	武器攻撃する時回転角度保存用

	private Vector3 damageRot;				//	ダメージを受ける時の回転角度
	private float timeCount;                //	タイムカウント
	private Vector3 nowPos;					//	
	private Vector3 damagePos;              //	モンスターからダメージを受けた位置
	private GameObject stickBackPos;        //	針からダメージを受けたら戻る位置
	private float damageTimeCount;          //	
	private bool playerCanMove = true;
	private bool doOnce;
	private bool unLockAttack = true;
	public float attackGapTime;
	private bool weaponSound1;
	private bool weaponSound2;
	private bool emmTrue = true;
	private bool emmFalse = false;

	//	初期化
	void Awake()
	{
		pi = GetComponent<PlayerInput>();

		audio = gameObject.GetComponent<AudioSource>();

		anim = model.GetComponent<Animator>();

		cc = GameObject.Find("Main Camera").GetComponent<CameraController>();

		rigid = GetComponent<Rigidbody>();

		gc = GameObject.Find("Goal").GetComponent<GoalController>();

		weaponStartPos = weapon.transform.localPosition;

		weaponStartRot = weapon.transform.localEulerAngles;

		walkDust = GetComponent<ParticleSystem>();

		Physics.IgnoreLayerCollision(11, 13, false);
		Physics.IgnoreLayerCollision(11, 16, false);
		Physics.IgnoreLayerCollision(11, 24, false);
	}

	void Update()
	{
		if (!isDead)
		{
			//	歩くアニメーションの数値
			if (pi.inputEnabled)
			{
				anim.SetFloat("Forward", pi.Dmag);
			}
			else if (!isEntryDoor)
			{
				anim.SetFloat("Forward", 0.0f);
			}

			//	近戦攻撃処理
			if (pi.isAttacking && !pi.isAimStatus)
            {
                anim.SetLayerWeight(anim.GetLayerIndex("Attack"), 1.0f);
                anim.SetTrigger("Slash1");
                pi.isAttacking = false;
			}
			//	第三視点武器を投げる処理
			else if (pi.isThrowing && cc.canThrowWeapon && !pi.isAimStatus)
            {
				anim.SetLayerWeight(anim.GetLayerIndex("Attack"), 1.0f);
                anim.SetTrigger("Throw");
                audio.PlayOneShot(sounds[1]);

                pi.canThrow = false;                    //	武器を手に戻るまで投げれない設定

                pi.isThrowing = false;
            }

            if (anim.GetCurrentAnimatorStateInfo(1).IsName("Slash1"))
            {
				if (!weaponSound1)
				{
					enemyCanHurt = true;
					audio.PlayOneShot(sounds[0]);
					weaponSound1 = true;
				}
				weapon.transform.tag = "Slash1";
            }
            else if (anim.GetCurrentAnimatorStateInfo(1).IsName("Slash2"))
			{
				if (!weaponSound2)
				{
					audio.PlayOneShot(sounds[0]);
					weaponSound2 = true;
				}
				weapon.transform.tag = "Slash2";
			}

			if (anim.GetCurrentAnimatorStateInfo(1).IsName("Slash1") || anim.GetCurrentAnimatorStateInfo(1).IsName("Slash2"))
			{
				if (unLockAttack)
				{
					anim.SetLayerWeight(anim.GetLayerIndex("Attack"), 1.0f);
					weapon.transform.SetParent(playerHand.transform);
					//	武器の位置調整
					weapon.transform.localPosition = weaponAttackPos;
					weapon.transform.localEulerAngles = weaponAttackRot;
					unLockAttack = false;
				}
            }			
			
			if (anim.GetCurrentAnimatorStateInfo(1).IsName("Idle") && !unLockAttack)
			{
				enemyCanHurt = false;
				cc.canThrowWeapon = true;
				pi.attackCount = 0;
				weapon.transform.SetParent(playerNeck.transform);
				//	武器の位置を初期に戻る
				weapon.transform.localPosition = weaponStartPos;
				weapon.transform.localEulerAngles = weaponStartRot;
				weapon.transform.tag = "Untagged";
				anim.SetLayerWeight(anim.GetLayerIndex("Attack"), 0.0f);
				unLockAttack = true;
				weaponSound1 = false;
				weaponSound2 = false;
			}

            if (isClimbing && (Input.GetKey(pi.keyUp) || Input.GetKey(pi.keyDown) || Input.GetAxis("axisY") != 0))                         //	梯子を登る処理
			{
				anim.speed = 1.0f;
				anim.SetBool("Climb", true);
			}
			else if (isClimbing)
			{
				anim.SetBool("Climb", true);
				if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && anim.GetCurrentAnimatorStateInfo(0).IsName("Climb"))
				{
					anim.speed = 0.0f;
				}
			}
			else
			{
				anim.SetBool("Climb", false);
			}
		}

		if (pi.isAimStatus || isPushBox)
		{
			moveSpeed = 3.0f;
		}
		else
		{
			moveSpeed = 7.0f;
		}

		CheckIsUnderDamage();

		CheckPlayerIsDead();
	}

	//	移動処理
	void FixedUpdate()
	{
		if (pi.Dmag > 0.01f && (!pi.isAimStatus && !isPushBox))     //	1.移動の入力値が0.1を超える時	2.狙う状態ではない時	->	 移動方向を設定する
		{
			model.transform.forward = Vector3.Slerp(model.transform.forward, pi.Dvec, 10.0f * Time.deltaTime);
			movingVec = pi.Dmag * model.transform.forward;
		}
		else if (pi.Dmag > 0.01f && pi.isAimStatus)  //	1.移動の入力値が0.1を超える時	2.狙う状態の時	->	 移動方向を設定する
		{
			movingVec = pi.Dmag * pi.Dvec;
		}
		else if (!isPushBox)                                                 //	以外の状態
		{
			movingVec = pi.Dmag * model.transform.forward;
		}

		/*
		if (pi.Dmag > 0.01f)
        {
			var walkDust_emission = walkDust.emission;
			walkDust_emission.enable = emmTrue;
		} // end if()
        else
        {
			var walkDust_emission = walkDust.emission;
			walkDust_emission.enable = emmFalse;
		} // end else
		*/

		transform.position += movingVec * moveSpeed * Time.fixedDeltaTime;
		
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

	public bool PlayerCanMove()
	{
		if (gc.gameClear || pi.lockJumpStatus || !playerCanMove || isDead || isEntryDoor || isClimbing || isFall || cc.cameraStatic != "Idle")
		{
			pi.inputEnabled = false;
		}
		else
		{
			pi.inputEnabled = true;
		}

		return pi.inputEnabled;
	}

	private void CheckIsUnderDamage()	//	敵と当たると時間内に無敵状態になる
	{
		if (isUnrivaled)
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

			if (shortTimeCount >= 8 && timeCount >= 0.15f)	//	点滅が終わったら
			{
				shortTimeCount = 0;
				modelMesh.enabled = true;
				weaponMesh.enabled = true;
				Physics.IgnoreLayerCollision(11, 13, false);//	
				damageTimeCount = 0.0f;         //	
				isUnrivaled = false;
				damageByStick = false;
				doOnce = false;
			}
			else if (shortTimeCount > 2)                    //	プレイヤー入力可能
			{
				if (damageByStick && !cameraCanMove)
				{
					if (!isPushBox)
					{
						transform.position = stickBackPos.transform.position;
					}
					bc.stickCanMove = true;
				}
				playerCanMove = true;
				pi.inputEnabled = true;
				cameraCanMove = true;           //	
				rigid.constraints = RigidbodyConstraints.FreezeRotation;
			}

			HitDistance();
		}
	}

	//	死亡処理
	private void CheckPlayerIsDead()	
	{
		if (hp <= 0)
		{
			isDead = true;
		}

		if (isDead)
		{
			HitDistance();
			Time.timeScale = 0.4f;		//	時間の流すを遅くなるように
			movingVec = Vector3.zero;	
			anim.enabled = false;
			pi.ResetSignal();
			model.GetComponent<CapsuleCollider>().enabled = true;
			Physics.IgnoreLayerCollision(11, 13, true);		//	敵とプレイヤー
			Physics.IgnoreLayerCollision(11, 16, true);		//	針とプレイヤー
			Physics.IgnoreLayerCollision(11, 24, true);		//	コインとプレイヤー
			model.transform.localRotation = Quaternion.Lerp(model.transform.localRotation, Quaternion.Euler(-90.0f, damageRot.y, damageRot.z), 3.0f * Time.deltaTime);
			transform.tag = "Untagged";
		}

		if (isFallDead)
		{
			pi.ResetSignal();
		}
	}

	private void HitDistance()
	{
		if (damageTimeCount >= 0.05f && !damageByStick && !doOnce)
		{
			damagePos = transform.position;

			if (Vector3.Distance(damagePos, nowPos) >= 0.8f)
			{
				rigid.constraints = ~RigidbodyConstraints.FreezePositionY;      //	~ -> Y軸以外の移動と回転を止める
			}
			doOnce = true;
		}
		else if (damageTimeCount < 0.05f)
		{
			damageTimeCount += Time.deltaTime;
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		switch (collider.transform.tag)
		{
			case "BlueKey":
				haveKeys.BlueKey = true;
				break;
			case "GreenKey":
				haveKeys.GreenKey = true;
				break;
			case "RedPiece":
				havePieces[0] = true;
				break;
			case "BluePiece":
				havePieces[1] = true;
				break;
			case "GreenPiece":
				havePieces[2] = true;
				break;
			case "PortalKey":
				havePortalKey = true;
				break;
			default:
				break;
		}

		if (collider.transform.tag == "ClimbOver") 
		{
			climbEnd = true;
			climbLandPos = collider.gameObject.transform.position;
			rigid.useGravity = true;
		}

		if (collider.transform.tag == "StickBackPos")
		{
			stickBackPos = collider.gameObject;
		}

		if (collider.transform.tag == "FireBall" && !isUnrivaled)
		{
			hp--;
			damageRot = model.transform.localEulerAngles;
			audio.PlayOneShot(sounds[2]);
			pi.ResetSignal();

			if (hp > 0)
			{
				modelMesh.enabled = false;
				weaponMesh.enabled = false;
				nowPos = transform.position;
				isUnrivaled = true;
			}
			else
			{
				rigid.AddExplosionForce(800.0f, collider.transform.position - new Vector3(0.0f, 1.5f, 0.0f), 5.0f, 2.0f);      //	爆発の位置を矯正
				isDead = true;
			}
			playerCanMove = false;
		}

		if (collider.transform.tag == "CameraZoom")
		{
			if (collider.GetComponent<DoorZoomController>().zoomIndex == 1)
			{
				collider.GetComponent<DoorZoomController>().zoomIndex = 4;
			}
			else if (collider.GetComponent<DoorZoomController>().zoomIndex == 4)
			{
				collider.GetComponent<DoorZoomController>().zoomIndex = 1;
			}

			cameraZoomIndex = collider.GetComponent<DoorZoomController>().zoomIndex;
			cameraCanMove = false;
			isInCameraZoom = true;
		}
	}
	//-------------------------

	private void OnTriggerStay(Collider collider)
	{
        if (collider.transform.gameObject.layer == LayerMask.NameToLayer("Trigger"))
        {
            isInTrigger = true;
        }

		if (collider.transform.tag == "DeadCheck")
		{
			isFallDead = true;
		}

		if (collider.transform.tag == "Stick" && !isUnrivaled)
		{
			hp--;
			damageRot = model.transform.localEulerAngles;
			audio.PlayOneShot(sounds[2]);
			cameraCanMove = false;
			pi.inputEnabled = false;
			bc = collider.transform.parent.GetComponent<BarrierController>();
			if (bc != null)
			{
				bc.stickCanMove = false;
			}

			if (hp > 0)
			{
				modelMesh.enabled = false;
				weaponMesh.enabled = false;
				damageByStick = true;
				playerCanMove = false;
				isUnrivaled = true;
			}
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (collider.transform.gameObject.layer == LayerMask.NameToLayer("Trigger"))
		{
			isInTrigger = false;
		}

		if (collider.transform.tag == "CameraZoom")
		{
			cameraCanMove = true;
			isInCameraZoom = false;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.transform.tag == "Enemy" && !isUnrivaled)		//	敵と当たる処理
		{
			hp--;
			damageRot = model.transform.localEulerAngles;
			audio.PlayOneShot(sounds[2]);
			Physics.IgnoreLayerCollision(11, 13, true);
			cameraCanMove = false;
			pi.ResetSignal();

			if (hp > 0)
			{
				modelMesh.enabled = false;
				weaponMesh.enabled = false;
				rigid.AddExplosionForce(500.0f, collision.transform.position - new Vector3(0.0f, 1.5f, 0.0f), 5.0f, 1.5f);      //	爆発の位置を矯正
				nowPos = transform.position;
				isUnrivaled = true;
			}
			else
			{
				rigid.AddExplosionForce(800.0f, collision.transform.position - new Vector3(0.0f, 1.5f, 0.0f), 5.0f, 1.5f);      //	爆発の位置を矯正
			}
			playerCanMove = false;
		}
	}
}
