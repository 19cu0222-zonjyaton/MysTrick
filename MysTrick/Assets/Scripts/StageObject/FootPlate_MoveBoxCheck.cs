using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootPlate_MoveBoxCheck : MonoBehaviour
{
	public bool isMoveBox;
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	void OnTriggerStay(Collider other)
	{
		if (other.transform.tag == "MoveBox")
		{
			isMoveBox = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.transform.tag == "MoveBox")
		{
			isMoveBox = false;
		}
	}
}
