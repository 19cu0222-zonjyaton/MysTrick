﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeController : MonoBehaviour
{
	public TriggerController Device;
	[SerializeField]
	private bool isTrigger = false;
	private float startAng;
	public Vector3 targetAng;
	private Quaternion targetEuAng;
	public float speed = 5.0f;

	void Start()
	{
		targetEuAng = Quaternion.Euler(targetAng);
	}

	// Update is called once per frame
	void Update()
	{
		if (Device.isTriggered) isTrigger = true;
		if (isTrigger)
		{
			this.transform.rotation = Quaternion.Slerp(transform.rotation, targetEuAng, Time.deltaTime * speed);
			//Debug.Log(this.transform.eulerAngles.z);
			//this.transform.Rotate(0f, 0f, angle);
		}
	}
}
