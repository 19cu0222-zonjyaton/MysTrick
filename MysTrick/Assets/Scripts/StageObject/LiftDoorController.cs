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
	public float openTimeCount;		// 開門時間
	public float scaleSpeed;		// 伸縮スピード
	public float openSpeed;			// 開門スピード
	public float putInterval;		// 欠片を置く時間

	[Header("===監視用===")]
	[SerializeField]
	private bool doNext;			// 次の行動を行うかどうかフラグ
	private ActorController actorController;
	private PlayerInput player;
	private float openReset;		// 開門の復帰時間
	private float putReset;			// 欠片を置く復帰時間
	private bool isTriggered;		// オブジェクトが起動したかどうかフラグ
	private bool isTriggering;		// オブジェクトが起動しているフラグ
	private int i = 0, j = 0;		// 欠片用変数

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
			// 手に持つ欠片を嵌めることが完了し、次の行動に行う
			if (i >= 3)
			{
				doNext = true;
				isTriggering = false;
			}
		}
		// 扉を左へ収縮して開く
		if (doNext)
		{
			// 扉を１つずつ収縮する
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
			// 二つの扉の間に時間間隔を置く
			else if (j < 3)
			{
				Doors[j].GetComponent<Transform>().localScale = new Vector3(1.0f, 1.0f, 0.0f);
				Doors[j].GetComponent<BoxCollider>().enabled = false;
				openInterval = openReset;
				if (j < 3) ++j;
			}
			// 収縮完了
			else if (j >= 3)
			{
				this.GetComponent<BoxCollider>().enabled = false;
				HintUI.SetActive(false);
			}
		}
	}

	// 当たり判定
	//----------------------------------
	void OnTriggerStay(Collider other)
	{
		// プレイヤーが欠片を全て持っている場合、オブジェクトを起動する
		if (other.transform.tag == "Player")
		{
			HintUI.SetActive(true);
			if (player.isTriggered && actorController.havePieces[0] &&
				actorController.havePieces[1] && actorController.havePieces[2]) isTriggered = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
		// プレイヤーが離れた場合、、オブジェクトを起動しない
		if (other.transform.tag == "Player")
		{
			HintUI.SetActive(false);
			if (player.isTriggered) player.isTriggered = false;
			isTriggered = false;
		}
	}
}
