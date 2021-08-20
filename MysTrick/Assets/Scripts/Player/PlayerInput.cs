using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	[Header("======= JoyStick Setting =======")]
	public string axisX = "axisX";
	public string axisY = "axisY";
	public string axisJright = "axis3";
	public string axisJup = "axis4";

	[Header("======= Key Setting =======")]
	public string keyUp;
	public string keyDown;
	public string keyRight;
	public string keyLeft;

	public string keyJUp;
	public string keyJDown;
	public string keyJRight;
	public string keyJLeft;

	public string keyTrigger = "b";				
	public string keyThrow = "j";
	public string keyccmera = "LeftShift";
	public string keyAttack = "k";

	[Header("======= Output Signals =======")]
	public float Dup;
	public float Dright;
	public float Dmag;
	public Vector3 Dvec;
	public float moveToTargetTime = 0.1f;

	public float Jup;
	public float Jright;

	[Header("======= Player Static =======")]
	public bool inputEnabled = true;	//	入力可能フラグ
	public bool aimUI;					//	狙う時の中心UI
	public bool isTriggered;            //	仕掛けスイッチに入いるフラグ
	public bool isPushBox;
	public bool isEntryDoor;
	public bool isJumping;				//	ジャンプ中フラグ
	public bool lockJumpStatus;			//	ジャンプ状態の時ロックするフラグ
	public bool isThrowing;				//	武器を投げっているフラグ
	public bool isAttacking;			//	アタックフラグ
	public bool canAttack = true;		//	攻撃可能フラグ
	public bool canThrow = true;		//	第三視点武器を投げる可能フラグ
	public bool isAimStatus;			//	狙う状態のフラグ
	public bool overDistance;			//	壁の距離が近いかどうか
	public CameraController cc;			//	カメラコントローラー
	//	入力Signalターゲット
	public float targetDup;
	public float targetDright;

	private float velocityDup;
	private float velocityDright;
	private GameObject playercamera;    //	カメラオブジェクト
	private ActorController ac;
	private bool isUsingJoyStick;       //	今使っているコントローラーを検査する
	private bool resetFlag;

	//	初期化
	void Awake()
	{
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 60;   //	FPSを60に固定する

		playercamera = GameObject.Find("Main Camera");

		ac = gameObject.GetComponent<ActorController>();
	}

	void Update()
	{
		string[] joystickNames = Input.GetJoystickNames();
		foreach (string joystickName in joystickNames)
		{
			if (joystickName == "")     //  Keyboard
			{
				isUsingJoyStick = false;
			}
			else                        //  JoyStick
			{
				isUsingJoyStick = true;
			}
		}

		if (isUsingJoyStick)
		{
			//  Stick Signal
			Jup = -1 * Input.GetAxis(axisJup);
			Jright = Input.GetAxis(axisJright);

			targetDup = -1 * Input.GetAxis(axisY);
			targetDright = Input.GetAxis(axisX);
		}
		else
		{
			//  KeyBoard Signal
			Jup = (Input.GetKey(keyJUp) ? 1.0f : 0) - (Input.GetKey(keyJDown) ? 1.0f : 0);
			Jright = (Input.GetKey(keyJRight) ? 1.0f : 0) - (Input.GetKey(keyJLeft) ? 1.0f : 0);

			targetDup = (Input.GetKey(keyUp) ? 1.0f : 0) - (Input.GetKey(keyDown) ? 1.0f : 0);
			targetDright = (Input.GetKey(keyRight) ? 1.0f : 0) - (Input.GetKey(keyLeft) ? 1.0f : 0);
		}

		//  Moveable
		if (inputEnabled == false)
		{
			targetDup = 0;
			targetDright = 0;
		}
		print(inputEnabled);
		Dup = Mathf.SmoothDamp(Dup, targetDup, ref velocityDup, moveToTargetTime);
		Dright = Mathf.SmoothDamp(Dright, targetDright, ref velocityDright, moveToTargetTime);

		Vector2 tempDAxis = SquareToCircle(new Vector2(Dright, Dup));
		float Dright2 = tempDAxis.x;
		float Dup2 = tempDAxis.y;

		Dmag = Mathf.Sqrt((Dup2 * Dup2) + (Dright2 * Dright2));

		if ((Input.GetKeyDown(keyTrigger) || Input.GetButtonDown("action")) && cc.cameraStatic == "Idle" && !isAimStatus && canThrow)
		{
			isPushBox = true;
			isEntryDoor = true;
		}

		if (!ac.isPushBox && !ac.isEntryDoor)
		{
			if ((Input.GetKeyDown(keyTrigger) || Input.GetButtonDown("action")) && cc.cameraStatic == "Idle" && !isAimStatus && canThrow)
			{
				isTriggered = true;
				if (!isJumping && !lockJumpStatus)
				{
					isJumping = true;
				}
			}

			if (Input.GetKeyUp(keyTrigger) || Input.GetButtonUp("action"))
			{
				isTriggered = false;

				isJumping = false;
			}

			if ((Input.GetKeyDown(keyThrow) || Input.GetAxis("throw") == 1) && canThrow && !overDistance)
			{
				isThrowing = true;
			}
			else if ((Input.GetKeyDown(keyAttack) || Input.GetButtonDown("attack")) && canThrow && canAttack)
			{
				isAttacking = true;
			}

			if ((Input.GetKey(KeyCode.LeftShift) || Input.GetButton("perspect")) && cc.cameraStatic == "Idle")      //	第一人視点を切り替え
			{
				resetFlag = false;
				isAimStatus = true;
				Dvec = Dright * playercamera.transform.right + Dup * playercamera.transform.forward;
			}
			else                                                                    //	第三人視点を切り替え
			{
				if (!resetFlag)
				{
					isAttacking = false;
					resetFlag = true;
				}
				isAimStatus = false;
				Dvec = Dright * transform.right + Dup * transform.forward;
			}
		}		
		else if(ac.isPushBox)
		{
			Dvec = Dright * transform.right + Dup * transform.forward;
		}
	}

	public void ResetSignal()	//	シングルを0にする
	{
		Dup = 0.0f;          
		Dright = 0.0f;
		Dmag = 0.0f;
		moveToTargetTime = 0.0f;
		inputEnabled = false;
	}

	private Vector2 SquareToCircle(Vector2 input)
	{
		Vector2 output = Vector2.zero;

		output.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2.0f);
		output.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2.0f);

		return output;
	}
}
