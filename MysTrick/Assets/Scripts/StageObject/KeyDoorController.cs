//-------------------------------------------------
// ファイル名		：KeyDoorController.cs
// 概要				：鍵付き扉の制御
// 作成者			：鍾家同
// 更新内容			：2021/07/19 作成
//-------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyDoorController : MonoBehaviour
{
	// 扉の色確認
	private enum DoorColor
	{
		Blue,
		Green,
	}
	[Header("===調整用===")]
	[SerializeField]
	private DoorColor doorColor;

	public GameObject doorLock1;
	public GameObject doorLock2;
	public GameObject doorBolt1;
	public GameObject doorBolt2;
	public GameObject LeftDoor;
	public GameObject RightDoor;
	public GameObject HintUI;
	public float CameraTimeCount;				// カメラ移動に所要時間
	public float lockOpenTimeCount = 1.5f;		// ロックを開ける所要時間
	public float lockOpenSpeed = 1.0f;			// ロックを開けるスピード
	public float doorOpenTimeCount = 1.0f;		// 扉を開ける所要時間
	public float doorOpenSpeed = 2.0f;			// 扉を開けるスピード

	[Header("===監視用===")]
	[SerializeField]
	private bool canOpen = false;				// 開けるフラグ
	private PlayerInput Player;

	void Awake()
	{
		Player = GameObject.FindWithTag("Player").GetComponent<PlayerInput>();
	}

	void Update()
	{
		if (canOpen) OpenDoor();
	}

	void OpenDoor()
	{
		if (CameraTimeCount > 0.0f) CameraTimeCount -= Time.deltaTime;
		else if (CameraTimeCount < 0.0f) CameraTimeCount = 0.0f;
		else if (lockOpenTimeCount > 0.0f)
		{
			lockOpenTimeCount -= Time.deltaTime;
			if (doorLock1 != null) Destroy(doorLock1.gameObject);
			doorLock2.GetComponent<Rigidbody>().useGravity = true;
			doorLock2.GetComponent<BoxCollider>().enabled = true;
		}
		else if (lockOpenTimeCount < 0.0f)
		{
			lockOpenTimeCount = 0.0f;
			if (doorLock2 != null) Destroy(doorLock2.gameObject);
			if (doorBolt1 != null) Destroy(doorBolt1.gameObject);
			if (doorBolt2 != null) Destroy(doorBolt2.gameObject);
		}
		else if (lockOpenTimeCount == 0.0f)
		{
			if (doorOpenTimeCount > 0.0f)
			{
				doorOpenTimeCount -= Time.deltaTime;
				if (doorColor == DoorColor.Blue)
				{
					LeftDoor.transform.localPosition = Vector3.MoveTowards(LeftDoor.transform.localPosition,
						new Vector3(LeftDoor.transform.localPosition.x, LeftDoor.transform.localPosition.y, LeftDoor.transform.localPosition.z + 1.7f), doorOpenSpeed * Time.deltaTime);
					RightDoor.transform.localPosition = Vector3.MoveTowards(RightDoor.transform.localPosition,
						new Vector3(RightDoor.transform.localPosition.x, RightDoor.transform.localPosition.y, RightDoor.transform.localPosition.z - 1.7f), doorOpenSpeed * Time.deltaTime);
				}
				else if (doorColor == DoorColor.Green)
				{
					LeftDoor.transform.localPosition = Vector3.MoveTowards(LeftDoor.transform.localPosition,
						new Vector3(LeftDoor.transform.localPosition.x, LeftDoor.transform.localPosition.y, LeftDoor.transform.localPosition.z - 1.7f), doorOpenSpeed * Time.deltaTime);
					RightDoor.transform.localPosition = Vector3.MoveTowards(RightDoor.transform.localPosition,
						new Vector3(RightDoor.transform.localPosition.x, RightDoor.transform.localPosition.y, RightDoor.transform.localPosition.z + 1.7f), doorOpenSpeed * Time.deltaTime);
				}
			}
			else if (doorOpenTimeCount < 0.0f)
			{
				doorOpenTimeCount = 0.0f;
				if (LeftDoor != null) Destroy(LeftDoor.gameObject);
				if (RightDoor != null) Destroy(RightDoor.gameObject);
			}
		}
	}

	void OnTriggerStay(Collider other)
	{
		if (other.transform.tag == "Player")
		{
			if (!canOpen)
			{
				HintUI.SetActive(true);
			}
			else
			{
				HintUI.SetActive(false);
			}

			if (doorColor == DoorColor.Blue && other.GetComponent<ActorController>().haveKeys.BlueKey == true && Player.isTriggered)
			{
				canOpen = true;
			}
			else if (doorColor == DoorColor.Green && other.GetComponent<ActorController>().haveKeys.GreenKey == true && Player.isTriggered)
			{
				canOpen = true;
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.transform.tag == "Player")
		{
			HintUI.SetActive(false);
		}
	}
}
