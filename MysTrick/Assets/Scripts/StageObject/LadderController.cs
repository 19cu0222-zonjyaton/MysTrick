//-------------------------------------------------
// ファイル名		：LadderController.cs
// 概要				：梯子の制御
// 作成者			：鍾家同
// 更新内容			：2021/06/15 作成
//-------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderController : MonoBehaviour
{
	[Header("===調整用===")]
	public TriggerController Device;
	public Vector3[] targetAngs;		// 回転角度
	[HideInInspector]
	public int i = 0;					// 要素数
	private Vector3 nextAng;			// 次の角度
	[HideInInspector]
	public bool canRotate;				// 回転開始フラグ
	public bool rotateFinish;			// 回転終了フラグ

	[Tooltip("The initial time for rotation")]
	public float timeCount;				// 回転時間（初期値）
	[Tooltip("The maximun time for rotation")]
	public float timeMax;				// 回転時間（最大値）
	private float timeReset;			// 回転時間（リセット）
	public float speed;					// 回転スピード
	public bool hasDone;				//	カメラ用参数

	void Start()
	{
		timeReset = 0.0f;
		nextAng = targetAngs[i + 1];
		canRotate = false;
	}

	void Update()
	{
		if (Device.isTriggered) canRotate = true;
		// 回転開始
		if (canRotate)
		{
			timeCount += Time.deltaTime;
			if (timeCount <= timeMax && timeCount >= 0.0f)
			{
				// Quaternion.Slerp(Quaternion From, Quaternion To, Speed * deltaTime)
				this.transform.localRotation = Quaternion.Slerp(this.transform.localRotation, Quaternion.Euler(nextAng), speed * Time.deltaTime);
			}
			// 回転停止
			else if (timeCount > timeMax)
			{
				// 角度の補正
				if (this.transform.localRotation != Quaternion.Euler(nextAng)) this.transform.localRotation = Quaternion.Euler(nextAng);
				// 初期値に戻す
				Device.isTriggered = false;
				canRotate = false;
				rotateFinish = true;
				timeCount = timeReset;
				i = (i + 1) % 4;
				// 要素数超えたら、初期値に戻す
				try
				{
					nextAng = targetAngs[i + 1];
				}
				catch (System.Exception)
				{
					nextAng = targetAngs[0];
				}
			}
		}
	}
}
