//-------------------------------------------------
// ファイル名	：JumpCheck.cs
// 概要			：
// 作成者		：
// 更新内容		：
//-------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpCheck : MonoBehaviour
{
	public float jumpPowerX;
	public float jumpPowerY;
	public float jumpPowerZ;

	public bool canJump;
	public bool isJump;

	public float jumpTime;
	public int jumpCount;

	private int tempJumpCount;  //  階段数を保存するため
	private float tempJumpTime;	//	ジャンプタイムを保存するため
	private PlayerInput pi;
	private Rigidbody rigid;

	// Start is called before the first frame update
	void Awake()
	{
		pi = GameObject.Find("PlayerHandle").GetComponent<PlayerInput>();

		rigid = GameObject.Find("PlayerHandle").GetComponent<Rigidbody>();

		tempJumpCount = jumpCount;

		tempJumpTime = jumpTime;
	}

	// Update is called once per frame
	void Update()
	{
		if (jumpCount > 0)
		{
			if (canJump)        //  ジャンプをできるか
			{
				jumpTime -= Time.deltaTime;

				if (isJump)
				{
					rigid.AddForce(jumpPowerX, jumpPowerY, jumpPowerZ);

					isJump = false;
				}
			}

			if (jumpTime <= 0.0f)   //  何秒ごとにジャンプ一回をやるか
			{
				isJump = true;

				jumpTime = tempJumpTime;

				jumpCount--;    //  階段の段数

				if (jumpCount == 0)
				{
					pi.inputEnabled = true;

					canJump = false;
				}
			}
		}
	}

    void OnTriggerStay(Collider collider)
	{
		if (collider.transform.tag == "Player" && pi.isTriggered)
		{
			collider.transform.position = new Vector3(transform.position.x, collider.transform.position.y, transform.position.z);

			jumpCount = tempJumpCount;

			pi.inputEnabled = false;

			canJump = true;

			isJump = true;

			pi.isTriggered = false;
		}
	}
}
