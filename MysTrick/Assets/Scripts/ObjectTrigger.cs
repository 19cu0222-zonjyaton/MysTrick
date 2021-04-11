//-------------------------------------------------
// ファイル名		：ObjectTrigger.cs
// 概要				：オブジェクトのトリガーの制御
// 作成者			：鍾家同
// 更新内容			：2021/04/10 作成
//-------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTrigger : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{
		
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
		}
	}
}
