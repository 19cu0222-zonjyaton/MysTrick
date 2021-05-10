﻿using System.Collections;
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

	public string keyAttack = "j";
	public GameObject sickle;

	[Header("======= Output Signals =======")]
	public float Dup;
	public float Dright;
	public float Dmag;
	public Vector3 Dvec;
	public float moveSpeed = 0.1f;

	public float Jup;
	public float Jright;

	public bool inputEnabled = true;
	public bool isTriggered = false;
	public bool isJumping = false;
	public bool isAttacking = false;
	public bool lockJumpStatus = false;
	public CameraController ca;

	public float targetDup;
	public float targetDright;
	private float velocityDup;
	private float velocityDright;

	private GameObject model;

	private bool isUsingJoyStick;       //  コントローラーを検査する

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

		model = GameObject.Find("PlayerModule");
	}  

	void Update()
	{
		if (isUsingJoyStick)
		{
			//  Stick Signal
			Jup = Input.GetAxis(axisJup);
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

		Dup = Mathf.SmoothDamp(Dup, targetDup, ref velocityDup, moveSpeed);
		Dright = Mathf.SmoothDamp(Dright, targetDright, ref velocityDright, moveSpeed);

		Vector2 tempDAxis = SquareToCircle(new Vector2(Dright, Dup));
		float Dright2 = tempDAxis.x;
		float Dup2 = tempDAxis.y;

		Dmag = Mathf.Sqrt((Dup2 * Dup2) + (Dright2 * Dright2)); ;
		Dvec = Dright * transform.right + Dup * transform.forward;

		if ((Input.GetKeyDown(keyTrigger) || Input.GetButtonDown("action")) && ca.cameraStatic == "Idle")
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

		if (Input.GetKeyDown(keyAttack))
		{
			Instantiate(sickle, transform.position + model.transform.forward * 1.5f + new Vector3(0.0f, 1.0f, 0.0f), transform.rotation);
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