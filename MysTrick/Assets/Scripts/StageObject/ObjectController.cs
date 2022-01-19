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
		OneWayMove,			// 片道移動
		TwoWayMove,			// 往復移動
		AutoTwoWayMove,		// 自動往復移動
		RotatePerAngle,		// 一定角度ごとに回転
		RotateToTarget,		// 指定角度に回転
		WaitToStart,		// 指定時間から移動
		PortalMove,			// ポータルゲートの移動
	}

	//===調整用値===
	[Header("移動方法")]
	[Header("===調整用===")]
	public Trajectory trajectory;		// 移動方法変数

	[System.Serializable]
	public struct MoveData
	{
		[Tooltip("For OneWayMove")]
		public Transform target;		// 指定位置
		[Tooltip("For RotateToTarget, RotatePerAng")]
		public Vector3 targetAng;		// 指定角度
		[HideInInspector]
		public Quaternion targetEuAng;	// 指定オイラー角
		[HideInInspector]
		public Vector3 startPosition;	// 開始位置
		[Tooltip("For TwoWayMove, AutoWayMove, WaitToStart, PortalMove")]
		public Transform targetA;		// 指定位置A
		[Tooltip("For TwoWayMove, AutoWayMove, WaitToStart, PortalMove")]
		public Transform targetB;		// 指定位置B
		[Tooltip("For PortalMove")]
		public ObjectController portalA;// 指定ポータルゲート位置A
		[Tooltip("For PortalMove")]
		public ObjectController portalB;// 指定ポータルゲート位置B
		[Tooltip("For All")]
		public float speed;				// 移動スピード
	}
	public MoveData moveData;
	public TriggerController Device;
	public FootPlateDeviceController FootDevice;
	public TimerController ElevTimer;
	public bool DeviceIsFtDev;			// デバイスがFootDeviceかどうかフラグ
	public bool isSinglePortal;			// 単ポータルゲートかどうかフラグ
	//==============

	private Vector3 nextTarget;			// 次の指定位置
	private float startSpeed;			// スタートスピード
	private bool timeFlag = true;		// タイマー用フラグ
	private bool liftingFin;			// エレベーター使用完了フラグ
	private int pressCount = 0;			// 押し回数
	private bool awayFromLift = false;	// プレイヤーがリフトから離れたかフラグ


	// RotatePerAng用変数
	//--------------------------------
	private bool canRotate = false;		// 回転可能フラグ
	[Tooltip("For RotateToAng: The time before start rotating. ( <0: need preparing time.)")]
	public float timeCount = -2.0f;     // 回転開始までに準備時間
	[Tooltip("For RotateToAng: The maximum time.")]
	public float timeMax = 3.0f;        // 最大経過時間
	private Vector3 nextAng;			// 次の指定角度
	private new AudioSource audio;		// Audioコンポーネント
	private bool playOnce;              // 一回だけSEを流すフラグ
	//--------------------------------

	//===監視用値===
	[Header("===監視用===")]
	public bool isTrigger;				// カメラ用フラグ
	public bool hasDone;				// カメラ用フラグ
	public float audioSpeed = 1.0f;		// SEを流すスピード
	public bool portalMoveFin;			// ポータル移動完了フラグ
	public bool doubleTrigger = false;	// ダブルトリガー
										//==============

	// 初期化
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
	}

	void Update()
	{
		// 現在位置を取得
		Vector3 curPosition = this.transform.position;
		// 各移動方法
		switch (trajectory)
		{
			case Trajectory.OneWayMove:
				// デバイスが起動すれば、オブジェクトを起動開始
				if (Device.isTriggered)
				{
					isTrigger = true;
					timeCount += Time.deltaTime;
					// 移動開始
					if (timeCount <= timeMax && timeCount >= 0.0f)
					{
						if (Mathf.Abs(this.transform.localPosition.magnitude - nextTarget.magnitude) > 0.01f)
						{
							// 指定位置に移動
							this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, moveData.target.localPosition, moveData.speed * Time.deltaTime);
							// 音を出す
							if (!playOnce)
							{
								audio.Play();
								playOnce = true;
							}
						}
					}
					// 移動停止
					else if (timeCount > timeMax) 
					{
						// 初期値に戻す
						Device.isTriggered = false;
						isTrigger = false;
						playOnce = false;
					}
				}
				break;
			case Trajectory.TwoWayMove:
				// デバイスが起動すれば、オブジェクトを起動開始
				if (Device.isTriggered)
				{
					Debug.Log(nextTarget);
					isTrigger = true;
					timeCount += Time.deltaTime;
					// 移動開始
					if (timeCount <= timeMax && timeCount >= 0.0f)
					{
						this.transform.position = Vector3.MoveTowards(this.transform.position, nextTarget, moveData.speed * Time.deltaTime);
						// 音を出す
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
						//	一回行ったらDelay時間をなしにする
						if (isTrigger)
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
				// デバイスが起動すれば、オブジェクトを起動開始
				if (Device.isTriggered)
				{
					isTrigger = true;
					// 移動開始
					if (Mathf.Abs(curPosition.magnitude - nextTarget.magnitude) > 0.1f)
					{
						moveData.speed += Time.deltaTime * 10.0f;
						this.transform.position = Vector3.MoveTowards(moveData.startPosition, nextTarget, moveData.speed);
					}
					// 移動停止
					else
					{
						// タイマー起動
						if (timeFlag)
						{
							Invoke("Timer", 2.0f);
							timeFlag = false;
						}
					}
				}
				break;
			case Trajectory.RotatePerAngle:
				// デバイスが起動すれば、オブジェクトを起動開始
				if (Device.isTriggered) canRotate = true;
				if (canRotate)
				{
					isTrigger = true;
					// 回転開始までにカウントダウン
					timeCount += Time.deltaTime;
					// 音を出す
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
						// 一回行ったらDelay時間をなしにする
						if (isTrigger)
						{
							timeCount = 0.0f;
							isTrigger = false;
						}
						// 初期値に戻す
						playOnce = false;
						canRotate = false;
						Device.isTriggered = false;
					}
				}
				break;
			case Trajectory.RotateToTarget:
				break;
			case Trajectory.WaitToStart:
				// 現在位置を取得
				Vector3 currPosition = this.transform.position;
				// カウントダウン終了後、リフトを移動開始
				if (ElevTimer.TimerFinish)
				{
					if (this.transform.position != nextTarget && !liftingFin)
					{
						moveData.speed += Time.deltaTime * 10.0f;
						this.transform.position = Vector3.MoveTowards(moveData.startPosition, nextTarget, moveData.speed);
					}
					else
					{
						liftingFin = true;
					}

				}
				// リフトが移動完了後、次の移動に準備
				if (liftingFin)
				{
					// 次の移動位置はtargetAをtargetBに変更
					if (nextTarget == moveData.targetA.position)
					{
						// プレイヤーがリフトから離れたら、カウントダウン開始
						if (awayFromLift)
						{
							ElevTimer.TimerStart = true;
							awayFromLift = false;
						}
						moveData.startPosition = moveData.targetA.position;
						nextTarget = moveData.targetB.position;
					}
					// 次の移動位置はtargetBをtargetAに変更
					else
					{
						moveData.startPosition = moveData.targetB.position;
						nextTarget = moveData.targetA.position;
					}
					// 初期値に戻る
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
				else if (isSinglePortal && portalMoveFin)
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
		// 現在位置がtargetBの場合、targetAに向かって移動していく
		else if (Mathf.Abs(this.transform.position.magnitude) - Mathf.Abs(moveData.targetB.position.magnitude) < 0.1f)
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
		// プレイヤーはエレベーターに入っているかつ、エレベーターが移動していない場合
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
			// エレベーターが移動していない場合
			if (Mathf.Abs(this.transform.position.magnitude - moveData.targetB.position.magnitude) <= 0.1f)
			{
				ElevTimer.TimerStart = false;
			}
			// エレベーターが移動完了場合
			else if (Mathf.Abs(this.transform.position.magnitude - moveData.targetA.position.magnitude) <= 0.1f)
			{
				ElevTimer.TimerStart = true;
			}
			// エレベーターが移動中の場合
			else if (Mathf.Abs(this.transform.position.magnitude - moveData.targetA.position.magnitude) >= 0.1f)
			{
				awayFromLift = true;
			}
		}
	}
	//----------------------------------
}