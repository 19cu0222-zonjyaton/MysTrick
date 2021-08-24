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
	[Header("===調整用===")]
	public GameObject[] Doors = new GameObject[3];
	public GameObject[] Pieces = new GameObject[3];
	public GameObject HintUI;
	public TimeController timeController;
	public float openInterval;		// 開門間隔
	public float openTimeCount;
	public float scaleSpeed;
	public float openSpeed;
	public float putInterval;

	[Header("===監視用===")]
	[SerializeField]
	private bool doNext;
	private ActorController actorController;
	private PlayerInput player;
	private float openReset;
	private float putReset;
	private bool isTriggered;
	private bool isTriggering;
	private int i = 0, j = 0;

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
		openReset = 0.1f;
		putReset = 0.5f;
	}

	void Update()
	{
		if (isTriggered) isTriggering = true;
		if (isTriggering){
			// 手に持つ欠片を扉に嵌める
			if (i < 3 && !Pieces[i].GetComponent<MeshRenderer>().enabled && actorController.havePieces[i])
			{
				if (putInterval > 0.0f) putInterval -= Time.deltaTime;
				else
				{
					Pieces[i].GetComponent<MeshRenderer>().enabled = true;
					putInterval = putReset;
					if (i < 3) ++i;
				}
			}
			if (i >= 3)
			{
				doNext = true;
				isTriggering = false;
			}
			Debug.Log("i=" + i);
		}
		if (doNext)
		{
			Debug.Log(doNext);
			if (j < 3 && Doors[j].GetComponent<Transform>().localScale.z > 0.01f)
			{
				if (openInterval > 0.0f) openInterval -= Time.deltaTime;
				else
				{
					Doors[j].GetComponent<Transform>().rotation = Quaternion.Euler(0.0f, 0.0f, 180f);
					if (Pieces[j].gameObject != null) Destroy(Pieces[j].gameObject);
					Doors[j].GetComponent<Transform>().localScale -= new Vector3(0.0f, 0.0f, scaleSpeed * Time.deltaTime);
					Doors[j].GetComponent<Transform>().localPosition = Vector3.MoveTowards(Doors[j].GetComponent<Transform>().localPosition, new Vector3(0.0f, 0.0f, 4.0f), openSpeed * Time.deltaTime);
				}
			}
			else if (j < 3)
			{
				Doors[j].GetComponent<Transform>().localScale = new Vector3(1.0f, 1.0f, 0.0f);
				Doors[j].GetComponent<BoxCollider>().enabled = false;
				openInterval = openReset;
				if (j < 3) ++j;
			}
			else if (j >= 3)
			{
				this.GetComponent<BoxCollider>().enabled = false;
				HintUI.SetActive(false);
			}
		}
	}

	// 当たり判定
	//----------------------------------
	//void OnCollisionStay(Collision other)
	//{
	//	if (other.transform.tag == "Player")
	//	{
	//		HintUI.SetActive(true);
	//		if (player.isTriggered) isTriggered = true;
	//	}
	//}

	//void OnCollisionExit(Collision other)
	//{
	//	if (other.transform.tag == "Player")
	//	{
	//		HintUI.SetActive(false);
	//		isTriggered = false;
	//	}
	//}
	////----------------------------------

	// 当たり判定
	//----------------------------------
	void OnTriggerStay(Collider other)
	{
		if (other.transform.tag == "Player")
		{
			HintUI.SetActive(true);
			if (player.isTriggered) isTriggered = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.transform.tag == "Player")
		{
			HintUI.SetActive(false);
			if (player.isTriggered) player.isTriggered = false;
			isTriggered = false;
		}
	}
}
