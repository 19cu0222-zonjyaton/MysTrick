//-------------------------------------------------
// ファイル名		：ObjectController.cs
// 概要				：オブジェクトの制御
// 作成者			：鍾家同
// 更新内容			：2021/04/13 作成
//					：2021/05/23 更新　エレベーター用移動方法の変更
//					：2021/06/11 更新　回転制御の追加（RotatePerAng）
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
		AutoTwoWayMove,
		RotatePerAngle,
		RotateToTarget,
		WaitToStart,
		PortalMove,
	}

	//===調整用値===
	[Header("移動方法")]
	[Header("===調整用===")]
	public Trajectory trajectory;

	[System.Serializable]
	public struct MoveData
	{
		[Tooltip("For OneWayMove")]
		public Transform target;
		[Tooltip("For RotateToTarget, RotatePerAng")]
		public Vector3 targetAng;
		[HideInInspector]
		public Quaternion targetEuAng;
		[HideInInspector]
		public Vector3 startPosition;
		[Tooltip("For TwoWayMove, AutoWayMove, WaitToStart, PortalMove")]
		public Transform targetA;
		[Tooltip("For TwoWayMove, AutoWayMove, WaitToStart, PortalMove")]
		public Transform targetB;
		[Tooltip("For PortalMove")]
		public ObjectController portalA;
		[Tooltip("For PortalMove")]
		public ObjectController portalB;
		[Tooltip("For All")]
		public float speed;
	}
	public MoveData moveData;
	public TriggerController Device;
	public FootPlateDeviceController FootDevice;
	public TimerController ElevTimer;
	public bool DeviceIsFtDev;
	public bool isSinglePortal;
	//==============

	private Vector3 nextTarget;
	private float startSpeed;
	private bool timeFlag = true;
	private bool liftingFin;			// エレベーター使用完了フラグ
	private int pressCount = 0;			// 押し回数


	// RotatePerAng用変数
	//--------------------------------
	// 回転可能フラグ
	private bool canRotate = false;
	// 回転開始までに準備時間
	[Tooltip("For RotateToAng: The time before start rotating. ( <0: need preparing time.)")]
	public float timeCount = -2.0f;
	[Tooltip("For RotateToAng: The maximum time.")]
	public float timeMax = 3.0f;
	// 回転初期時間
	//private float timeReset;
	private Vector3 nextAng;
	//	Audioコンポーネント
	private new AudioSource audio;
	//	一回だけSEを流すフラグ
	private bool playOnce;
	//--------------------------------

	//===監視用値===
	[Header("===監視用===")]
	public bool isTrigger;				//	カメラ用フラグ
	public bool hasDone;				//	カメラ用フラグ
	public float audioSpeed = 1.0f;		//	SEを流すスピード
	public bool portalMoveFin;			// ポータル移動完了フラグ
	public bool doubleTrigger = false;	// 
										//==============

	void Awake()
	{
		moveData.targetEuAng = Quaternion.Euler(moveData.targetAng);
		nextAng = transform.rotation.eulerAngles + moveData.targetAng;
		moveData.startPosition = this.transform.position;
		if (moveData.targetB != null) nextTarget = moveData.targetB.position;
		startSpeed = moveData.speed;
		liftingFin = false;
		portalMoveFin = false;
		audio = gameObject.GetComponent<AudioSource>();
		ElevTimer = gameObject.GetComponent<TimerController>();
		//timeReset = timeCount;
	}

	// Update is called once per frame
	void Update()
	{
		//Debug.Log(moveData.targetA.position);
		//Debug.Log(moveData.targetA.localPosition);
		//Debug.Log(this.transform.position);
		//Debug.Log(this.transform.localPosition);
		Vector3 curPosition = this.transform.position;
		switch (trajectory)
		{
			case Trajectory.OneWayMove:
				if (Device.isTriggered)
				{
					isTrigger = true;
					timeCount += Time.deltaTime;
					// 移動開始
					if (timeCount <= timeMax && timeCount >= 0.0f)
					{
						if (Mathf.Abs(this.transform.localPosition.magnitude - nextTarget.magnitude) > 0.01f)
						{
							this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, moveData.target.localPosition, moveData.speed * Time.deltaTime);
							if (!playOnce)
							{
								audio.Play();
								playOnce = true;
							}
						}
					}
					else if (timeCount > timeMax) 
					{
						Device.isTriggered = false;
						isTrigger = false;
						playOnce = false;
					}
				}
				break;
			case Trajectory.TwoWayMove:
				if (Device.isTriggered)
				{
					Debug.Log(nextTarget);
					isTrigger = true;
					timeCount += Time.deltaTime;
					// 移動開始
					if (timeCount <= timeMax && timeCount >= 0.0f)
					{
						this.transform.position = Vector3.MoveTowards(this.transform.position, nextTarget, moveData.speed * Time.deltaTime);
						if (!playOnce)
						{
							audio.Play();
							playOnce = true;
						}
					}
					// 移動停止
					else if (timeCount > timeMax)
					{
						++pressCount;
						++Device.launchCount;
						if (isTrigger)              //	一回が行ったらDelay時間をなしにする
						{
							timeCount = 0.0f;
							isTrigger = false;
						}
						playOnce = false;
						Device.isTriggered = false;
						// 押し回数により次のターゲットを決定する
						if (pressCount % 2 == 1) nextTarget = moveData.targetA.position;
						else nextTarget = moveData.targetB.position;
					}
				}
				break;
			case Trajectory.AutoTwoWayMove:
				if (Device.isTriggered)
				{
					isTrigger = true;
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
				//Debug.Log(transform.rotation);
				//Debug.Log(transform.rotation.eulerAngles);
				//Debug.Log(nextAng);
				if (Device.isTriggered) canRotate = true;
				if (canRotate)
				{
					isTrigger = true;
					// 回転開始までにカウントダウン
					timeCount += Time.deltaTime;

					if (!playOnce)
					{
						audio.Play();
						playOnce = true;
					}

					// 回転開始
					if (timeCount <= timeMax && timeCount >= 0.0f)
					{
						// Quaternion.Slerp(Quaternion From, Quaternion To, Speed * deltaTime)
						this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.Euler(nextAng), moveData.speed * Time.deltaTime);
					}
					// 回転停止
					else if (timeCount > timeMax)
					{
						// 角度の補正
						if (this.transform.rotation != Quaternion.Euler(nextAng)) this.transform.rotation = Quaternion.Euler(nextAng);
						nextAng += moveData.targetAng;
						// 初期値に戻す
						if (isTrigger)    //	一回が行ったらDelay時間をなしにする
						{
							timeCount = 0.0f;
							isTrigger = false;
						}
						//timeCount = timeReset;
						playOnce = false;
						canRotate = false;
						Device.isTriggered = false;
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
			case Trajectory.PortalMove:
				// 移動開始
				if (FootDevice.isTriggered && !portalMoveFin && moveData.targetB != null)
				{
					if (this.transform.position != nextTarget)
					{
						// portalAの移動開始
						if (this.name == moveData.portalA.name)
						{
							if (!FootDevice.doubleTriggerA) this.transform.position = Vector3.MoveTowards(this.transform.position, nextTarget, moveData.speed * Time.deltaTime);
							else
							{
								nextTarget = (nextTarget == moveData.targetA.position) ? moveData.targetB.position : moveData.targetA.position;
								FootDevice.doubleTriggerA = false;
							}
						}
						// portalBの移動開始
						else if (this.name == moveData.portalB.name)
						{
							if (!FootDevice.doubleTriggerB) this.transform.position = Vector3.MoveTowards(this.transform.position, nextTarget, moveData.speed * Time.deltaTime);
							else
							{
								nextTarget = (nextTarget == moveData.targetA.position) ? moveData.targetB.position : moveData.targetA.position;
								FootDevice.doubleTriggerB = false;
							}
						}
					}
					else
					{
						// 移動完了フラグを立て
						portalMoveFin = true;
						// 移動方向変換
						nextTarget = (nextTarget == moveData.targetA.position) ? moveData.targetB.position : moveData.targetA.position;
						isTrigger = false;
					}
				}
				// 初期化(移動ポータルが2台である)
				else if (!isSinglePortal && moveData.portalA.portalMoveFin && moveData.portalB.portalMoveFin)
				{
					FootDevice.isTriggered = false;
					moveData.portalA.portalMoveFin = false;
					moveData.portalB.portalMoveFin = false;
				}
				// 初期化(移動ポータルが1台である)
				else if (portalMoveFin)
				{
					FootDevice.isTriggered = false;
					portalMoveFin = false;
				}
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

	// 当たり判定
	//----------------------------------
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
	//----------------------------------
}