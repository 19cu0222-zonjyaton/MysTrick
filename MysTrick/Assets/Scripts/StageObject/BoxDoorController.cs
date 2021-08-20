using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxDoorController : MonoBehaviour
{
    public GameObject hintUI;
	public GameObject player;
	public int entryIndex;

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
		if (entryIndex == 1)
		{ 
			
		}
    }

	private void OnTriggerStay(Collider other)
	{
		if (other.transform.tag == "Player" && !ac.isPushBox)
		{
			hintUI.SetActive(true);
			if (pi.isEntryDoor)
			{
				ac.isEntryDoor = true;
				entryIndex++;
				pi.isEntryDoor = false;
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
