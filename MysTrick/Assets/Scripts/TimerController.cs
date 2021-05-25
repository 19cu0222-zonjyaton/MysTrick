//-------------------------------------------------
// ファイル名		：TimerController.cs
// 概要				：タイマーを設計
// 作成者			：鍾家同
// 更新内容			：2021/05/23 作成
//-------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerController : MonoBehaviour
{
	[Header("調整用")]
	public float timeMax;
	[Header("監視用")]
	public float timeCount;
	public bool TimerStart;		// タイマーをスタートするフラグ
	public bool TimerFinish;	// タイマーが終了するフラグ
	public bool TimerReset;		// タイマーをリセットするフラグ

	void Start()
	{
		TimerReset = false;
		TimerStart = false;
		TimerFinish = false;
	}

	void Update()
	{
		if (TimerStart) Timer();

		else timeCount = 0.0f;
	}

	private void Timer()
	{
		// タイマーをスタット
		if (timeCount <= timeMax && !TimerReset)
		{
			timeCount += Time.deltaTime;
		}
		// タイマーを復帰
		else
		{
			if (timeCount > timeMax) TimerFinish = true;
			timeCount = 0.0f;
			TimerStart = false;
		}
		//Debug.Log("Object Name: " + this.name + ", Timer: " + timeCount);
	}
}
