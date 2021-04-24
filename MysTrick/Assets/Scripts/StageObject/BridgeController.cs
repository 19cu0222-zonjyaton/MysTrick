using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeController : MonoBehaviour
{
	public TriggerController Device;
	[SerializeField]
	public bool isTriggered;
	private float startAng;
	public Vector3 targetAng;
	private Quaternion targetEuAng;
	public float speed = 5.0f;
	public bool hasDone;                //	カメラ用参数
	public float timeCount = 1.8f;      //	Triggerを出すまでの時間

	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		if (Device.isTriggered) isTriggered = true;
		if (isTriggered)
		{
			timeCount -= Time.deltaTime;

			if (timeCount <= 0.0f)
			{
				this.transform.rotation = Quaternion.Slerp(transform.rotation, targetEuAng, Time.deltaTime * speed);

				targetEuAng = Quaternion.Euler(targetAng);
				//Debug.Log(this.transform.eulerAngles.z);
				//this.transform.Rotate(0f, 0f, angle);
			}
		}
	}
}
