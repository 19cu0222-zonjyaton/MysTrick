//-------------------------------------------------
// ファイル名		：LockController.cs
// 概要				：錠の制御
// 作成者			：鍾家同
// 更新内容			：2021/08/23 作成
//-------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockController : MonoBehaviour
{
	public GameObject Lock1;
	public GameObject Lock2;

	private ActorController Player;
	private bool canStart;

	void Awake()
	{
		Player = GameObject.FindWithTag("Player").GetComponent<ActorController>();
	}

	void Update()
	{
		if (canStart)
		{

		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.transform.tag == "Player" && Player.haveKeys.BlueKey)
		{
			canStart = true;
		}
	}
}
