using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
	public Vector3 dest;
	public float speed;
	public bool isTriggered;
	public bool hasDone;				//	カメラ用参数
	public float timeCount = 1.8f;		//	Triggerを出すまでの時間

	private float moveSpeed;

	// Start is called before the first frame update
	void Start()
	{
		isTriggered = false;

		moveSpeed = Vector3.Distance(this.transform.position, dest);
	}

	// Update is called once per frame
	void Update()
	{
		if (isTriggered)
		{
			timeCount -= Time.deltaTime;

			if (timeCount <= 0.0f)
			{
				this.transform.position = Vector3.MoveTowards(this.transform.position, dest, moveSpeed / 1.5f * Time.deltaTime);
			}

		}

	}
}
