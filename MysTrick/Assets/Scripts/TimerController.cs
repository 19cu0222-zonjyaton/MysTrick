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
	public float timeStart;
	public float timeMax;
	[Header("監視用")]
	[SerializeField]
	private bool timeFinished;
	[SerializeField]
	private bool isTimeStart;

	void Start()
	{
		timeFinished = false;
		isTimeStart = false;
	}

	void Update()
	{
		if (isTimeStart)
		{
			Timer(timeStart, timeMax);
		}
		Debug.Log("timeFinished:" + timeFinished);
	}

	public void Timer(float timeCount,float _timeMax)
	{
		if (timeCount <= _timeMax)
		{
			timeCount += Time.deltaTime;
		}
		else
		{
			timeCount = 0.0f;
		}
	}
}
