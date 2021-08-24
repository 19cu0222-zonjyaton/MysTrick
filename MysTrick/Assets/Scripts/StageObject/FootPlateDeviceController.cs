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
	public float cameraTimeCount;

	[Header("===監視用===")]
	public bool isTriggered = false;
	public bool doubleTriggerA = false;		// portalA用フラグ
	public bool doubleTriggerB = false;		// portalB用フラグ
	private bool canTrigger = false;

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
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.transform.tag == "Player" || other.transform.tag == "MoveBox")
		{
			this.transform.BroadcastMessage("DeviceOnTriggered", "sFootPlate");
			if (isTriggered)
			{
				doubleTriggerA = true;
				doubleTriggerB = true;
			}
			else canTrigger = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.transform.tag == "Player" || other.transform.tag == "MoveBox")
		{
			this.transform.BroadcastMessage("DeviceOnTriggered", "sFootPlate");
			if (isTriggered)
			{
				doubleTriggerA = true;
				doubleTriggerB = true;
			}
			else canTrigger = true;
		}
	}


}
