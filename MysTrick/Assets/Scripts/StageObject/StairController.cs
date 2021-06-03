//-------------------------------------------------
// ファイル名		：StairController.cs
// 概要				：オブジェクトのトリガーの制御
// 作成者			：鍾家同
// 更新内容			：2021/04/10 作成
//-------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairController : MonoBehaviour
{
	public bool isTriggered;
	public bool moveToHere;				//	カメラ用参数
	public bool hasDone;                //	カメラ用参数

	private float movePosY;
	private float moveSpeed;

	// Start is called before the first frame update
	void Awake()
	{
		this.gameObject.SetActive(false);
		isTriggered = false;
		movePosY = this.transform.position.y + 9.3f;

		moveSpeed = Vector3.Distance(this.transform.position, new Vector3(this.transform.position.x, movePosY, this.transform.position.z));
	}

	// Update is called once per frame
	void Update()
	{
		if (moveToHere)
		{
			transform.position = Vector3.MoveTowards(this.transform.position, new Vector3(this.transform.position.x, movePosY, this.transform.position.z), moveSpeed / 2.0f * Time.deltaTime);	//	2秒で指定された位置に移動する
		}
	}

	public void DeviceOnTriggered(string msg)
	{
		if (msg == "sDevice")
		{
			moveToHere = true;
		}
		
		if (msg == "sCamera")
		{
			isTriggered = true;
		}
	}
}
