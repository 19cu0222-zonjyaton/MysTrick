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

	public bool inputEnabled = true;
	public bool aimUI;
	public bool isTriggered;
	public bool isJumping;
	public bool lockJumpStatus;
	public bool isThrowing;
	public bool isAttacking;
	public bool canThrow = true;
	public bool isAimStatus;
	public bool overDistance;           //	壁の距離が近いかどうか
	public CameraController cc;
	private GameObject playercamera;

	public float targetDup;
	public float targetDright;
	private float velocityDup;
	private float velocityDright;

	private bool isUsingJoyStick;       //  コントローラーを検査する

	private GameObject playerModel;

	void Awake()
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

		Application.targetFrameRate = 60;   //	FPSを60に固定する

		playerModel = GameObject.Find("PlayerModule");

		playercamera = GameObject.Find("Main Camera");
	}  

	void Update()
	{
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

		if ((Input.GetKeyDown(keyTrigger) || Input.GetButtonDown("action")) && cc.cameraStatic == "Idle" && !isAimStatus)
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

        if ((Input.GetKeyDown(keyThrow) || Input.GetAxis("throw") == 1) && canThrow && !overDistance && inputEnabled)
        {
            isThrowing = true;
		}
        else if ((Input.GetKeyDown(keyAttack) || Input.GetButtonDown("attack")) && canThrow && inputEnabled)
        {
            isAttacking = true;
        }

        Dup = Mathf.SmoothDamp(Dup, targetDup, ref velocityDup, moveToTargetTime);
		Dright = Mathf.SmoothDamp(Dright, targetDright, ref velocityDright, moveToTargetTime);

		Vector2 tempDAxis = SquareToCircle(new Vector2(Dright, Dup));
		float Dright2 = tempDAxis.x;
		float Dup2 = tempDAxis.y;

		Dmag = Mathf.Sqrt((Dup2 * Dup2) + (Dright2 * Dright2));
		if ((Input.GetKey(KeyCode.LeftShift) || Input.GetButton("perspect")) && cc.cameraStatic == "Idle")		//	第一人視点を切り替え
		{
			isAimStatus = true;
			Dvec = Dright * playercamera.transform.right + Dup * playercamera.transform.forward;
		}
		else                                                                    //	第三人視点を切り替え
		{
			isAimStatus = false;
			Dvec = Dright * transform.right + Dup * transform.forward;
		}
	}

	private Vector2 SquareToCircle(Vector2 input)
	{
		Vector2 output = Vector2.zero;

		output.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2.0f);
		output.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2.0f);

		return output;
	}
}
