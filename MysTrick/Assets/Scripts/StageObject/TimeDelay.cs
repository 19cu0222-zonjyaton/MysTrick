using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeDelay : MonoBehaviour
{
	private bool timeFlag = true;
	// Start is called before the first frame update
	void Start()
	{
		Invoke("InvokeDelayFunc", 1.0f);
	}

	// Update is called once per frame
	void Update()
	{
		//Debug.Log("Stop!");
		if (timeFlag)
		{
			InvokeDelayFunc();
			timeFlag = false;
		}
	}

	private void InvokeDelayFunc()
	{
		Debug.Log("Time Up!");
		timeFlag = true;
	}
}
