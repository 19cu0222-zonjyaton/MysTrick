//-------------------------------------------------
// ファイル名		：ObjectController.cs
// 概要				：オブジェクトの制御
// 作成者			：鍾家同
// 更新内容			：2021/04/13 作成
//					：2021/05/23 更新　エレベーター用移動方法の変更
//-------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
	public enum Trajectory
	{
		OneWayMove,
		TwoWayMove,
		RotatePerAngle,
		RotateToTarget,
		WaitToStart
	}
	[Header("移動方法")]
	public Trajectory trajectory;

	[System.Serializable]
	public struct MoveData
	{
		[Tooltip("For One Way Move")]
		public Vector3 target;
		[Tooltip("For Rotate To Target and Rotate Per Ang")]
		public Vector3 targetAng;
		[HideInInspector]
		public Quaternion targetEuAng;
		[HideInInspector]
		public Vector3 startPosition;
		[Tooltip("For Two Way Move and Wait to Start")]
		public Transform targetA;
		[Tooltip("For Two Way Move and Wait to Start")]
		public Transform targetB;
		[Tooltip("For All")]
		public float speed;
	}
	public MoveData moveData;
	public TriggerController Device;
	public TimerController ElevTimer;
	private Vector3 nextTarget;
	private float startSpeed;
	private bool timeFlag = true;
	// エレベーター使用完了フラグ
	private bool liftingFin;

	//--------------------------------
	// RotatePerAng用変数

	// 回転可能フラグ
	private bool canRotate = false;
	// 回転開始までに準備時間
	public float timeCount = 2.0f;
	// 回転初期時間
	private float timeReset;
	private Vector3 nextEuAng;
	//--------------------------------

	void Awake()
	{
		moveData.targetEuAng = Quaternion.Euler(moveData.targetAng);
		nextEuAng = transform.rotation.eulerAngles + moveData.targetAng;
	}

	// Start is called before the first frame update
	void Start()
	{
		moveData.startPosition = this.transform.position;
		nextTarget = moveData.targetA.position;
		startSpeed = moveData.speed;
		ElevTimer = gameObject.GetComponent<TimerController>();
		liftingFin = false;
		timeReset = timeCount;
	}

	// Update is called once per frame
	void Update()
	{
		//Debug.Log(moveData.targetA.position);
		//Debug.Log(moveData.targetA.localPosition);
		//Debug.Log(this.transform.position);
		//Debug.Log(this.transform.localPosition);
		switch (trajectory)
		{
			case Trajectory.OneWayMove:
				break;
			case Trajectory.TwoWayMove:
				Vector3 curPosition = this.transform.position;
				if (Device.isTriggered)
				{
					if (Mathf.Abs(curPosition.magnitude - nextTarget.magnitude) > 0.1f)
					{
						moveData.speed += Time.deltaTime * 10.0f;
						//this.transform.position = Vector3.Slerp(moveData.startPosition, nextTarget, moveData.speed);
						this.transform.position = Vector3.MoveTowards(moveData.startPosition, nextTarget, moveData.speed);
					}
					else
					{
						if (timeFlag)
						{
							Invoke("Timer", 2.0f);
							timeFlag = false;
						}
					}
				}
				break;
			case Trajectory.RotatePerAngle:
				if (Device.isTriggered) canRotate = true;
				Debug.Log(transform.rotation);
				Debug.Log(transform.rotation.eulerAngles);
				Debug.Log(moveData.targetAng);
				Debug.Log(moveData.targetEuAng);
				Debug.Log(nextEuAng);

				if (canRotate)
				{
					timeCount -= Time.deltaTime;
					// 回転開始
					if (timeCount >= -3.0f && timeCount <= 0.0f)
					{
						
						this.transform.rotation = Quaternion.Slerp(this.transform.rotation, moveData.targetEuAng, Time.deltaTime * moveData.speed);
					}
					// 回転停止
					else if (timeCount < -3.0f)
					{
						timeCount = timeReset;
						canRotate = false;

					}
				}
				break;
			case Trajectory.RotateToTarget:
				break;
			case Trajectory.WaitToStart:
				Vector3 currPosition = this.transform.position;
				if (ElevTimer.TimerFinish)
				{
					if (this.transform.position != nextTarget && !liftingFin)
					{
						moveData.speed += Time.deltaTime * 10.0f;
						//this.transform.position = Vector3.Slerp(moveData.startPosition, nextTarget, moveData.speed);
						this.transform.position = Vector3.MoveTowards(moveData.startPosition, nextTarget, moveData.speed);
					}
					else liftingFin = true;

				}
				if (liftingFin)
				{
					if (nextTarget == moveData.targetA.position)
					{
						moveData.startPosition = moveData.targetA.position;
						nextTarget = moveData.targetB.position;
					}
					else
					{
						moveData.startPosition = moveData.targetB.position;
						nextTarget = moveData.targetA.position;
					}
					moveData.speed = 0.0f;
					liftingFin = false;
					ElevTimer.TimerFinish = false;
				}
				//Debug.Log(this.transform.position + "\n" + moveData.targetB.position);
				break;
			default:
				break;
		}
	}

	private void Timer()
	{
		// 現在位置がtargetAの場合、targetBに向かって移動していく
		if (Mathf.Abs(Mathf.Abs(this.transform.position.magnitude) - Mathf.Abs(moveData.targetA.position.magnitude)) < 0.1f)
		{
			nextTarget = moveData.targetB.position;
			moveData.startPosition = moveData.targetA.position;
		}
		else if(Mathf.Abs(this.transform.position.magnitude) - Mathf.Abs(moveData.targetB.position.magnitude) < 0.1f)
		{
			nextTarget = moveData.targetA.position;
			moveData.startPosition = moveData.targetB.position;
		}
		moveData.speed = startSpeed;
		timeFlag = true;
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.tag == "Player" && trajectory == Trajectory.WaitToStart
			&& this.transform.position == moveData.targetB.position)
		{
			ElevTimer.TimerStart = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Player" && trajectory == Trajectory.WaitToStart)
		{
			// エレベーターから外す場合
			if (Mathf.Abs(this.transform.position.magnitude - moveData.targetB.position.magnitude) <= 0.1f)
			{
				ElevTimer.TimerStart = false;
			}
			else if (Mathf.Abs(this.transform.position.magnitude - moveData.targetA.position.magnitude) <= 0.1f)
			{
				ElevTimer.TimerStart = true;
			}
		}
	}
}