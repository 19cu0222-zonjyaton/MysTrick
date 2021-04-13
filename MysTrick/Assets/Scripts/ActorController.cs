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
	//============================
	// 作成者：鍾家同
	// Trigger用変数宣言
	public bool[] keys;
	public bool[] devices;
	//============================

	[SerializeField]
	//private Animator anim;
	private Rigidbody rigid;
	private Vector3 movingVec;

	// Start is called before the first frame update
	void Awake()
	{
		pi = GetComponent<PlayerInput>();
		//anim = model.GetComponent<Animator>();
		rigid = GetComponent<Rigidbody>();
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
	}

	void FixedUpdate()
	{
		rigid.position += movingVec * 8.0f * Time.fixedDeltaTime;
	}
}
