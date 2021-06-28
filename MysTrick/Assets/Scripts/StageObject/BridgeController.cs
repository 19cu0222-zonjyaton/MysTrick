using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeController : MonoBehaviour
{
	public TriggerController Device;
	[SerializeField]
	public bool isTriggered;
	public Vector3 targetAng;
	private Quaternion targetEuAng;
	public float speed = 5.0f;
	public bool hasDone;				//	カメラ用参数
	private float timeCount = 3.6f;		//	Triggerを出すまでの時間
	private int pressCount = 0;			// 押し回数

	void Start()
	{
		
	}

	void Update()
	{
		if (Device.isTriggered)
		{
			isTriggered = true;
			timeCount -= Time.deltaTime;
			if (timeCount <= 1.8f && timeCount > 0.0f)
			{
				targetEuAng = Quaternion.Euler(targetAng);
				// Quaternion.Slerp(Quaternion from, Quaternion to, deltaTime * speed)
				this.transform.rotation = Quaternion.Slerp(transform.rotation, targetEuAng, Time.deltaTime * speed);
				//Debug.Log(this.transform.eulerAngles.z);
				//this.transform.Rotate(0f, 0f, angle);
			}
			else if(timeCount <= 0.0f)
			{
				++pressCount;
				++Device.launchCount;

				if (pressCount % 2 == 1)
				{
					targetAng = new Vector3(0.0f, 0.0f, -90.0f);
				}
				else
				{ 
					targetAng = new Vector3(0.0f, 0.0f, 0.0f);
				}

				timeCount = 1.8f;
				Device.isTriggered = false;
			}
		}
	}
}
