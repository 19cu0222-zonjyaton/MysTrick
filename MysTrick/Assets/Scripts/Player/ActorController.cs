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
	public AudioClip[] sounds;				//	SEオブジェクト
	public int hp;							//	プレイヤーHP
	public int coinCount;					//	獲得したコイン数
	public bool coinUIAction;				//  コインUIを動くための信号
	public bool climbEnd;					//	登るエンドフラグ
	public bool isDead;						//	プレイヤーが死亡flag
	public bool isFall;						//	外に落ちるflag

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
	public bool isInCameraZoom;
	public int cameraZoomIndex;
	public Vector3 climbLandPos;

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
	private Animation attack_anim;			//	アニメーションコントローラー
	private Rigidbody rigid;				//	鋼体コンポーネント
	private Vector3 movingVec;				//	移動方向
	private GoalController gc;              //	ゴールコントローラー
	private BarrierController bc;			//	針オブジェクトコントローラー
	private int shortTimeCount;				//	点滅用タイムカウント
	private Vector3 weaponStartPos;			//	武器の初期位置座標保存用
	private Vector3 weaponStartRot;         //	武器の初期回転角度保存用
	private Vector3 weaponAttackPos = new Vector3(0.96f, 0.316f, -0.447f);					//	武器攻撃する時位置座標保存用
	private Vector3 weaponAttackRot = new Vector3(195.201f, -142.271f, -167.015f);			//	武器攻撃する時回転角度保存用
	private Vector3 damageRot;				//	ダメージを受ける時の回転角度
	private float timeCount;                //	タイムカウント
	private Vector3 nowPos;					//	
	private Vector3 damagePos;              //	モンスターからダメージを受けた位置
	private GameObject stickBackPos;        //	針からダメージを受けたら戻る位置
	private float damageTimeCount;          //	
	private bool doOnce;

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

		Physics.IgnoreLayerCollision(11, 13, false);
	}

	void Update()
	{
		//	歩くアニメーションの数値
		anim.SetFloat("Forward", pi.Dmag);

		if (!isDead)
		{			
			if (pi.Dmag > 0.1f && (!pi.isAimStatus && !isPushBox))		//	1.移動の入力値が0.1を超える時	2.狙う状態ではない時	->	 移動方向を設定する
			{
				model.transform.forward = Vector3.Slerp(model.transform.forward, pi.Dvec, 10.0f * Time.deltaTime);
				movingVec = pi.Dmag * model.transform.forward; 
			}
			else if (pi.Dmag > 0.1f && (pi.isAimStatus || isPushBox))  //	1.移動の入力値が0.1を超える時	2.狙う状態の時	->	 移動方向を設定する
			{
				movingVec = pi.Dmag * pi.Dvec;
			}
			else													   //	以外の状態
			{
				movingVec = pi.Dmag * model.transform.forward;
			}

			//	近戦攻撃処理
			if (pi.isAttacking && !pi.isAimStatus)							
			{
				attack_anim.Play();						//	近戦アニメを流す

				if (!audio.isPlaying)					//	SEを流してない時
				{
					audio.pitch = 2.0f;					//	音の大きさを調整
					audio.PlayOneShot(sounds[0]);		//	近戦SEを流す
				}

				pi.isAttacking = false;
			}
			//	第三視点武器を投げる処理
			else if (pi.isThrowing && !pi.isAimStatus && !attack_anim.isPlaying)   
			{
				anim.SetTrigger("Throw");
				anim.SetLayerWeight(anim.GetLayerIndex("Throw"), 1.0f);
				audio.PlayOneShot(sounds[1]);

				pi.canThrow = false;					//	武器を手に戻るまで投げれない設定

				pi.isThrowing = false;					
			}

			if (attack_anim.isPlaying)                  //	攻撃する時tagを有効にする
			{
				weapon.transform.SetParent(playerHand.transform);
				//	武器の位置調整
				weapon.transform.localPosition = weaponAttackPos;
				weapon.transform.localEulerAngles = weaponAttackRot;
				weapon.transform.tag = "Weapon";
				pi.canAttack = false;           //	
			}
			else if (!pi.canAttack)                         //	一回だけ実行する
			{
				weapon.transform.SetParent(playerNeck.transform);
				//	武器の位置を初期に戻る
				weapon.transform.localPosition = weaponStartPos;
				weapon.transform.localEulerAngles = weaponStartRot;
				weapon.transform.tag = "Untagged";
				audio.pitch = 1.0f;
				pi.canAttack = true;            //	
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
					transform.position = stickBackPos.transform.position;
					bc.stickCanMove = true;
				}
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
			Physics.IgnoreLayerCollision(11, 13, true);
			model.transform.localRotation = Quaternion.Lerp(model.transform.localRotation, Quaternion.Euler(-90.0f, damageRot.y, damageRot.z), 3.0f * Time.deltaTime);
			transform.tag = "Untagged";
		}

		if (isFall)
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

	//---鍾家同(2021/07/19)---
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

		if (collider.transform.tag == "CameraZoom")
		{
			cameraZoomIndex = collider.GetComponent<DoorZoomController>().zoomIndex;
			cameraCanMove = false;
			isInCameraZoom = true;
		}
	}
	//-------------------------

	private void OnTriggerStay(Collider collider)
	{
		//if ((collider.transform.tag == "Device" || collider.transform.tag == "Handle" || collider.transform.tag == "Key" || collider.transform.tag == "MoveBox") && !isPushBox)
		//{
		//	isInTrigger = true;
		//}
		//else if(isPushBox)
		//{
		//	isInTrigger = false;
		//}

		if (collider.transform.tag == "DeadCheck")
		{
			isFall = true;
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
			}
			isUnrivaled = true;
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		//if (collider.transform.tag == "Device" || collider.transform.tag == "Handle" || collider.transform.tag == "Key" || collider.transform.tag == "MoveBox")
		//{
		//	isInTrigger = false;
		//}

		if (collider.transform.tag == "CameraZoom")
		{
			cameraCanMove = true;
			isInCameraZoom = false;
		}
	}

	private void OnCollisionStay(Collision collision)
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
				rigid.AddExplosionForce(800.0f, collision.transform.position - new Vector3(0.0f, 1.5f, 0.0f), 5.0f, 2.0f);      //	爆発の位置を矯正
				isDead = true;
			}
		}
	}
}
