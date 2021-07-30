//-------------------------------------------------
// ファイル名		：LiftDoorController.cs
// 概要				：リフトドアの制御
// 作成者			：鍾家同
// 更新内容			：2021/07/23 作成
//-------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftDoorController : MonoBehaviour
{
	public GameObject[] Doors = new GameObject[2];
	public GameObject[] Pieces = new GameObject[3];
	public GameObject HintUI;
	public TimeController timeController;
	public float openInterval;		// 開門間隔
	public float openTimeCount;
	public float openSpeed;

	private ActorController actorController;
	private PlayerInput player;
	private float openReset;
	private bool isTriggered;
	private bool doNext;

	void Awake()
	{
		actorController = GameObject.Find("PlayerHandle").GetComponent<ActorController>();
		player = GameObject.Find("PlayerHandle").GetComponent<PlayerInput>();
		timeController = GetComponent<TimeController>();
	}

	void Start()
	{
		isTriggered = false;
		doNext = false;
		openReset = openInterval;
	}

	void Update()
	{
		if (isTriggered)
		{
			for(int i = 0; i < 3; ++i)
			{
				// 手に持つ欠片を扉に嵌める
				if (!Pieces[i].GetComponent<MeshRenderer>().enabled && actorController.havePieces[i])
				{
					if (timeController.isFinish)
					{
						Pieces[i].GetComponent<MeshRenderer>().enabled = true;
						timeController.TimeDelay(0.0f, 1.0f);
					}
					
				}
			}
		}
	}

	void OnCollisionStay(Collision other)
	{
		if (other.transform.tag == "Player" && player.isTriggered)
		{
			isTriggered = true;
		}
	}

	void OnCollisionExit(Collision other)
	{
		if (other.transform.tag == "Player")
		{
			isTriggered = false;
		}
	}
}
