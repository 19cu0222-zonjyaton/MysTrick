//-------------------------------------------------
// ファイル名		：FireBallController.cs
// 概要				：ファイヤーボールの制御
// 作成者			：鍾家同
// 更新内容			：2021/08/23 作成
//-------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallController : MonoBehaviour
{
	[Header("===調整用===")]
	public GameObject explore;
	public float moveSpeed = 500.0f;		// 移動スピード
	public float rotateSpeed = 5.0f;		// 回転スピード

	private Rigidbody rb;
	private Transform particalTrans;
	private Transform totemTrans;
	private float lifetime;

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
		particalTrans = this.transform.GetChild(3);
		totemTrans = GameObject.Find("totemPivot").GetComponent<Transform>();
	}
	void Start()
	{
		rb.velocity = this.transform.forward * moveSpeed * Time.fixedDeltaTime;
	}

	void Update()
	{
		// 回転させ
		this.transform.Rotate(Vector3.forward, 5.0f);
		// ParticalSystemのスケールを調整
		particalTrans.localScale = Vector3.one + new Vector3(Mathf.Abs(totemTrans.right.x), Mathf.Abs(totemTrans.right.y), Mathf.Abs(totemTrans.right.z)) * 2.0f;
		// ライフサイクル計算
		lifetime += Time.deltaTime;
		if (lifetime > 5.0f) Destroy(this.gameObject);
	}

	void OnTriggerEnter(Collider other)
	{		
		if (other.transform.tag != "StickBackPos")
		{
			Instantiate(explore, transform.position, Quaternion.identity);
			Destroy(this.gameObject);
		}

		if (other.transform.tag == "Player")
		{

		}
		else if (other.transform.tag == "Wood")
		{
			Destroy(other.transform.parent.gameObject);		
		}
	}
}
