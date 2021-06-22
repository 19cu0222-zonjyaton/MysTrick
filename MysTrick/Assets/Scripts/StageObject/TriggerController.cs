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
	public GameObject[] handle;					//	Triggerのスイッチ

	private bool hadDone;						//	一回だけ実行する
	private bool cameraCanMoveToStair;			//	カメラ視点を移動し始めます
	private Vector3 tempHandlePos;				//	Handleの座標保存用

	[Header("===監視用===")]
	public bool isTriggered;
	public bool isStairTriggered;
	public bool isCameraTriggered;
	
	private PlayerInput Player;
	void Awake()
	{
		Player = GameObject.Find("PlayerHandle").GetComponent<PlayerInput>();
		//hintUI = transform.Find("hintUI").gameObject;

		if (handle.Length != 0 && transform.gameObject.tag == "Key")
		{
			tempHandlePos = handle[0].transform.position - new Vector3(0.0f, 0.5f, 0.0f);		//	Keyを押す距離
		}
	}

	void Update()
	{
		if (cameraCanMoveToStair)
		{
			timeCount -= Time.deltaTime;

			if (timeCount <= 0.0f)
			{
				isStairTriggered = true;

				cameraCanMoveToStair = false;

				timeCount = 1.2f;
			}
		}

		if (isTriggered)
		{
			if (handle.Length != 0)
			{
				if (transform.gameObject.tag == "Device")
				{
					handle[0].transform.rotation = Quaternion.Lerp(handle[0].transform.rotation, handle[1].transform.rotation, Time.fixedDeltaTime * 3.0f);
				}
				else if (transform.gameObject.tag == "Key")
				{
					handle[0].transform.position = Vector3.Lerp(handle[0].transform.position, tempHandlePos, 0.3f);
				}
			}
		}
	}

	private void OnCollisionStay(Collision other)
	{
		if (other.transform.tag == "Player")
		{
			hintUI.SetActive(true);

			if (this.transform.tag == "Device" && Player.isTriggered)
			{
				isTriggered = true;
				Player.isTriggered = false;
				Debug.Log(this.transform.name + " has touched.");
			}

			if (this.transform.tag == "Key" && Player.isTriggered)
			{
				isTriggered = true;
				if (!kDoor.isTriggered) kDoor.isTriggered = true;
				Debug.Log(this.transform.name + " has touched.");
			}
		}
	}

	private void OnCollisionExit(Collision other)
	{
		if (other.transform.tag == "Player")
		{
			hintUI.SetActive(false);
		}
	}

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
						// 表示になった場合
						if (Stair.transform.gameObject.activeSelf)
						{
							Stair.transform.gameObject.SetActive(false);
						}
						// 表示になってない場合
						else
						{
							Stair.transform.gameObject.SetActive(true);
							isCameraTriggered = true;

							if (!hadDone)
							{
								cameraCanMoveToStair = true;
								hadDone = true;
							}
						}
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

				// 表示になった場合
				if (Stair.transform.gameObject.activeSelf)
				{
					Stair.transform.gameObject.SetActive(false);
				}
				// 表示になってない場合
				else
				{
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
				}
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
}
