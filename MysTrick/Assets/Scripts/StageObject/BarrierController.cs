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
	public bool stickCanMove;				// 針は移動可能フラグ
	public float moveSpeed;					// 移動スピード
	public float stopTimeCount;				// 移動経過時間
	public float cameraSwitchTime = 1.5f;	// カメラの切り替え時間

	
	[Header("===監視用===")]
	[SerializeField]
	private bool isTriggered;
	[SerializeField]
	private bool right;
	private int count;
	private bool toStop;
	private float stopTimeReset;
	private Vector3 nextPosition;
	private Vector3 currentPosition;

	void Start()
	{
		stickCanMove = true;
		isTriggered = false;
		right = true;
		toStop = false;
		count = 0;
		nextPosition = targetXB.position;
		stopTimeReset = stopTimeCount;
	}

	void Update()
	{
		//	プレイヤーがダメージを受けた後一定時間過ぎたら動けるにする
		if (stickCanMove)
		{
			// デバイスを作動する時
			if (Device != null && Device.isTriggered)
			{
				if (cameraSwitchTime > 0.0f)
				{
					cameraSwitchTime -= Time.deltaTime;
					toStop = true;
				}
				else
				{
					isTriggered = true;
					//toStop = false;
					if (count == 0 && right)
					{
						++Device.launchCount;
						right = false;
					}
					else if (count == 0 && !right)
					{
						++Device.launchCount;
						right = true;
					}
					++count;
				}
			}
			// 障害物を右へ移動する時
			if (isTriggered && right)
			{
				// 移動中
				if (this.transform.localPosition != new Vector3(currentPosition.x, currentPosition.y, targetXA.localPosition.z)) this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, 
					new Vector3(currentPosition.x, currentPosition.y, targetXA.localPosition.z), moveSpeed * 1.5f * Time.deltaTime);
				// 上下移動に切り替えるまでの準備
				else
				{
					isTriggered = false;
					Device.isTriggered = false;
					count = 0;
					nextPosition = targetXB.localPosition;
					stopTimeCount = stopTimeReset;
					cameraSwitchTime = 0.0f;
					toStop = false;
				}
			}
			// 障害物を左へ移動する時
			else if (isTriggered && !right)
			{
				// 移動中
				if (this.transform.localPosition != new Vector3(currentPosition.x, currentPosition.y, targetYA.localPosition.z)) this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition,
					new Vector3(currentPosition.x, currentPosition.y, targetYA.localPosition.z), moveSpeed * 1.5f * Time.deltaTime);
				// 上下移動に切り替えるまでの準備
				else
				{
					isTriggered = false;
					Device.isTriggered = false;
					count = 0;
					nextPosition = targetYB.localPosition;
					stopTimeCount = stopTimeReset;
					cameraSwitchTime = 0.0f;
					toStop = false;
				}
			}
			// 障害物が右に上下移動する時
			else if (!isTriggered && right && !toStop)
			{
				// 停止中
				if (stopTimeCount > 0.0f) stopTimeCount -= Time.deltaTime;
				// 移動中
				else if (this.transform.localPosition != nextPosition) this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, nextPosition, moveSpeed * 1.5f * Time.deltaTime);
				// 上(下)移動に切り替え
				else
				{
					if (nextPosition == targetXB.localPosition) nextPosition = targetXA.localPosition;
					else nextPosition = targetXB.localPosition;
					stopTimeCount = stopTimeReset;
				}
				currentPosition = this.transform.localPosition;
			}
			// 障害物が左に上下移動する時
			else if (!isTriggered && !right && !toStop)
			{
				// 停止中
				if (stopTimeCount > 0.0f) stopTimeCount -= Time.deltaTime;
				// 移動中
				else if (this.transform.localPosition != nextPosition) this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, nextPosition, moveSpeed * 1.5f * Time.deltaTime);
				// 上(下)移動に切り替え
				else
				{
					if (nextPosition == targetYB.localPosition) nextPosition = targetYA.localPosition;
					else nextPosition = targetYB.localPosition;
					stopTimeCount = stopTimeReset;
				}
				currentPosition = this.transform.localPosition;
			}
		}
	}

	// 当たり判定
	//----------------------------------
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
	//----------------------------------
}
