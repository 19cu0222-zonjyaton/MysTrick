//-------------------------------------------------
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
	public GameObject rightHand;	//	プレイヤーの右手
	public int hp;
	public bool isDead;

	//============================
	// 作成者：鍾家同
	// Trigger用変数宣言
	public bool[] keys;
	public bool[] devices;
	//============================

	public float moveSpeed = 5.0f;
	public bool isInTrigger;
	public bool isUnrivaled;        //	無敵Time
	public bool shootStart;			//	武器発射flag

	[SerializeField]
	private Animator anim;
	private Rigidbody rigid;
	private Vector3 movingVec;
	private GoalController gc;
	private SkinnedMeshRenderer mesh;
	private int shortTimeCount;   //	点滅用タイムカウント
	private float timeCount;

	// Start is called before the first frame update
	void Awake()
	{
		pi = GetComponent<PlayerInput>();
		anim = model.GetComponent<Animator>();
		rigid = GetComponent<Rigidbody>();
		gc = GameObject.Find("Goal").GetComponent<GoalController>();
		mesh = GameObject.Find("Model").GetComponent<SkinnedMeshRenderer>();
	}

	// Update is called once per frame
	void Update()
	{
		anim.SetFloat("Forward", pi.Dmag);

		if (pi.Dmag > 0.1f && !pi.isAimStatus)
		{
			model.transform.forward = Vector3.Slerp(model.transform.forward, pi.Dvec, 0.3f);
			movingVec = pi.Dmag * model.transform.forward;
		}
		else if (pi.Dmag > 0.1f && pi.isAimStatus)
		{
			//model.transform.forward = pi.Dvec;
			movingVec = pi.Dmag * pi.Dvec;
		}
		else
		{
			movingVec = pi.Dmag * model.transform.forward;
		}
		
        if (pi.isAttacking && !pi.isAimStatus)
        {
			//shootStart = true;

			anim.SetTrigger("Throw");

            pi.canAttack = false;

			pi.isAttacking = false;
		}

		checkIsUnderDamage();

		checkPlayerIsDead();
    }

	void FixedUpdate()
	{
		rigid.position += movingVec * moveSpeed * Time.fixedDeltaTime;
		
		if (gc.gameClear)
		{
			timeCount -= Time.fixedDeltaTime;
			if (timeCount < 0.0f)
			{
				rigid.AddForce(0, 500.0f, 0);
				timeCount = 2.0f;
			}
		}
	}

	private void checkIsUnderDamage()   //	敵と当たると時間内で無敵状態になる
	{
		if (isUnrivaled)
		{
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

			if (shortTimeCount >= 8)
			{
				shortTimeCount = 0;
				mesh.enabled = true;
				isUnrivaled = false;
			}
			else if (shortTimeCount > 2)	//	プレイヤー操作を解禁
			{
				pi.inputEnabled = true;
			}

		}
	}

	private void checkPlayerIsDead()	//	死亡処理
	{
		if (hp <= 0 || transform.position.y < -10.0f)
		{
			isDead = true;
		}

		if (isDead)
		{
			anim.enabled = false;
			pi.inputEnabled = false;
			transform.position = new Vector3(10.0f, -125.0f, 50.0f);
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

	private void OnCollisionStay(Collision collision)
	{
		if (collision.transform.tag == "Enemy" && !isUnrivaled)     //	敵と当たる処理
		{
			hp--;
			if (hp > 0)
			{
				pi.inputEnabled = false;
				mesh.enabled = false;
				rigid.AddForce(0.0f, 500.0f, 0.0f);
				rigid.AddExplosionForce(300.0f, collision.transform.position, 5.0f);
				isUnrivaled = true;
			}
		}

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
