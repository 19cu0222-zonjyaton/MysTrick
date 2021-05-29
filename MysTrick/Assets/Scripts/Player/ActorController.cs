﻿//-------------------------------------------------
// ファイル名		：ActorController.cs
// 概要				：
// 作成者			：曹飛
// 更新内容			：2020/04/02 作成
//					；2020/04/11 変数増加(keys,devices)
//-------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour
{
	public GameObject model;
	public PlayerInput pi;
	public GameObject weapon;       //	武器
	public int hp;
	public bool isDead;

	//============================
	// 作成者：鍾家同
	// Trigger用変数宣言
	public bool[] keys;
	public bool[] devices;
	//============================

	public bool isInTrigger;
	public bool isUnrivaled;        //	無敵Time

	[SerializeField]
	//private Animator anim;
	private Rigidbody rigid;
	private Vector3 movingVec;
	private GoalController Gc;
	private MeshRenderer mesh;
	private int shortTimeCount;   //	点滅用タイムカウント
	private float timeCount;

	// Start is called before the first frame update
	void Awake()
	{
		pi = GetComponent<PlayerInput>();
		//anim = model.GetComponent<Animator>();
		rigid = GetComponent<Rigidbody>();
		Gc = GameObject.Find("Goal").GetComponent<GoalController>();
		mesh = GameObject.Find("PlayerModule").GetComponent<MeshRenderer>();
	}

	// Update is called once per frame
	void Update()
	{
		//anim.SetFloat("forward", pi.Dmag);
		if (pi.Dmag > 0.1f)
		{
			model.transform.forward = pi.Dvec;
		}

		movingVec = pi.Dmag * model.transform.forward;

        if (pi.isAttacking && !pi.isAimStatus)
        {
            Instantiate(weapon, transform.position + model.transform.forward * 1.5f + new Vector3(0.0f, 1.0f, 0.0f), transform.rotation);

            pi.canAttack = false;

			pi.isAttacking = false;
		}

		checkIsUnderDamage();

		checkPlayerIsDead();
    }

	void FixedUpdate()
	{
		if (!pi.isAimStatus)
		{
			rigid.position += movingVec * 5.0f * Time.fixedDeltaTime;
		}

		if (Gc.gameClear)
		{
			timeCount -= Time.fixedDeltaTime;
			if (timeCount < 0.0f)
			{
				rigid.AddForce(0, 500.0f, 0);
				timeCount = 2.0f;
			}
		}
	}

	private void checkIsUnderDamage()
	{
		if (isUnrivaled)
		{
			pi.inputEnabled = false;
			timeCount += Time.deltaTime;
			if (timeCount >= 0.1f)
			{
				mesh.enabled = true;
			}

			if (timeCount >= 0.2f)
			{
				mesh.enabled = false;
				shortTimeCount++;
				timeCount = 0.0f;
			}

			if (shortTimeCount >= 5)
			{
				shortTimeCount = 0;
				mesh.enabled = true;
				isUnrivaled = false;
				pi.inputEnabled = true;
			}
		}
	}

	private void checkPlayerIsDead()
	{
		if (hp <= 0)
		{
			isDead = true;
		}

		if (isDead)
		{
			pi.inputEnabled = false;
			transform.position = new Vector3(8.37f, -124.63f, 47.8f);
			rigid.useGravity = false;
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.transform.tag == "Device")
		{
			isInTrigger = true;
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (collider.transform.tag == "Device")
		{
			isInTrigger = false;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.transform.tag == "Enemy" && !isUnrivaled)
		{
			hp--;
			mesh.enabled = false;
			rigid.AddForce(0.0f, 500.0f, 0.0f);
			rigid.AddExplosionForce(300.0f, collision.transform.position, 5.0f);
			isUnrivaled = true;
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		if (collision.transform.tag == "Device" || collision.transform.tag == "Key")
		{
			isInTrigger = true;
		}
	}
	private void OnCollisionExit(Collision collision)
	{
		if (collision.transform.tag == "Device" || collision.transform.tag == "Key")
		{
			isInTrigger = false;
		}
	}
}
