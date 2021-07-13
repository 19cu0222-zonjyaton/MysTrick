//-------------------------------------------------
// ファイル名		：TimeController.cs
// 概要				：Timerの制御
// 作成者			：鍾家同
// 更新内容			：2021/06/14 作成
//-------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
	[Header("===監視用===")]
	public bool isFinish;			// カウント終了フラグ
	private bool isStart;			// カウント開始フラグ
	private float timeStart;		// 初期値
	private float timeMax;			// 最大値
	private float speed;			// カウントスピード

	void Start()
	{
		isStart = false;
		isFinish = true;
	}

	void Update()
	{
		// カウント開始
		if (isStart)
		{
			timeStart += Time.deltaTime * speed;
			if (timeStart >= timeMax)
			{
				isFinish = true;
				isStart = false;
			}
		}
	}

	// void TimeDelay(float Start, float Maximum, float TimeSpeed)
	public void TimeDelay(float _timeStart, float _timeMax, float _speed = 1.0f)
	{
		if (!isStart)				// カウント開始（カウント終了までに再カウント防止）
		{
			isStart = true;
			timeStart = _timeStart;
			timeMax = _timeMax;
			speed = _speed;
		}
	}
}
