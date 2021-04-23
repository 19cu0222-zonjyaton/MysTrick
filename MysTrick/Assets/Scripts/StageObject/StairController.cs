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
	public bool moveToHere;     //	カメラ用参数
	public bool hasDone;        //	カメラ用参数
	// Start is called before the first frame update
	void Start()
	{
		this.gameObject.SetActive(false);
		isTriggered = false;
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void DeviceOnTriggered(string msg)
	{
		if (msg == "sDevice")
		{
			isTriggered = true;

			moveToHere = true;
		}
	}
}
