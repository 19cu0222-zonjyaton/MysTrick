//-------------------------------------------------
// ファイル名		：ObjectTrigger.cs
// 概要				：オブジェクトのトリガーの制御
// 作成者			：鍾家同
// 更新内容			：2021/04/10 作成
//					：2021/04/16 更新　OnTriggerEnterをOnCollisionStayに変更
//					：2021/05/05 更新　Deviceに使った場合はOnTriggerStayを使用する
//-------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerController : MonoBehaviour
{
	public DoorController kDoor;
	public bool isTriggered;
	private PlayerInput Player;
	public GameObject hintUI;
	public float timeCount = 1.2f;              //	Triggerを出すまでの時間
	public GameObject[] handle;					//	Triggerのスイッチ

	private bool hadDone;						//	一回だけ実行する
	private bool cameraCanMoveToStair;          //	カメラ視点を移動し始めます
	private Vector3 tempHandlePos;				//	Handleの座標保存用

	void Awake()
	{
		Player = GameObject.Find("PlayerHandle").GetComponent<PlayerInput>();

		hintUI = transform.Find("hintUI").gameObject;

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
				// 子オブジェクトに受け渡すメッセージ
				this.transform.BroadcastMessage("DeviceOnTriggered", "sDevice");

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

	private void OnCollisionEnter(Collision other)
	{
		// 武器に当たり
		if (other.transform.tag == "Weapon")
		{
			if (this.transform.tag == "Device")
			{
				// 子オブジェクトに受け渡すメッセージ
				this.transform.BroadcastMessage("DeviceOnTriggered", "sFootPlate");

				// 子オブジェクトが存在し、階段が隠れた状態だったら、階段を示して稼働する
				if (this.transform.childCount > 1)
				{
					if (this.transform.GetChild(2).gameObject.activeSelf)
					{
						this.transform.GetChild(2).gameObject.SetActive(false);
					}
					else
					{
						if (!hadDone)
						{
							cameraCanMoveToStair = true;

							hadDone = true;
						}
						this.transform.GetChild(2).gameObject.SetActive(true);

						this.transform.BroadcastMessage("DeviceOnTriggered", "sCamera");
					}
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
			}

			if (this.transform.tag == "Key" && Player.isTriggered)
			{
				isTriggered = true;
				Debug.Log(this.transform.name + " has touched.");
				if (!kDoor.isTriggered) kDoor.isTriggered = true;
				//Destroy(this.gameObject);
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

				// 子オブジェクトが存在し、階段が隠れた状態だったら、階段を示して稼働する
				if (this.transform.childCount > 1)
				{
					if (this.transform.GetChild(2).gameObject.activeSelf)
					{
						this.transform.GetChild(2).gameObject.SetActive(false);
					}
					else
					{
						if (!hadDone)
						{
							if (this.gameObject.name == "sDevice002")
							{
								Destroy(GameObject.Find("TempCollider"));
							}

							cameraCanMoveToStair = true;

							hadDone = true;
						}
						this.transform.GetChild(2).gameObject.SetActive(true);

						this.transform.BroadcastMessage("DeviceOnTriggered", "sCamera");
					}
				}
				Player.isTriggered = false;
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
