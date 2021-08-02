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
	public TriggerController Device;
	public Transform targetXA;
	public Transform targetXB;
	public Transform targetYA;
	public Transform targetYB;
	public float moveSpeed;
	public float moveTimeCount;

	private int count;
	[SerializeField]
	private bool isTriggered;
	[SerializeField]
	private bool right;
	private float moveTimeReset;
	private Vector3 nextPosition;

	void Start()
	{
		isTriggered = false;
		right = true;
		count = 0;
		nextPosition = targetXB.position;
		moveTimeReset = moveTimeCount;
	}

	
	void Update()
	{
		Debug.Log(isTriggered + " " + right);
		if (Device != null && Device.isTriggered)
		{
			isTriggered = true;
			++count;
			if (count == 2 && right) right = false;
			else if (count == 2 && !right) right = true;
			Debug.Log("GD");
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
		else if (!isTriggered && right)
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
		else if (!isTriggered && !right)
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
	}

	void OnCollisionStay(Collision other)
	{
		if (other.transform.tag == "Player")
		{
			other.transform.SetParent(this.gameObject.transform);
		}
	}

	private void OnCollisionExit(Collision other)
	{
		if (other.transform.tag == "Player")
		{
			other.transform.SetParent(null);
		}
	}
}
