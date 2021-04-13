//-------------------------------------------------
// ファイル名		：ObjectTrigger.cs
// 概要				：オブジェクトのトリガーの制御
// 作成者			：鍾家同
// 更新内容			：2021/04/10 作成
//-------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerController : MonoBehaviour
{
	public DoorController kDoor;
	public bool isTriggered;

	// Start is called before the first frame update
	void Start()
	{
		isTriggered = false;
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.tag == "Player")
		{
			if (this.transform.tag == "Device")
			{
				isTriggered = true;
				Debug.Log(this.transform.name + " has touched.");

				if (this.transform.GetChild(0) != null)
				{
					if (this.transform.GetChild(0).gameObject.activeSelf)
					{
						this.transform.GetChild(0).gameObject.SetActive(false);
					}
					else
					{
						this.transform.GetChild(0).gameObject.SetActive(true);
						// 子オブジェクトに受け渡すメッセージ
						this.transform.BroadcastMessage("DeviceOnTriggered", "sDevice");
					}
				}
			}
			if (this.transform.tag == "Key")
			{
				isTriggered = true;
				Debug.Log(this.transform.name + " has touched.");
				if (!kDoor.isTriggered) kDoor.isTriggered = true;
				Destroy(this.gameObject);
			}
		}
	}
}
