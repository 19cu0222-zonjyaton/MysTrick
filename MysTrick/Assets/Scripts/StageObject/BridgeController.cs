//-------------------------------------------------
// ファイル名		：BridgeController.cs
// 概要				：橋の制御
// 作成者			：鍾家同
// 更新内容			：2021/04/12 作成
//					：2021/07/15 更新　各角度を変更できるように修正
//-------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeController : MonoBehaviour
{
	[Header("===変更用===")]
	public TriggerController Device;
	public Vector3 targetAng;
	public float speed = 5.0f;

	[Header("===監視用===")]
	public bool hasDone;				// カメラ用参数
	public bool isTriggered;			// オブジェクトを起動するフラグ
	private Quaternion targetEuAng;		// 目標角度（オイラー角）
	private Vector3 curAng;				// 現在角度
	private Vector3 nextAng;			// 目標角度
	private new AudioSource audio;
	private bool playOnce;
	private float timeCount = 3.6f;		//	Triggerを出すまでの時間
	private int pressCount = 0;			// 押し回数

	void Awake()
	{
		audio = gameObject.GetComponent<AudioSource>();
	}

	void Start()
	{
		curAng = this.transform.rotation.eulerAngles;
		nextAng = targetAng;
	}

	void Update()
	{
		if (Device.isTriggered)
		{
			isTriggered = true;
			timeCount -= Time.deltaTime;
			// 音を一度しか出さないようにする
			if (!playOnce)
			{
				audio.Play();
				playOnce = true;
			}
			// 回転開始
			if (timeCount <= 1.8f && timeCount > 0.0f)
			{
				targetEuAng = Quaternion.Euler(targetAng);
				// Quaternion.Slerp(Quaternion from, Quaternion to, deltaTime * speed)
				this.transform.rotation = Quaternion.Slerp(transform.rotation, targetEuAng, Time.deltaTime * speed);
			}
			// 回転停止、初期値に戻る
			else if(timeCount <= 0.0f)
			{
				++pressCount;
				++Device.launchCount;

				if (pressCount % 2 == 1) targetAng = curAng;
				else targetAng = nextAng;

				timeCount = 1.8f;
				playOnce = false;
				Device.isTriggered = false;
			}
		}
	}
}
