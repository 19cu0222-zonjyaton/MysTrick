using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
	public TriggerController Device;
	public Vector3 dest;
	public Vector3 targetA;
	public Vector3 targetB;
	public float speed = 5.0f;
	public bool isTriggered;
	public bool hasDone;                //	カメラ用参数

	private float timeCount = 2.5f;      //	Triggerを出すまでの時間
	private int pressCount = 0;         // 押し回数

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (Device.isTriggered)
		{
			isTriggered = true;
			timeCount -= Time.deltaTime;
			if (timeCount <= 1.0f && timeCount > 0.0f)
			{
				this.transform.position = Vector3.MoveTowards(this.transform.position, dest, 4.0f * Time.deltaTime);
			}
			else if (timeCount <= 0.0f)
			{
				++pressCount;
				++Device.launchCount;

				if (pressCount % 2 == 1)
				{
					dest = targetA;
				}
				else
				{
					dest = targetB;
				}

				timeCount = 1.0f;
				Device.isTriggered = false;
			}
		}
	}
}
