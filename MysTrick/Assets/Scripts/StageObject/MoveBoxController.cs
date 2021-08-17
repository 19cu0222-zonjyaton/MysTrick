using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBoxController : MonoBehaviour
{
	public GameObject hintUI;
	public bool isTriggered;

	private PlayerInput Player;

	void Awake()
    {
		Player = GameObject.Find("PlayerHandle").GetComponent<PlayerInput>();
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnTriggerStay(Collider other)
	{
		if (!isTriggered)
		{
			hintUI.SetActive(true);
		}
		else
		{
			hintUI.SetActive(false);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.transform.tag == "Player")
		{
			hintUI.SetActive(false);
		}
	}
}
