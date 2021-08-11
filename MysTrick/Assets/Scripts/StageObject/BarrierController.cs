//-------------------------------------------------
// ファイル名		：BarrierController.cs
// 概要				：障害物の制御
// 作成者			：鍾家同
// 更新内容			：2021/08/02 作成
//-------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierController : MonoBehaviour
{
	[Header("===調整用===")]
	public TriggerController Device;
	public Transform targetXA;				// 右A点
	public Transform targetXB;				// 右B点
	public Transform targetYA;				// 左A点
	public Transform targetYB;				// 左B点
	public bool stickCanMove;               // 針は移動可能フラグ
	public bool cameraMoveStart;			//	カメラ移動フラグ
	public bool hasDone;					//	カメラプロパティ
	public float moveSpeed;					// 移動スピード
	public float moveTimeCount;				// 移動経過時間
	public float cameraSwitchTime = 1.5f;	// カメラの切り替え時間
	public float stopTime = 1.0f;			// 停止時間

	private int count;
	[Header("===監視用===")]
	[SerializeField]
	private bool isTriggered;
	[SerializeField]
	private bool right;
	private float moveTimeReset;
	private float cameraSwitchTimeReset;
	private Vector3 nextPosition;

	void Start()
	{
		stickCanMove = true;
		isTriggered = false;
		right = true;
		count = 0;
		nextPosition = targetXB.position;
		moveTimeReset = moveTimeCount;
		cameraSwitchTimeReset = cameraSwitchTime;
	}

	void Update()
	{
		//	プレイヤーがダメージを受けた後一定時間過ぎたら動けるにする
		if (stickCanMove)
		{
			// デバイスを作動する時
			if (Device != null && Device.isTriggered)
			{
				cameraMoveStart = true;
				if (cameraSwitchTime > 0.0f)
				{
					cameraSwitchTime -= Time.deltaTime;
				} 
				else
				{
					isTriggered = true;
				}

				++count;
				if (count == 2 && right)
				{
					++Device.launchCount;
					right = false;
				}
				else if (count == 2 && !right)
				{
					++Device.launchCount;
					right = true;
				}
			}
			// 障害物を右へ移動する時
			if (isTriggered && right)
			{
				// 移動中
				if (this.transform.localPosition != targetXA.localPosition) this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, targetXA.localPosition, moveSpeed * 1.5f * Time.deltaTime);
				// 上下移動に切り替えるまでの準備
				else
				{
					isTriggered = false;
					Device.isTriggered = false;
					count = 0;
					nextPosition = targetXB.localPosition;
					moveTimeCount = moveTimeReset + stopTime;
					cameraSwitchTime = 0.0f;
				}
			}
			// 障害物を左へ移動する時
			else if (isTriggered && !right)
			{
				// 移動中
				if (this.transform.localPosition != targetYA.localPosition) this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, targetYA.localPosition, moveSpeed * 1.5f * Time.deltaTime);
				// 上下移動に切り替えるまでの準備
				else
				{
					isTriggered = false;
					Device.isTriggered = false;
					count = 0;
					nextPosition = targetYB.localPosition;
					moveTimeCount = moveTimeReset + stopTime;
					cameraSwitchTime = 0.0f;
				}
			}
			// 障害物が右に上下移動する時
			else if (!isTriggered && right)
			{
				// 移動中
				if (moveTimeCount > 0.0f)
				{
					moveTimeCount -= Time.deltaTime;
					if (moveTimeCount < 3.0f) this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, nextPosition, moveSpeed * 1.5f * Time.deltaTime);
				}
				// 上(下)移動に切り替え
				else
				{
					if (nextPosition == targetXB.localPosition) nextPosition = targetXA.localPosition;
					else nextPosition = targetXB.localPosition;
					moveTimeCount = moveTimeReset;
				}
			}
			// 障害物が左に上下移動する時
			else if (!isTriggered && !right)
			{
				// 移動中
				if (moveTimeCount > 0.0f)
				{
					moveTimeCount -= Time.deltaTime;
					if (moveTimeCount < 3.0f) this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, nextPosition, moveSpeed * 1.5f * Time.deltaTime);
				}
				// 上(下)移動に切り替え
				else
				{
					if (nextPosition == targetYB.localPosition) nextPosition = targetYA.localPosition;
					else nextPosition = targetYB.localPosition;
					moveTimeCount = moveTimeReset;
				}
			}
		}
	}

	void OnCollisionStay(Collision other)
	{
		if (other.transform.tag == "Player")
		{
			other.transform.SetParent(this.gameObject.transform);
		}
	}
	private void OnCollisionExit(Collision other)
	{
		if (other.transform.tag == "Player")
		{
			other.transform.SetParent(null);
		}
	}
}
