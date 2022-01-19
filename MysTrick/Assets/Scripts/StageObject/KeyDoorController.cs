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
	public bool MoveXPos;

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

	// 開門行動
	void OpenDoor()
	{
		// カメラの切り替え時間開始
		if (CameraTimeCount > 0.0f) CameraTimeCount -= Time.deltaTime;
		// カメラの切り替え時間停止
		else if (CameraTimeCount < 0.0f) CameraTimeCount = 0.0f;
		// ロックを解除開始
		else if (lockOpenTimeCount > 0.0f)
		{
			lockOpenTimeCount -= Time.deltaTime;
			if (doorLock1 != null) Destroy(doorLock1.gameObject);
			doorLock2.GetComponent<Rigidbody>().useGravity = true;
			doorLock2.GetComponent<BoxCollider>().enabled = true;
		}
		// 解除完了した場合、ドアロックとドアボルトを削除
		else if (lockOpenTimeCount < 0.0f)
		{
			lockOpenTimeCount = 0.0f;
			if (doorLock2 != null) Destroy(doorLock2.gameObject);
			if (doorBolt1 != null) Destroy(doorBolt1.gameObject);
			if (doorBolt2 != null) Destroy(doorBolt2.gameObject);
		}
		// 解除完了した場合、扉を開ける
		else if (lockOpenTimeCount == 0.0f)
		{
			// 扉を移動開始
			if (doorOpenTimeCount > 0.0f)
			{
				doorOpenTimeCount -= Time.deltaTime;
				if (doorColor == DoorColor.Blue)
				{
					if (!MoveXPos)
					{
						// 扉を左右へ移動する
						LeftDoor.transform.localPosition = Vector3.MoveTowards(LeftDoor.transform.localPosition,
							new Vector3(LeftDoor.transform.localPosition.x, LeftDoor.transform.localPosition.y, LeftDoor.transform.localPosition.z + 1.7f), doorOpenSpeed * Time.deltaTime);
						RightDoor.transform.localPosition = Vector3.MoveTowards(RightDoor.transform.localPosition,
							new Vector3(RightDoor.transform.localPosition.x, RightDoor.transform.localPosition.y, RightDoor.transform.localPosition.z - 1.7f), doorOpenSpeed * Time.deltaTime);
					}
					else
					{
						// 扉を左右へ移動する
						LeftDoor.transform.localPosition = Vector3.MoveTowards(LeftDoor.transform.localPosition,
							new Vector3(LeftDoor.transform.localPosition.x + 1.7f, LeftDoor.transform.localPosition.y, LeftDoor.transform.localPosition.z), doorOpenSpeed * Time.deltaTime);
						RightDoor.transform.localPosition = Vector3.MoveTowards(RightDoor.transform.localPosition,
							new Vector3(RightDoor.transform.localPosition.x - 1.7f, RightDoor.transform.localPosition.y, RightDoor.transform.localPosition.z), doorOpenSpeed * Time.deltaTime);

					}
				}
				else if (doorColor == DoorColor.Green)
				{
					// 扉を左右へ移動する
					LeftDoor.transform.localPosition = Vector3.MoveTowards(LeftDoor.transform.localPosition,
						new Vector3(LeftDoor.transform.localPosition.x, LeftDoor.transform.localPosition.y, LeftDoor.transform.localPosition.z - 1.7f), doorOpenSpeed * Time.deltaTime);
					RightDoor.transform.localPosition = Vector3.MoveTowards(RightDoor.transform.localPosition,
						new Vector3(RightDoor.transform.localPosition.x, RightDoor.transform.localPosition.y, RightDoor.transform.localPosition.z + 1.7f), doorOpenSpeed * Time.deltaTime);
				}
			}
			// 扉を移動完了した場合、扉を削除
			else if (doorOpenTimeCount < 0.0f)
			{
				doorOpenTimeCount = 0.0f;
				if (LeftDoor != null) Destroy(LeftDoor.gameObject);
				if (RightDoor != null) Destroy(RightDoor.gameObject);
			}
		}
	}

	// 当たり判定
	//----------------------------------
	void OnTriggerStay(Collider other)
	{
		if (other.transform.tag == "Player")
		{
			// 扉が開けられない場合、hintUIを隠す
			if (!canOpen)
			{
				HintUI.SetActive(true);
			}
			// 扉が開けられる場合、hintUIを現す
			else
			{
				HintUI.SetActive(false);
			}

			// プレイヤーが青いキーを持っている状態で、扉が青い扉且デバイスが起動した場合、開門開始
			if (doorColor == DoorColor.Blue && other.GetComponent<ActorController>().haveKeys.BlueKey == true && Player.isTriggered)
			{
				canOpen = true;
			}
			// プレイヤーが緑色キーを持っている状態で、扉が緑色扉且デバイスが起動した場合、開門開始
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
	//----------------------------------
}
