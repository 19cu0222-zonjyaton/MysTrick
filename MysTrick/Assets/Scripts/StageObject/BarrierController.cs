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
	public Transform targetRA;				// 右A点
	public Transform targetRB;				// 右B点
	public Transform targetLA;				// 左A点
	public Transform targetLB;				// 左B点
	public bool stickCanMove;				// 針は移動可能フラグ
	public bool right = true;               // 最初位置(右か左か)
	public bool hasDone;                   // 
	public bool fromZtoX;					// 復帰する順番
	public bool fromXtoZ;					// 復帰する順番
	public float moveSpeed = 5.0f;			// 移動スピード
	public float stopTimeCount = 1.5f;		// 移動経過時間
	public float cameraSwitchTime = 1.5f;	// カメラの切り替え時間

	
	[Header("===監視用===")]
	[SerializeField]
	private bool isTriggered;
	private int count;
	private bool toStop;
	private bool _fromZtoX;
	private bool _fromXtoZ;
	private float stopTimeReset;
	private float moveInterval = 0.5f;
	private Vector3 nextPosition;
	private bool handleFlag;

	void Start()
	{
		stickCanMove = true;
		isTriggered = false;
		toStop = false;
		_fromZtoX = fromZtoX;
		_fromXtoZ = fromXtoZ;
		count = 0;
		nextPosition = right ? targetRB.localPosition : targetLB.localPosition;
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
				if (!handleFlag)
				{
					++Device.launchCount;
					handleFlag = true;
				}

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
						right = false;
					}
					else if (count == 0 && !right)
					{
						right = true;
					}
					++count;
				}
			}
			// 障害物を右へ移動する時
			if (isTriggered && right)
			{
				// 移動中(X軸を復帰->Z軸を復帰)
				if (fromXtoZ)
				{
					if (this.transform.localPosition.x != targetLA.localPosition.x)
						this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, targetLA.localPosition, moveSpeed * 1.5f * Time.deltaTime);
					else if (this.transform.localPosition.z != targetRA.localPosition.z)
					{
						if (moveInterval > 0.0f) moveInterval -= Time.deltaTime;
						else
						{
							this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, targetRA.localPosition, moveSpeed * 1.5f * Time.deltaTime);
							
						}
					}
					else fromXtoZ = false;
				}
				// 移動中(Z軸を復帰->X軸を復帰)
				else if (fromZtoX)
				{
					if (this.transform.localPosition.z != targetLA.localPosition.z)
						this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, targetLA.localPosition, moveSpeed * 1.5f * Time.deltaTime);
					else if (this.transform.localPosition.x != targetRA.localPosition.x)
					{
						if (moveInterval > 0.0f) moveInterval -= Time.deltaTime;
						else
						{
							this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, targetRA.localPosition, moveSpeed * 1.5f * Time.deltaTime);
						}
					}
					else fromZtoX = false;
				}
				// 上下移動に切り替えるまでの準備
				else
				{
					isTriggered = false;
					Device.isTriggered = false;
					toStop = false;
					handleFlag = false;
					fromXtoZ = _fromXtoZ;
					fromZtoX = _fromZtoX;
					count = 0;
					nextPosition = targetRB.localPosition;
					stopTimeCount = stopTimeReset;
					cameraSwitchTime = 0.0f;
					moveInterval = 0.5f;
				}
			}
			// 障害物を左へ移動する時
			else if (isTriggered && !right)
			{
				// 移動中(X軸を復帰->Z軸を復帰)
				if (fromXtoZ)
				{
					if (this.transform.localPosition.x != targetRA.localPosition.x)
						this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, targetRA.localPosition, moveSpeed * 1.5f * Time.deltaTime);
					else if (this.transform.localPosition.z != targetLA.localPosition.z)
					{
						if (moveInterval > 0.0f) moveInterval -= Time.deltaTime;
						else this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, targetLA.localPosition, moveSpeed * 1.5f * Time.deltaTime);
					}
					else fromXtoZ = false;
				}
				// 移動中(Z軸を復帰->X軸を復帰)
				else if (fromZtoX)
				{
					if (this.transform.localPosition.z != targetRA.localPosition.z)
						this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, targetRA.localPosition, moveSpeed * 1.5f * Time.deltaTime);
					else if (this.transform.localPosition.x != targetLA.localPosition.x)
					{
						if (moveInterval > 0.0f) moveInterval -= Time.deltaTime;
						else this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, targetLA.localPosition, moveSpeed * 1.5f * Time.deltaTime);

					}
					else fromZtoX = false;
				}
				// 上下移動に切り替えるまでの準備
				else
				{
					isTriggered = false;
					Device.isTriggered = false;
					toStop = false;
					handleFlag = false;
					fromXtoZ = _fromXtoZ;
					fromZtoX = _fromZtoX;
					count = 0;
					nextPosition = targetLB.localPosition;
					stopTimeCount = stopTimeReset;
					cameraSwitchTime = 0.0f;
					moveInterval = 0.5f;
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
					if (nextPosition == targetRB.localPosition) nextPosition = targetRA.localPosition;
					else nextPosition = targetRB.localPosition;
					stopTimeCount = stopTimeReset;
				}
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
					if (nextPosition == targetLB.localPosition) nextPosition = targetLA.localPosition;
					else nextPosition = targetLB.localPosition;
					stopTimeCount = stopTimeReset;
				}
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
