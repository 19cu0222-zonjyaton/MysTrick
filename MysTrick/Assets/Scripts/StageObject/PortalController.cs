//-------------------------------------------------
// ファイル名		：PortalController.cs
// 概要				：ゲートの制御
// 作成者			：鍾家同
// 更新内容			：2021/08/16 作成
//-------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
	[Header("===調整用===")]
	public GameObject portalA;
	public GameObject portalB;
	public GameObject hintUI;
	public float cameraTimeCount;
	public float transportTimeCount = 6.0f;
	public bool needKey = false;

	private GameObject player;
	private Vector3 curPosition;
	private bool isPlayingAnimation;
	private bool isTriggered;
	private float transportTimeReset;
	

	void Awake()
	{
		player = GameObject.FindGameObjectWithTag("Player");
	}

	void Start()
	{
		transportTimeReset = transportTimeCount;
	}

	void Update()
	{
		if (isTriggered)
		{
			if (transportTimeCount > 0.0f)
			{
				transportTimeCount -= Time.deltaTime;
				PlayAnimation();
			}
			else
			{
				transportTimeCount = transportTimeReset;
				isTriggered = false;
			}
		}
	}

	void PlayAnimation()
	{
		Debug.Log("PlayingAnimation...");
	}

	// 当たり判定
	//----------------------------------
	void OnTriggerStay(Collider other)
	{
		if (other.transform.tag == "Player" && !needKey || (needKey && player.GetComponent<ActorController>().havePortalKey))
		{
			hintUI.SetActive(true);
			if (player.GetComponent<PlayerInput>().isTriggered) isTriggered = true;
		}
	}
	void OnTriggerExit(Collider other)
	{
		if (other.transform.tag == "Player")
		{
			hintUI.SetActive(false);
		}
	}
	//----------------------------------
}
