using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxDoorController : MonoBehaviour
{
    public GameObject hintUI;
	public GameObject player;
	public GameObject model;
	public GameObject[] movePos;
	public GameObject door;
	public GameObject linkDoor;
	public GameObject lockKey;
	public Animator anim;
	public int entryIndex;
	public float moveSpeed;
	public float rotSpeed;
	public float doorRotSpeed;
	public float levelYRot;
	public float[] doorRotTarget;
	public bool needKey;

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
		entryDoor();
	}

	private void entryDoor()
	{
		if (entryIndex == 1)    //	Move To WaitPos
		{
			if (lockKey != null)
			{
				lockKey.GetComponent<Rigidbody>().useGravity = true;
				lockKey.GetComponent<BoxCollider>().isTrigger = false;
			}
			player.transform.position = Vector3.MoveTowards(player.transform.position, new Vector3(movePos[0].transform.position.x, player.transform.position.y, movePos[0].transform.position.z), moveSpeed * Time.deltaTime);
			model.transform.rotation = Quaternion.Lerp(model.transform.rotation, movePos[0].transform.rotation, rotSpeed * Time.deltaTime);
			anim.SetFloat("Forward", 1.0f);
			if (player.transform.position.x == movePos[0].transform.position.x && player.transform.position.z == movePos[0].transform.position.z)
			{
				anim.SetFloat("Forward", 0.0f);
				entryIndex++;
			}
		}
		else if (entryIndex == 2)   //	EntryDoor Open
		{
			Destroy(lockKey.gameObject);
			door.transform.rotation = Quaternion.Lerp(door.transform.rotation, Quaternion.Euler(new Vector3(0, doorRotTarget[0], 0)), doorRotSpeed * Time.deltaTime);
			if (door.transform.localEulerAngles.y < 252.0f)
			{
				moveSpeed = 5.0f;
				door.transform.localEulerAngles = new Vector3(0, -110, 0);
				entryIndex++;
			}
		}
		else if (entryIndex == 3)   //	Move To Door
		{
			player.transform.position = Vector3.MoveTowards(player.transform.position, new Vector3(movePos[1].transform.position.x, player.transform.position.y, movePos[1].transform.position.z), moveSpeed * Time.deltaTime);
			model.transform.rotation = Quaternion.Lerp(model.transform.rotation, movePos[1].transform.rotation, rotSpeed * Time.deltaTime);
			anim.SetFloat("Forward", 1.0f);
			if (player.transform.position.x == movePos[1].transform.position.x && player.transform.position.z == movePos[1].transform.position.z)
			{
				anim.SetFloat("Forward", 0.0f);
				doorRotSpeed = 8.0f;
				entryIndex++;
			}
		}
		else if (entryIndex == 4)   //	EntryDoor Close 
		{
			door.transform.rotation = Quaternion.Lerp(door.transform.rotation, Quaternion.Euler(new Vector3(0, doorRotTarget[1], 0)), doorRotSpeed * Time.deltaTime);
			if (door.transform.localEulerAngles.y > 358.0f)
			{
				doorRotSpeed = 4.0f;
				door.transform.localEulerAngles = new Vector3(0, 0, 0);
				entryIndex++;
			}
		}
		else if (entryIndex == 5)   //	LevelDoor Open
		{
			player.transform.position = movePos[2].transform.position;
			model.transform.rotation = movePos[3].transform.rotation;
			linkDoor.transform.rotation = Quaternion.Lerp(linkDoor.transform.rotation, Quaternion.Euler(new Vector3(0, doorRotTarget[2], 0)), doorRotSpeed * Time.deltaTime);
			if (linkDoor.transform.localEulerAngles.y < 252.0f)
			{
				moveSpeed = 5.0f;
				linkDoor.transform.localEulerAngles = new Vector3(0, -110, 0);
				entryIndex++;
			}
		}
		else if (entryIndex == 6)   //	Move To LevelPos
		{
			player.transform.position = Vector3.MoveTowards(player.transform.position, new Vector3(movePos[3].transform.position.x, player.transform.position.y, movePos[3].transform.position.z), moveSpeed * Time.deltaTime);
			model.transform.rotation = movePos[3].transform.rotation;
			anim.SetFloat("Forward", 1.0f);
			player.transform.localEulerAngles = new Vector3(0, levelYRot, 0);
			if (player.transform.position.x == movePos[3].transform.position.x && player.transform.position.z == movePos[3].transform.position.z)
			{
				anim.SetFloat("Forward", 0.0f);
				doorRotSpeed = 8.0f;
				entryIndex++;
			}
		}
		else if (entryIndex == 7)   //	LevelDoor Close
		{
			linkDoor.transform.rotation = Quaternion.Lerp(linkDoor.transform.rotation, Quaternion.Euler(new Vector3(0, doorRotTarget[3], 0)), doorRotSpeed * Time.deltaTime);
			if (linkDoor.transform.localEulerAngles.y > 358.0f)
			{
				moveSpeed = 4.0f;
				doorRotSpeed = 4.0f;
				linkDoor.transform.localEulerAngles = new Vector3(0, 0, 0);
				entryIndex = 0;
				ac.isEntryDoor = false;
			}
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.transform.tag == "Player")
		{
			hintUI.SetActive(true);
			if (pi.isTriggered)
			{
				if (!ac.isEntryDoor)
				{
					if (needKey && ac.haveKeys.BlueKey)
					{
						ac.isEntryDoor = true;
						entryIndex++;
					}
					else if (!needKey)
					{
						ac.isEntryDoor = true;
						entryIndex++;
					}
				}
				pi.isTriggered = false;
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
