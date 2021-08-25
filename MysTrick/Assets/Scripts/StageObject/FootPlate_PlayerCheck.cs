﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootPlate_PlayerCheck : MonoBehaviour
{
	public bool isPlayer;
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
		if (other.transform.tag == "Player")
		{
			isPlayer = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.transform.tag == "Player")
		{
			isPlayer = false;
		}
	}
}
