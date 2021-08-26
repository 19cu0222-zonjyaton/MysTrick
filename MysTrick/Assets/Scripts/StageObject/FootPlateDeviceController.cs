//-------------------------------------------------
// ファイル名		：FootPlateDeviceController.cs
// 概要				：足掛けの制御
// 作成者			：鍾家同
// 更新内容			：2021/08/22
//-------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootPlateDeviceController : MonoBehaviour
{
	[Header("===調整用===")]
	public ObjectController oc;
	public float cameraTimeCount;
	public FootPlate_MoveBoxCheck mbc;
	public FootPlate_PlayerCheck pc;

	[Header("===監視用===")]
	public bool isTriggered = false;
	public bool doubleTriggerA = false;		// portalA用フラグ
	public bool doubleTriggerB = false;		// portalB用フラグ
	private bool canTrigger = false;
	private bool lockFlag;

	void Update()
	{
		if (canTrigger)
		{
			if (cameraTimeCount > 0.0f)
			{
				cameraTimeCount -= Time.deltaTime;
			}
			else
			{
				cameraTimeCount = 0.0f;
				isTriggered = true;
				canTrigger = false;
			}
		}

		if ((pc.isPlayer || mbc.isMoveBox) && !lockFlag)
		{
			this.transform.BroadcastMessage("DeviceOnTriggered", "sFootPlate");
			lockFlag = true;
			oc.isTrigger = true;
			if (isTriggered)
			{
				doubleTriggerA = true;
				doubleTriggerB = true;
			}
			else canTrigger = true;
		}
		else if (!(pc.isPlayer) && !(mbc.isMoveBox) && lockFlag)
		{
			print("aaaaaaaaaaaaaa");
			this.transform.BroadcastMessage("DeviceOnTriggered", "sFootPlate");
			lockFlag = false;
			if (isTriggered)
			{
				doubleTriggerA = true;
				doubleTriggerB = true;
			}
			else canTrigger = true;
		}
	}

	//void OnTriggerEnter(Collider other)
	//{
	//	if ((other.transform.tag == "Player" || other.transform.tag == "MoveBox") && !lockFlag)
	//	{
	//			print(other.transform.name);
	//			lockFlag = true;
	//			this.transform.BroadcastMessage("DeviceOnTriggered", "sFootPlate");
	//			oc.isTrigger = true;
	//			if (isTriggered)
	//			{
	//				doubleTriggerA = true;
	//				doubleTriggerB = true;
	//			}
	//			else canTrigger = true;
	//	}
	//}

	//void OnTriggerExit(Collider other)
	//{
	//	if ((other.transform.tag == "Player" || other.transform.tag == "MoveBox") && lockFlag)
	//	{
	//		lockFlag = false;
	//		this.transform.BroadcastMessage("DeviceOnTriggered", "sFootPlate");
	//		if (isTriggered)
	//		{
	//			doubleTriggerA = true;
	//			doubleTriggerB = true;
	//		}
	//		else canTrigger = true;
	//		Debug.Log("GD!");
	//	}
	//}
}
