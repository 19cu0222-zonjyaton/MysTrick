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

	private GameObject cameraHandle;
	private GameObject playerHandle;
	private float tempEulerX;
	private GameObject model;
	private bool doOnce;                        //  ゴールに着いたら一回だけを実行するための参数
	private float countTime = 20.0f;            //  カメラ視角切り替えの時間
	private Vector3 relativePos;

	// Start is called before the first frame update
	void Awake()
	{
		cameraHandle = transform.parent.gameObject;

		playerHandle = cameraHandle.transform.parent.gameObject;

		model = playerHandle.GetComponent<ActorController>().model;

		goal = GameObject.Find("Goal").GetComponent<GoalController>();

		cameraBackPos = GameObject.Find("CameraPos");
	}

	// Update is called once per frame
	void LateUpdate()
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
			Vector3 tempModelEuler = model.transform.eulerAngles;

			playerHandle.transform.Rotate(Vector3.up, pi.Jright * horizontalSpeed * Time.fixedDeltaTime);
			tempEulerX -= pi.Jup * verticalSpeed * Time.fixedDeltaTime;
			tempEulerX = Mathf.Clamp(tempEulerX, -25, 15);

			cameraHandle.transform.localEulerAngles = new Vector3(tempEulerX, 0, 0);   //  縦の回転角を制限する

			model.transform.eulerAngles = tempModelEuler;
		}
		//else
		//{
		//    if (!doOnce)
		//    {
		//        transform.SetParent(null);

		//        //print(model.transform.forward.normalized);
		//        //transform.rotation = Quaternion.identity;
		//        transform.rotation = Quaternion.LookRotation(-model.transform.forward); //  カメラをプレイヤーの正方向に置く
		//        transform.Rotate(Vector3.right, 30.0f);                                 //  カメラをX軸に沿って30度を回転する
		//        //transform.LookRotation(model.transform);
		//        transform.localPosition = new Vector3(model.transform.position.x, model.transform.position.y, model.transform.position.z);
		//        transform.Translate(Vector3.forward * -15.0f);
		//        //model.transform.parent.transform.LookAt(transform);            

		//        doOnce = true;
		//        //transform.localScale = Vector3.one;
		//    }
			
		//}
	}

	//  カメラ移動関数
	private void cameraMove(Vector3 movePos, StairController stair, DoorController door, BridgeController bridge)
	{
		countTime -= Time.fixedDeltaTime;

		if (countTime <= 20.0f && countTime > 12.0f)
		{
			//  親関係を解除
			transform.parent = null;

			transform.localPosition = Vector3.Slerp(transform.localPosition, movePos, 0.01f);

			// 補完スピードを決める
			float speed = 0.05f;
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
		else if (countTime <= 8.0f && countTime >= 0.0f)
		{
			//  親関係になる
			transform.SetParent(cameraHandle.transform);

			//  位置を戻る
			transform.localPosition = Vector3.Slerp(transform.localPosition, cameraBackPos.transform.localPosition, 0.01f);

			//  角度を戻る
			transform.rotation = Quaternion.Slerp(transform.rotation, cameraBackPos.transform.rotation, Time.fixedDeltaTime * 1.0f);
		}
		else if (countTime < 0.0f)
		{
			cameraStatic = "Idle";

			countTime = 20.0f;

			pi.inputEnabled = true;
		}
	}
}
