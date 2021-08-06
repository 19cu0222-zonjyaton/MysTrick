﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public PlayerInput pi;						//	プレイヤーの入力コントローラー
	public GoalController goal;					//	ゴールコントローラー
	public GameObject cameraBackPos;            //  カメラ戻る時の位置
	public Vector3 moveToPos;                   //  カメラ目標の位置
	public StairController[] stair;				//	階段オブジェクト
	public DoorController[] door;				//	ドアオブジェクト
	public BridgeController[] bridge;			//	梯オブジェクト
	public LadderController[] ladder;			//	梯子オブジェクト
	public ObjectController[] ob;				//	他のオブジェクト
	public GameObject[] lookAtStair;			//	階段を注目する位置
	public GameObject[] lookAtDoor;				//	ドアを注目する位置
	public GameObject[] lookAtBridge;           //	梯を注目する位置
	public GameObject[] lookAtLadder;           //	梯子を注目する位置
	public GameObject[] lookAtObject;           //	他のオブジェクトを注目する位置
	public GameObject lookAtGoal;				//	ゴールを注目する位置
	public GameObject firstPerspect;			//	狙う状態に切り替える時移動の位置
	public GameObject weapon;					//	武器オブジェクト
	public GameObject usingWeapon;              //	今手が持っている武器
	public GameObject deadPos;                  //	プレイヤーが死亡したら視点の位置
	public bool canThrowWeapon = true;			//	狙う状態で武器を投げる可能フラグ
	public string cameraStatic = "Idle";        //  カメラ状態
	public float horizontalSpeed = 50.0f;		//	カメラ横移動のスピード
	public float verticalSpeed = 40.0f;			//	カメラ縦移動のスピード

	private ActorController ac;					//	プレイヤーの挙動コントローラー
	private GameObject cameraHandle;			//	カメラハンドルオブジェクト
	private GameObject playerHandle;			//	プレイヤーハンドルオブジェクト
	private GameObject model;					//	プレイヤーモデルオブジェクト
	private SkinnedMeshRenderer smr;			//	プレイヤーメッシュコンポーネント
	private new AudioSource audio;				//	SEコンポーネント
	private Vector3 relativePos;				//	移動ターゲットの相対位置
	private float aimEulerX;					//	狙う状態のカメラX軸回転値
	private float idleEulerX;                   //	制限されたX軸の回転角度
	private float countTime = 10.0f;            //  カメラ視角切り替えの時間
	private bool canRotate;                     //	プレイヤー視点切り替えた後回転できるか

	// 初期化
	void Awake()
	{
		cameraHandle = transform.parent.gameObject;

		playerHandle = cameraHandle.transform.parent.gameObject;

		model = playerHandle.GetComponent<ActorController>().model;

		smr = GameObject.Find("Model").GetComponent<SkinnedMeshRenderer>();

		goal = GameObject.Find("Goal").GetComponent<GoalController>();

		cameraBackPos = GameObject.Find("CameraPos");

		ac = playerHandle.GetComponent<ActorController>();

		audio = gameObject.GetComponent<AudioSource>();
	}

	//	カメラアップデート処理
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

		if (!goal.gameClear && cameraStatic == "Idle" && !ac.isDead && !ac.isFall)
		{
			if (pi.isAimStatus && !pi.lockJumpStatus)       //	Aiming and is not jumping
			{
				transform.parent = null;

				transform.position = Vector3.Slerp(transform.position, firstPerspect.transform.position, 20.0f * Time.fixedDeltaTime);

				if (transform.eulerAngles.x > 0.01f && !canRotate)      //	transform.eulerAngles　->	自身の回転角度を獲得できる		視点切り替え途中は回転できない
				{
					transform.rotation = Quaternion.Slerp(transform.rotation, firstPerspect.transform.rotation, Time.fixedDeltaTime * 12.0f);
					aimEulerX = 0.0f;
				}
				else
				{
					canRotate = true;
					ac.moveSpeed = 2.0f;
					aimEulerX -= pi.Jup * verticalSpeed * 2.0f * Time.fixedDeltaTime;
					transform.Rotate(Vector3.up, pi.Jright * 80 * Time.fixedDeltaTime);
					aimEulerX = Mathf.Clamp(aimEulerX, -80, 80);                  //  縦の回転角を制限する
					transform.localEulerAngles = new Vector3(aimEulerX, transform.localEulerAngles.y, 0);

					smr.enabled = false;

					if (aimEulerX < 35.0f)      //	角度が一定範囲だけ打つことができる
					{
						if (pi.isThrowing)
						{
							Instantiate(weapon, transform.position + transform.forward * 1.5f, transform.rotation);

							audio.Play();

							canThrowWeapon = false;

							pi.canThrow = false;

							pi.isThrowing = false;
						}
					}
					else
					{
						if (pi.isThrowing)
						{
							pi.isThrowing = false;      //	撃つ信号をなくす
						}
					}
				}
			}
			else if (!pi.isAimStatus && !pi.lockJumpStatus && ac.cameraCanMove && !ac.isDead)  //	not Aiming and not jumping
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
		else if (goal.gameClear)    //	プレイヤーがクリアしたらカメラの処理
		{
			transform.SetParent(null);
			cameraStatic = "GameClear";

			transform.position = Vector3.Slerp(transform.position, lookAtGoal.transform.position, 20.0f * Time.fixedDeltaTime);
			transform.rotation = Quaternion.Slerp(transform.rotation, lookAtGoal.transform.rotation, 3.0f * Time.fixedDeltaTime);
			transform.LookAt(playerHandle.transform);
		}
		else if (ac.isDead)         //	プレイヤーが死亡したらカメラの処理
		{
			transform.SetParent(null);
			deadPos.transform.SetParent(null);
			cameraStatic = "GameOver";

			transform.position = deadPos.transform.position;
			transform.LookAt(playerHandle.transform);
		}
		else if (ac.isFall)         //	プレイヤーが外に落ちたらカメラの処理
		{
			transform.SetParent(null);
		}

		if (ac.isClimbing)
		{
			gameObject.GetComponent<Camera>().fieldOfView = 30.0f;
		}
		else
		{
			gameObject.GetComponent<Camera>().fieldOfView = 60.0f;
		}
	}

	//  カメラ移動関数
	private void cameraMove(Vector3 movePos, GameObject target)
	{
		countTime -= Time.fixedDeltaTime * 1.5f;

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


