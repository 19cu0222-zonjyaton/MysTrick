//-------------------------------------------------
// ファイル名		：TotemController.cs
// 概要				：トーテムの制御
// 作成者			：鍾家同
// 更新内容			：2021/08/23 作成
//-------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemController : MonoBehaviour
{
	[Header("===調整用===")]
	public float shootInterval;
	public GameObject fireBall;

	private TimeController timeController;
	private ObjectController objectController;

	void Start()
	{
		timeController = GetComponent<TimeController>();
		objectController = GetComponent<ObjectController>();
	}

	void Update()
	{
		// カウント終了すれば弾を発射する
		if (timeController.isFinish)
		{
			Instantiate(fireBall, this.transform.position + this.transform.right * -3.0f, Quaternion.Euler(this.transform.rotation.eulerAngles + new Vector3(0.0f, -90.0f, 0.0f)));
			timeController.TimeDelay(0.0f, shootInterval);
			timeController.isFinish = false;
		}
		// 回転している場合、カウントをストップする
		//else if(objectController.isTrigger) timeController.isStop = true;
		// カウント再開
		else timeController.isStop = false;
	}
}
