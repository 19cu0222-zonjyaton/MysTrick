//-------------------------------------------------
// ファイル名		：ObjectTrigger.cs
// 概要				：オブジェクトのトリガーの制御
// 作成者			：鍾家同
// 更新内容			：2021/04/10 作成
//					：2021/04/16 更新　OnTriggerEnterをOnCollisionStayに変更
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

	private bool hadDone;						//	一回だけ実行する
	private bool cameraCanMoveToStair;			//	カメラ視点を移動し始めます

	// Start is called before the first frame update
	void Awake()
	{
		Player = GameObject.Find("PlayerHandle").GetComponent<PlayerInput>();

		hintUI = transform.Find("hintUI").gameObject;
	}

	// Update is called once per frame
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
	}

	private void OnCollisionStay(Collision other)
	{
		if (other.transform.tag == "Player")
		{
			hintUI.SetActive(true);

			if (this.transform.tag == "Device" && Player.isTriggered)
			{
				isTriggered = true;
				Debug.Log(this.transform.name + " has touched.");
				if (this.transform.childCount > 1)
				{
					if (this.transform.GetChild(0).gameObject.activeSelf)
					{
						this.transform.GetChild(0).gameObject.SetActive(false);
					}
					else
					{
						if (!hadDone)
						{
							cameraCanMoveToStair = true;

							hadDone = true;
						}
						this.transform.GetChild(0).gameObject.SetActive(true);

					this.transform.BroadcastMessage("DeviceOnTriggered", "sCamera");
					}
				}
				Player.isTriggered = false;
			}
			if (this.transform.tag == "Key" && Player.isTriggered)
			{
				isTriggered = true;
				Debug.Log(this.transform.name + " has touched.");
				if (!kDoor.isTriggered) kDoor.isTriggered = true;
				Destroy(this.gameObject);
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
}
