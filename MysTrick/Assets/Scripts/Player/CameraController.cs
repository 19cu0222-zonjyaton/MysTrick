using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public PlayerInput pi;
	public GoalController goal;
	public float horizontalSpeed = 50.0f;
	public float verticalSpeed = 40.0f;
	public GameObject cameraBackPos;            //  カメラ戻る時の位置
	public Vector3 moveToPos;                   //  カメラ目標の位置
	public string cameraStatic = "Idle";        //  カメラ状態
	public StairController[] stair;
	public DoorController[] door;
	public BridgeController[] bridge;
	public LadderController[] ladder;
	public ObjectController[] ob;
	public GameObject[] lookAtStair;
	public GameObject[] lookAtDoor;
	public GameObject[] lookAtBridge;
	public GameObject[] lookAtLadder;
	public GameObject[] lookAtObject;
	public GameObject lookAtGoal;
	public GameObject firstPerspect;
	public GameObject weapon;
	public GameObject usingWeapon;              //	今手が持っている武器
	public bool canThrowWeapon = true;

	private ActorController ac;
	private GameObject cameraHandle;
	private GameObject playerHandle;
	private float aimEulerX;					//	狙う状態のカメラX軸回転値
	private float idleEulerX;
	private GameObject model;
	private SkinnedMeshRenderer smr;
	private float countTime = 10.0f;            //  カメラ視角切り替えの時間
	private bool canRotate;						//	プレイヤー視点切り替えた後回転できるか
	private Vector3 relativePos;

	// Start is called before the first frame update
	void Awake()
	{
		cameraHandle = transform.parent.gameObject;

		playerHandle = cameraHandle.transform.parent.gameObject;

		model = playerHandle.GetComponent<ActorController>().model;

		smr = GameObject.Find("Model").GetComponent<SkinnedMeshRenderer>();

		goal = GameObject.Find("Goal").GetComponent<GoalController>();

		cameraBackPos = GameObject.Find("CameraPos");

		ac = GameObject.Find("PlayerHandle").GetComponent<ActorController>();
	}

	// Update is called once per frame
	void LateUpdate()
	{
		if (Time.deltaTime != 0)
		{
			checkCameraStatic();
		}
	}

	private void checkCameraStatic()
	{
		//  When Trigger are Stairs
		for (int i = 0; i < stair.Length; i++)
		{
			if (stair[i].isTriggered && !stair[i].hasDone)
			{
				cameraStatic = "MoveToStair" + (i + 1);
				pi.inputEnabled = false;
				stair[i].hasDone = true;
			}

			if (cameraStatic == "MoveToStair" + (i + 1))
			{
				cameraMove(lookAtStair[i].transform.position, stair[i].gameObject);
			}
		}
		//  When Trigger are doors
		for (int i = 0; i < door.Length; i++)
		{
			if (door[i].isTriggered && !door[i].hasDone)
			{
				cameraStatic = "MoveToDoor" + (i + 1);
				pi.inputEnabled = false;
				door[i].hasDone = true;
			}

			if (cameraStatic == "MoveToDoor" + (i + 1))
			{
				cameraMove(lookAtDoor[i].transform.position, door[i].gameObject);
			}
		}
		//  When Trigger are bridges
		for (int i = 0; i < bridge.Length; i++)
		{
			if (bridge[i].isTriggered && !bridge[i].hasDone)
			{
				cameraStatic = "MoveToBridge" + (i + 1);
				pi.inputEnabled = false;
				bridge[i].hasDone = true;
			}

			if (cameraStatic == "MoveToBridge" + (i + 1))
			{
				cameraMove(lookAtBridge[i].transform.position, bridge[i].gameObject);
			}
		}

		//  When Trigger are ladders
		for (int i = 0; i < ladder.Length; i++)
		{
			if (ladder[i].canRotate && !ladder[i].hasDone)
			{
				cameraStatic = "MoveToLadder" + (i + 1);
				pi.inputEnabled = false;
				ladder[i].hasDone = true;
			}

			if (cameraStatic == "MoveToLadder" + (i + 1))
			{
				cameraMove(lookAtLadder[i].transform.position, ladder[i].gameObject);
			}
		}

		//  When Trigger are object
		for (int i = 0; i < ob.Length; i++)
		{
			if (ob[i].isTrigger && !ob[i].hasDone)
			{
				cameraStatic = "MoveToObject" + (i + 1);
				pi.inputEnabled = false;
				ob[i].hasDone = true;
			}

			if (cameraStatic == "MoveToObject" + (i + 1))
			{
				cameraMove(lookAtObject[i].transform.position, ob[i].gameObject);
			}
		}

		if (!goal.gameClear && cameraStatic == "Idle")
		{
			if (pi.isAimStatus && !pi.lockJumpStatus)       //	Aiming and is not jumping
			{
				transform.parent = null;

				//指定した方向とか指定したゲームオブジェクトからレーザー光線をだせる機能です
				//Ray ray = new Ray(transform.position, transform.forward);

				//RaycastHit hit; //レイが衝突したオブジェクト

				//if (Physics.Raycast(ray, out hit, Mathf.Infinity))

				//{
				//	//もしrayとhitが衝突した場合．．．の処理内容
				//	if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
				//	{
				//		//もしhitのレイヤーが指定した"レイヤーの名前"と一致していた場合．．．の処理内容
				//	}
				//	Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 0.1f);
				//}

				transform.position = Vector3.Slerp(transform.position, firstPerspect.transform.position, 20.0f * Time.fixedDeltaTime);

				if (transform.eulerAngles.x > 0.01f && !canRotate)      //	transform.eulerAngles　->	自身の回転角度を獲得できる		視点切り替え途中は回転できない
				{
					transform.rotation = Quaternion.Slerp(transform.rotation, firstPerspect.transform.rotation, Time.fixedDeltaTime * 12.0f);
					aimEulerX = 0.0f;
				}
				else
				{
					if (pi.isThrowing)
					{
						Instantiate(weapon, transform.position + transform.forward * 1.5f, transform.rotation);

						canThrowWeapon = false;

						pi.canThrow = false;

						pi.isThrowing = false;
					}

					canRotate = true;
					ac.moveSpeed = 2.0f;
					aimEulerX -= pi.Jup * verticalSpeed * 2.0f * Time.fixedDeltaTime;
					transform.Rotate(Vector3.up, pi.Jright * 80 * Time.fixedDeltaTime);
					aimEulerX = Mathf.Clamp(aimEulerX, -80, 80);                  //  縦の回転角を制限する
					transform.localEulerAngles = new Vector3(aimEulerX, transform.localEulerAngles.y, 0);

					smr.enabled = false;
				}
			}
			else if (!pi.isAimStatus && !pi.lockJumpStatus && !ac.isUnrivaled && !ac.isDead)  //	not Aiming and not jumping
			{
				Vector3 tempModelEuler = model.transform.eulerAngles;
				playerHandle.transform.Rotate(Vector3.up, pi.Jright * horizontalSpeed * Time.fixedDeltaTime);
				idleEulerX -= pi.Jup * verticalSpeed * Time.fixedDeltaTime;
				idleEulerX = Mathf.Clamp(idleEulerX, -25, 15);                      //  縦の回転角を制限する

				cameraHandle.transform.localEulerAngles = new Vector3(idleEulerX, 0, 0);

				model.transform.eulerAngles = tempModelEuler;

				transform.parent = cameraHandle.transform;
				canRotate = false;
				smr.enabled = true;
				pi.inputEnabled = true;
				ac.moveSpeed = 5.0f;

				//  位置を戻る
				transform.position = Vector3.Slerp(transform.position, cameraBackPos.transform.position, 5.0f * Time.fixedDeltaTime);

				transform.rotation = Quaternion.Slerp(transform.rotation, cameraBackPos.transform.rotation, 6.0f * Time.fixedDeltaTime);
			}
			else if (pi.lockJumpStatus) //	is jumping
			{
				Vector3 tempModelEuler = model.transform.eulerAngles;
				playerHandle.transform.Rotate(Vector3.up, pi.Jright * horizontalSpeed * Time.fixedDeltaTime);
				idleEulerX -= pi.Jup * verticalSpeed * Time.fixedDeltaTime;
				idleEulerX = Mathf.Clamp(idleEulerX, -25, 15);

				cameraHandle.transform.localEulerAngles = new Vector3(idleEulerX, 0, 0);   //  縦の回転角を制限する

				model.transform.eulerAngles = tempModelEuler;
			}
		}
		else if (goal.gameClear)
		{
			transform.SetParent(null);
			cameraStatic = "GameClear";

			transform.position = Vector3.Slerp(transform.position, lookAtGoal.transform.position, 20.0f * Time.fixedDeltaTime);
			transform.rotation = Quaternion.Slerp(transform.rotation, lookAtGoal.transform.rotation, 3.0f * Time.fixedDeltaTime);
			transform.LookAt(playerHandle.transform);
		}
		
		if (ac.isDead)
		{
			model.transform.localRotation = new Quaternion(0.0f, 180.0f, 90.0f, 0.0f);
			gameObject.GetComponent<Camera>().fieldOfView = 20.0f;
		}
	}

	//  カメラ移動関数
	private void cameraMove(Vector3 movePos, GameObject target)
	{
		countTime -= Time.fixedDeltaTime * 1.2f;

		if (countTime <= 9.0f && countTime > 6.0f)
		{
			//  親関係を解除
			transform.parent = null;

			transform.position = Vector3.Slerp(transform.position, movePos, 2.0f * Time.fixedDeltaTime);

			// 補完スピードを決める
			float speed = 0.08f;
			if (target != null)
			{
				// ターゲット方向のベクトルを取得
				relativePos = target.transform.position - this.transform.position;
			}
			// 方向を、回転情報に変換
			Quaternion rotation = Quaternion.LookRotation(relativePos);
			// 現在の回転情報と、ターゲット方向の回転情報を補完する
			transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, speed);
		}
		else if (countTime <= 4.0f && countTime >= 2.0f)
		{
			//  親関係になる
			transform.SetParent(cameraHandle.transform);

			//  位置を戻る
			transform.position = Vector3.Slerp(transform.position, cameraBackPos.transform.position, 4.0f * Time.fixedDeltaTime);

			//  角度を戻る
			transform.rotation = Quaternion.Slerp(transform.rotation, cameraBackPos.transform.rotation, 2.0f * Time.fixedDeltaTime);
		}
		else if (countTime < 2.0f)
		{
			cameraStatic = "Idle";

			countTime = 10.0f;

			pi.inputEnabled = true;
		}
	}
}


