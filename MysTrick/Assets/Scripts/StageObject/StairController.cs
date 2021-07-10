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

	private new AudioSource audio;
	private float timeCount;
	private bool playOnce;

	void Awake()
	{
		isTriggered = false;

		audio = gameObject.GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update()
	{
		if (Device.isCameraTriggered) isTriggered = true;
		if (Device.isTriggered)
		{
            timeCount += Time.deltaTime;
            if (timeCount >= 1.0f && timeCount <= 2.5f)
            {
                transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, new Vector3(this.transform.localPosition.x, movePos, this.transform.localPosition.z), 6.5f * Time.deltaTime);  //	2秒で指定された位置に移動する
				if (!playOnce)
				{
					audio.Play();
					playOnce = true;
				}
			}
            else if (timeCount > 2.5f)
            {
				moveVec++;

                if (moveVec % 2 != 0)
                {
					movePos = moveTargetA;
                }
                else
                {
					movePos = moveTargetB;
                }
				playOnce = false;
				Device.isTriggered = false;
                timeCount = 1.0f;
            }
        }
	}
}
