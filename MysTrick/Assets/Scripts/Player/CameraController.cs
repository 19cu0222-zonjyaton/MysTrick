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
	public GameObject[] targetPos;
	public GameObject firstPerspect;
	public GameObject weapon;

	private ActorController ac;
	private GameObject cameraHandle;
	private GameObject playerHandle;
	private float tempEulerX;
	private GameObject model;
	private bool doOnce;                        //  ゴールに着いたら一回だけを実行するための参数
	private float countTime = 10.0f;            //  カメラ視角切り替えの時間
	private Vector3 relativePos;

	// Start is called before the first frame update
	void Awake()
	{
		cameraHandle = transform.parent.gameObject;

		playerHandle = cameraHandle.transform.parent.gameObject;

		model = playerHandle.GetComponent<ActorController>().model;

		goal = GameObject.Find("Goal").GetComponent<GoalController>();

		cameraBackPos = GameObject.Find("CameraPos");

		ac = GameObject.Find("PlayerHandle").GetComponent<ActorController>();
	}

	// Update is called once per frame
	void LateUpdate()
	{
		checkCameraStatic();
    }

	//  カメラ移動関数
	private void cameraMove(Vector3 movePos, StairController stair, DoorController door, BridgeController bridge)
	{
		countTime -= Time.fixedDeltaTime;

		if (countTime <= 10.0f && countTime > 6.0f)
		{
			//  親関係を解除
			transform.parent = null;

			transform.position = Vector3.Slerp(transform.position, movePos, 0.02f);

			// 補完スピードを決める
			float speed = 0.08f;
			if (stair != null)
			{
				// ターゲット方向のベクトルを取得
				relativePos = stair.transform.position - this.transform.position;
			}
			else if (door != null)
			{
				relativePos = door.transform.position - this.transform.position;
			}
			else if (bridge != null)
			{
				relativePos = bridge.transform.position - this.transform.position;
			}
			// 方向を、回転情報に変換
			Quaternion rotation = Quaternion.LookRotation(relativePos);
			// 現在の回転情報と、ターゲット方向の回転情報を補完する
			transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, speed);
		}
		else if (countTime <= 4.0f && countTime >= 0.0f)
		{
			//  親関係になる
			transform.SetParent(cameraHandle.transform);

			//  位置を戻る
			transform.position = Vector3.Slerp(transform.position, cameraBackPos.transform.position, 0.02f);

			//  角度を戻る
			transform.rotation = Quaternion.Slerp(transform.rotation, cameraBackPos.transform.rotation, Time.fixedDeltaTime * 1.0f);
		}
		else if (countTime < 0.0f)
		{
			cameraStatic = "Idle";

			countTime = 10.0f;

			pi.inputEnabled = true;
		}
	}

	private void checkCameraStatic()
	{
		//  When Trigger are Stairs
		if (stair[0].isTriggered && !stair[0].hasDone)
		{
			cameraStatic = "MoveToStair1";

			pi.inputEnabled = false;

			stair[0].hasDone = true;
		}
		else if (stair[1].isTriggered && !stair[1].hasDone)
		{
			cameraStatic = "MoveToStair2";

			pi.inputEnabled = false;

			stair[1].hasDone = true;
		}
		else if (stair[2].isTriggered && !stair[2].hasDone)
		{
			cameraStatic = "MoveToStair3";

			pi.inputEnabled = false;

			stair[2].hasDone = true;
		}

		//  When Trigger are doors
		if (door[0].isTriggered && !door[0].hasDone)
		{
			cameraStatic = "MoveToDoor1";

			pi.inputEnabled = false;

			door[0].hasDone = true;
		}
		else if (door[1].isTriggered && !door[1].hasDone)
		{
			cameraStatic = "MoveToDoor2";

			pi.inputEnabled = false;

			door[1].hasDone = true;
		}

		//  When Trigger are bridges
		if (bridge[0].isTriggered && !bridge[0].hasDone)
		{
			cameraStatic = "MoveToBridge1";

			pi.inputEnabled = false;

			bridge[0].hasDone = true;
		}

		if (cameraStatic == "MoveToStair1")
		{
			cameraMove(targetPos[0].transform.position, stair[0], null, null);
		}
		else if (cameraStatic == "MoveToStair2")
		{
			cameraMove(targetPos[1].transform.position, stair[1], null, null);
		}
		else if (cameraStatic == "MoveToStair3")
		{
			cameraMove(targetPos[2].transform.position, stair[2], null, null);
		}
		else if (cameraStatic == "MoveToDoor1")
		{
			cameraMove(targetPos[3].transform.position, null, door[0], null);
		}
		else if (cameraStatic == "MoveToDoor2")
		{
			cameraMove(targetPos[4].transform.position, null, door[1], null);
		}
		else if (cameraStatic == "MoveToBridge1")
		{
			cameraMove(targetPos[5].transform.position, null, null, bridge[0]);
		}


		if (!goal.gameClear && cameraStatic == "Idle")
		{
			if (pi.isAimStatus && !pi.lockJumpStatus)       //	Aiming and is not jumping
			{
				if (pi.isAttacking)
				{
					Instantiate(weapon, transform.position + transform.forward * 1.5f, transform.rotation);

					pi.canAttack = false;

					pi.isAttacking = false;
				}

				Vector3 tempModelEuler = transform.eulerAngles;
				transform.Rotate(Vector3.up, pi.Jright * 120 * Time.fixedDeltaTime);
				transform.Rotate(Vector3.right, -pi.Jup * 120 * Time.fixedDeltaTime);

				transform.parent = null;

				transform.position = Vector3.Slerp(transform.position, firstPerspect.transform.position, 0.1f);

				transform.rotation = Quaternion.Slerp(transform.rotation, model.transform.rotation, Time.fixedDeltaTime * 2.0f);
			}
			else if (!pi.isAimStatus && !pi.lockJumpStatus && !ac.isUnrivaled && !ac.isDead)  //	not Aiming and not jumping
			{
				pi.inputEnabled = true;
				Vector3 tempModelEuler = model.transform.eulerAngles;
				playerHandle.transform.Rotate(Vector3.up, pi.Jright * horizontalSpeed * Time.fixedDeltaTime);
				tempEulerX -= pi.Jup * verticalSpeed * Time.fixedDeltaTime;
				tempEulerX = Mathf.Clamp(tempEulerX, -25, 15);

				cameraHandle.transform.localEulerAngles = new Vector3(tempEulerX, 0, 0);   //  縦の回転角を制限する

				model.transform.eulerAngles = tempModelEuler;

				transform.SetParent(cameraHandle.transform);

				//  位置を戻る
				transform.position = Vector3.Slerp(transform.position, cameraBackPos.transform.position, 0.05f);

				transform.rotation = Quaternion.Slerp(transform.rotation, cameraBackPos.transform.rotation, Time.fixedDeltaTime * 2.0f);
			}
			else if (pi.lockJumpStatus) //	is jumping
			{
				Vector3 tempModelEuler = model.transform.eulerAngles;
				playerHandle.transform.Rotate(Vector3.up, pi.Jright * horizontalSpeed * Time.fixedDeltaTime);
				tempEulerX -= pi.Jup * verticalSpeed * Time.fixedDeltaTime;
				tempEulerX = Mathf.Clamp(tempEulerX, -25, 15);

				cameraHandle.transform.localEulerAngles = new Vector3(tempEulerX, 0, 0);   //  縦の回転角を制限する

				model.transform.eulerAngles = tempModelEuler;
			}
		}
		else if (goal.gameClear)
		{
			transform.SetParent(null);
			cameraStatic = "GameClear";

			transform.position = Vector3.Slerp(transform.position, targetPos[6].transform.position, 0.2f);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetPos[6].transform.rotation, Time.fixedDeltaTime * 3.0f);
		}
		
		if (ac.isDead)
		{
			model.transform.localRotation = new Quaternion(0.0f, 180.0f, 90.0f, 0.0f);
			gameObject.GetComponent<Camera>().fieldOfView = 20.0f;
		}
	}
}
