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
	public float moveSpeed = 500.0f;		// 移動スピード
	public float rotateSpeed = 5.0f;		// 回転スピード

	private Rigidbody rb;
	private Transform particalTrans;
	private float lifetime;

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
		particalTrans = this.transform.GetChild(3);
		
	}
	void Start()
	{
		rb.velocity = this.transform.forward * moveSpeed * Time.fixedDeltaTime;
	}

	void Update()
	{
		// 回転させ
		this.transform.Rotate(Vector3.forward, 5.0f);

		// ライフサイクル計算
		lifetime += Time.deltaTime;
		if (lifetime > 5.0f) Destroy(this.gameObject);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.transform.tag == "Player")
		{

		}
		else
		{
			//Debug.Log("FireBall touched " + other.name);
			Destroy(this.gameObject);
		}
	}
}
