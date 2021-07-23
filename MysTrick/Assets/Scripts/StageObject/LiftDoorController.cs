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
	public GameObject[] Doors;
	public GameObject[] Pieces;
	public GameObject HintUI;
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
		actorController = GetComponent<ActorController>();
		player = GetComponent<PlayerInput>();
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
				if (!Pieces[i].GetComponent<MeshRenderer>().enabled)
				{

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
}
