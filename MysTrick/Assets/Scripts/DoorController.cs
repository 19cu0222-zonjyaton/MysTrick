using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
	public Vector3 dest;
	public float speed;
	public bool isTriggered;
	// Start is called before the first frame update
	void Start()
	{
		isTriggered = false;
	}

	// Update is called once per frame
	void Update()
	{
		if (isTriggered)
		{
			this.transform.position = Vector3.MoveTowards(this.transform.position, dest, speed * Time.deltaTime);
		}

	}
}
