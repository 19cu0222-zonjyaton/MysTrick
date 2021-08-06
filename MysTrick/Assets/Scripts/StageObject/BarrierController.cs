//-------------------------------------------------
// ファイル名		：BarrierController.cs
// 概要				：障害物の制御
// 作成者			：鍾家同
// 更新内容			：2021/08/02 作成
//-------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierController : MonoBehaviour
{
	[Header("===調整用===")]
	public TriggerController Device;
	public Transform targetXA;		// 右A点
	public Transform targetXB;		// 右B点
	public Transform targetYA;		// 左A点
	public Transform targetYB;		// 左B点
	public float moveSpeed;			// 移動スピード
	public float moveTimeCount;		// 移動経過時間
	public float stopTimeCount;		// 停止経過時間

	[Header("===監視用===")]
	[SerializeField]
	private bool isTriggered;
	[SerializeField]
	private bool right;
	private int count;
	private float moveTimeReset;
	private Vector3 nextPosition;
	private bool touchPlayer;

	// 初期化
	void Start()
	{
		isTriggered = false;
		right = true;
		count = 0;
		nextPosition = targetXB.position;
		moveTimeReset = moveTimeCount;
		touchPlayer = false;
	}

	
	void Update()
	{
		if (Device != null && Device.isTriggered)
		{
			isTriggered = true;
			++count;
			if (count == 2 && right) right = false;
			else if (count == 2 && !right) right = true;
		}

		if (isTriggered && right)
		{
			if (this.transform.localPosition != targetXA.localPosition) this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, targetXA.localPosition, moveSpeed * 1.5f * Time.deltaTime);
			else
			{
				isTriggered = false;
				Device.isTriggered = false;
				count = 0;
				nextPosition = targetXB.localPosition;
				moveTimeCount = moveTimeReset;
			}
		}
		else if (isTriggered && !right)
		{
			if (this.transform.localPosition != targetYA.localPosition) this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, targetYA.localPosition, moveSpeed * 1.5f * Time.deltaTime);
			else
			{
				isTriggered = false;
				Device.isTriggered = false;
				count = 0;
				nextPosition = targetYB.localPosition;
				moveTimeCount = moveTimeReset;
			}
		}
		else if (!isTriggered && right && !touchPlayer)
		{
			if (moveTimeCount > 0.0f)
			{
				moveTimeCount -= Time.deltaTime;
				this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, nextPosition, moveSpeed * 1.5f * Time.deltaTime);
			}
			else
			{
				if (nextPosition == targetXB.localPosition) nextPosition = targetXA.localPosition;
				else nextPosition = targetXB.localPosition;
				moveTimeCount = moveTimeReset;
			}

		}
		else if (!isTriggered && !right && !touchPlayer)
		{
			if (moveTimeCount > 0.0f)
			{
				moveTimeCount -= Time.deltaTime;
				this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, nextPosition, moveSpeed * 1.5f * Time.deltaTime);
			}
			else
			{
				if (nextPosition == targetYB.localPosition) nextPosition = targetYA.localPosition;
				else nextPosition = targetYB.localPosition;
				moveTimeCount = moveTimeReset;
			}
		}
		else if (touchPlayer)
		{

		}
	}

	void OnCollisionStay(Collision other)
	{
		if (other.transform.tag == "Player")
		{
			other.transform.SetParent(this.gameObject.transform);
		}
	}

	void OnCollisionExit(Collision other)
	{
		if (other.transform.tag == "Player")
		{
			other.transform.SetParent(null);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.transform.tag == "Player")
		{
			// 方法一
			touchPlayer = true;
			other.GetComponent<Rigidbody>().AddForce(-this.transform.right * 700.0f);
			other.GetComponent<Rigidbody>().AddForce(this.transform.up * 500.0f);
		}
	}
}
