using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBoxController : MonoBehaviour
{
	public GameObject hintUI;
	public GameObject player;
	public bool moveWithPlayer;

	private PlayerInput pi;
	private ActorController ac;

	void Awake()
    {
		pi = player.GetComponent<PlayerInput>();

		ac = player.GetComponent<ActorController>();
	}

    // Update is called once per frame
    void Update()
    {

	}

	private void OnTriggerStay(Collider other)
	{
		if (other.transform.tag == "Player")
		{
			hintUI.SetActive(true);
			if (pi.isPushBox)
			{
				if (moveWithPlayer)
				{
					moveWithPlayer = false;
					ac.moveSpeed = 3.0f;
				}
				else
				{
					moveWithPlayer = true;
					ac.moveSpeed = 7.0f;
				}

				pi.isPushBox = false;
			}
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
