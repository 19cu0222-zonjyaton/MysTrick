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
		RotatePerSecond,
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
		[Tooltip("For Rotate To Target")]
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
	public TriggerController triggerController;
	public TimerController timerController;
	private Vector3 nextTarget;
	private float startSpeed;
	private bool timeFlag = true;

	// Start is called before the first frame update
	void Start()
	{
		moveData.targetEuAng = Quaternion.Euler(moveData.targetAng);
		moveData.startPosition = this.transform.position;
		nextTarget = moveData.targetA.position;
		startSpeed = moveData.speed;
		timerController = gameObject.GetComponent<TimerController>();
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
				if (triggerController.isTriggered)
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
			case Trajectory.RotatePerSecond:
				break;
			case Trajectory.RotateToTarget:
				break;
			case Trajectory.WaitToStart:

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

	private void OnCollisionStay(Collision other)
	{
		
	}
}