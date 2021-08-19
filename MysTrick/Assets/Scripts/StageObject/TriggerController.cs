
//-------------------------------------------------
// ファイル名		：ObjectTrigger.cs
// 概要				：オブジェクトのトリガーの制御
// 作成者			：鍾家同
// 更新内容			：2021/04/10 作成
//					：2021/04/16 更新　OnTriggerEnterをOnCollisionStayに変更
//					：2021/05/05 更新　踏み台に使った場合はOnTriggerStayを使用する
//					：2021/06/14 更新　武器に使った場合はOnTriggerEnterを使用する
//-------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerController : MonoBehaviour
{
	[Header("===調整用===")]
	public DoorController kDoor;
	public StairController Stair;
	public TimeController timeController;
	public GameObject hintUI;
	public float timeCount = 1.2f;				//	Triggerを出すまでの時間
	public GameObject handle;					//	Triggerのスイッチ
	private bool hadDone;						//	一回だけ実行する
	private bool cameraCanMoveToStair;			//	カメラ視点を移動し始めます

	[Header("===監視用===")]
	public bool isTriggered;
	public bool isCameraTriggered;
	public int launchCount;						//	Triggerの状態(0 -> 最初の状態 1 -> 発動した状態)
	private PlayerInput Player;

	void Awake()
	{
		Player = GameObject.Find("PlayerHandle").GetComponent<PlayerInput>();
	}

	void Update()
	{
		if (cameraCanMoveToStair)
		{
			timeCount -= Time.deltaTime;

			if (timeCount <= 0.0f)
			{
				cameraCanMoveToStair = false;

				timeCount = 1.2f;
			}
		}

		if (isTriggered)	//	Handleの動き
		{
			if (handle != null)
			{
				if (launchCount % 2 == 0)
				{
					if (handle.transform.gameObject.tag == "Handle")
					{
						handle.transform.localRotation = Quaternion.Lerp(handle.transform.localRotation, Quaternion.Euler(0.0f, 0.0f, -70.0f), 3.0f * Time.deltaTime);
					}
					else if (handle.transform.gameObject.tag == "Key")
					{
						handle.transform.localPosition = Vector3.MoveTowards(handle.transform.localPosition, new Vector3(0.0f, -0.5f, 0.0f), 3.0f * Time.deltaTime);
					}
				}
				else
				{
					if (handle.transform.gameObject.tag == "Handle")
					{
						handle.transform.localRotation = Quaternion.Lerp(handle.transform.localRotation, Quaternion.Euler(0.0f, 0.0f, 0.0f), 3.0f * Time.deltaTime);
					}
					else if (handle.transform.gameObject.tag == "Key")
					{
						handle.transform.localPosition = Vector3.MoveTowards(handle.transform.localPosition, new Vector3(0.0f, 0.0f, 0.0f), 3.0f * Time.deltaTime);

					}
				}
			}
		}
	}

	//private void OnCollisionStay(Collision other)       //	プレイヤーが動いていない場合OnCollisionStayは呼び出されない
	//{
	//    if (other.transform.tag == "Player")
	//    {
	//        hintUI.SetActive(true);

	//        if (this.transform.tag == "Device" && Player.isTriggered)
	//        {
	//            isTriggered = true;
	//Player.isTriggered = false;

	//Debug.Log(this.transform.name + " has touched.");
	//        }
	//    }
	//}

	//private void OnCollisionExit(Collision other)
	//{
	//	if (other.transform.tag == "Player")
	//	{
	//		hintUI.SetActive(false);
	//	}
	//}

	// 当たり判定
	//----------------------------------
	private void OnTriggerEnter(Collider other)
	{
		// 武器に当たり
		if (other.transform.tag == "Weapon")
		{
			if (this.transform.tag == "Device")
			{
				// 子オブジェクトに受け渡すメッセージ
				this.transform.BroadcastMessage("DeviceOnTriggered", "sFootPlate");

				isTriggered = true;

				// TimerControllerを使用（武器に二重当たられることを防止）
				try			// TimerControllerが的確に設置したか
				{
					// カウント終了
					if (timeController.isFinish)
					{
						//// 表示になった場合
						//if (Stair.transform.gameObject.activeSelf)
						//{
						//	Stair.transform.gameObject.SetActive(false);
						//}
						// 表示になってない場合
						//else
						//{
							Stair.transform.gameObject.SetActive(true);
							isCameraTriggered = true;

							if (!hadDone)
							{
								cameraCanMoveToStair = true;
								hadDone = true;
							}
						//}
						timeController.isFinish = false;
						timeController.TimeDelay(0.0f, 0.5f);
					}
				}
				catch (System.Exception)
				{
					Debug.Log("Didn't find out TimeController!!");
				}
			}
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.transform.tag == "Player")
		{
			hintUI.SetActive(true);

			if (this.transform.tag == "Device" && Player.isTriggered)
			{
				isTriggered = true;
				//Debug.Log(this.transform.name + " has touched.");

				// 子オブジェクトに受け渡すメッセージ
				this.transform.BroadcastMessage("DeviceOnTriggered", "sFootPlate");

				Player.isTriggered = false;

				//// 表示になった場合
				//if (Stair.transform.gameObject.activeSelf)
				//{
				//	Stair.transform.gameObject.SetActive(false);
				//}
				//// 表示になってない場合
				//else
				//{
					Stair.transform.gameObject.SetActive(true);
					isCameraTriggered = true;

					if (!hadDone)
					{
						if (this.gameObject.name == "sDevice002")
						{
							Destroy(GameObject.Find("TempCollider"));
						}
						cameraCanMoveToStair = true;
						hadDone = true;
					}
				//}
			}

			if (this.transform.tag == "Handle" && Player.isTriggered)
			{
				isTriggered = true;
				Player.isTriggered = false;
				Debug.Log(this.transform.name + " has touched.");
			}

			if (this.transform.tag == "Key" && Player.isTriggered)
			{
				isTriggered = true;
				Player.isTriggered = false;
				Debug.Log(this.transform.name + " has touched.");
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.transform.tag == "Player")
		{
			hintUI.SetActive(false);
		}
	}
	//----------------------------------
}
