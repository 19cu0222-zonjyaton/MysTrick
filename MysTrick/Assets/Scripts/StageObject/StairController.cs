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
	[Header("===調整用===")]
	public TriggerController Device;

	[Header("===監視用===")]
	public bool isTriggered;			
	public int moveVec;					//	移動方向
	public bool hasDone;                //	カメラ用参数
	public float moveTargetA;
	public float moveTargetB;
	public float movePos;

	private AudioSource au;
	private float timeCount;
	private bool playOnce;
	private float targetScale;
	private float maxScale = 1.2f;
	private float minScale = 0.0f;
	private bool isMaxScale;
	private bool scalePlus = true;
	private bool triggerIsAlready;

	void Awake()
	{
		isTriggered = false;

		au = gameObject.GetComponent<AudioSource>();

		targetScale = maxScale;
	}

	// Update is called once per frame
	void Update()
	{
		if (Device.isCameraTriggered) isTriggered = true;
		if (Device.isTriggered)
		{
            timeCount += Time.deltaTime;
			if (timeCount >= 1.0f && !triggerIsAlready)
			{
				//transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, new Vector3(this.transform.localPosition.x, movePos, this.transform.localPosition.z), 6.5f * Time.deltaTime);  //	2秒で指定された位置に移動する
				if (scalePlus)
				{
					if (transform.localScale.x <= targetScale && !isMaxScale)
					{
						transform.localScale += new Vector3(0.04f, 0, 0);
					}
					else if (transform.localScale.x > targetScale)
					{
						isMaxScale = true;
						targetScale = 1.0f;
						transform.localScale -= new Vector3(0.03f, 0, 0);
					}
					else if (transform.localScale.x < targetScale)
					{
						transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
						triggerIsAlready = true;
					}
				}
				else
				{
					if (transform.localScale.x > targetScale)
					{
						transform.localScale -= new Vector3(0.04f, 0, 0);
					}
					else if (transform.localScale.x < targetScale)
					{
						transform.localScale = new Vector3(0, 0, 0);
						triggerIsAlready = true;
					}
				}

                if (!playOnce)
				{
					if (scalePlus)
					{
						transform.localScale = new Vector3(0, 1.0f, 1.0f);
					}
					au.Play();
					playOnce = true;
				}
			}
			else if (triggerIsAlready)
			{
				moveVec++;

				if (moveVec % 2 != 0)
				{
					targetScale = minScale;
					scalePlus = false;
					//movePos = moveTargetA;
				}
				else
				{
					targetScale = maxScale;
					scalePlus = true;
					isMaxScale = false;
					//movePos = moveTargetB;
				}
				playOnce = false;
				Device.isTriggered = false;
				timeCount = 1.0f;
				triggerIsAlready = false;
			}
		}
	}
}
