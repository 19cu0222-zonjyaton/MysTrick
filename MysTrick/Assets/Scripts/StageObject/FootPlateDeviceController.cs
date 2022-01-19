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
	private bool lockFlag = false;			// ロックフラグ(重複ループ防止ため)

	void Update()
	{
		// デバイス起動開始
		if (canTrigger)
		{
			// カメラの切り替え時間開始
			if (cameraTimeCount > 0.0f)
			{
				cameraTimeCount -= Time.deltaTime;
			}
			// カメラの切り替え時間完了
			else
			{
				cameraTimeCount = 0.0f;
				isTriggered = true;
				canTrigger = false;
			}
		}

		// プレイヤーかボックスがオブジェクトと接触した場合、ポータルゲートを上昇開始
		if ((pc.isPlayer || mbc.isMoveBox) && !lockFlag)
		{
			this.transform.BroadcastMessage("DeviceOnTriggered", "sFootPlate");
			lockFlag = true;		// ロックする（ループしないよう）
			oc.isTrigger = true;
			if (isTriggered)
			{
				doubleTriggerA = true;
				doubleTriggerB = true;
			}
			else canTrigger = true;
		}
		// プレイヤーかボックスがオブジェクトから離れた場合、ポータルゲートを降下開始
		else if (!pc.isPlayer && !mbc.isMoveBox && lockFlag)
		{
			this.transform.BroadcastMessage("DeviceOnTriggered", "sFootPlate");
			lockFlag = false;		// ロック解除
			if (isTriggered)
			{
				doubleTriggerA = true;
				doubleTriggerB = true;
			}
			else canTrigger = true;
		}
	}
}
