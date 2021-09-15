//-------------------------------------------------
// ファイル名		：FootPlateController.cs
// 概要				：アニメーションの制御
// 作成者			：鍾家同
// 更新内容			：2021/05/04 作成
//					：2021/08/04 更新　FootPlateを回転必要ためのフラグを追加
//-------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootPlateController : MonoBehaviour
{
	[Header("===調整用===")]
	public Transform targetUp;			// 上昇後の最終座標
	public Transform targetDown;		// 降下後の最終座標
	public float moveSpeed = 0.01f;		// 移動速度
	public float timeCountMax = 2.5f;	// 回復まで最大時間
	public bool needTurn = false;		// 回転必要かフラグ

	[Header("===監視用===")]
	[SerializeField]
	private bool nextUpward = false;	// True:次は上昇、False:次は降下
	public bool isTriggered = false;	// True:稼働中、False:停止中
	[SerializeField]
	private float timeCount;			// 回復まで所要時間

	void Start()
	{

	}

	void Update()
	{
		if (isTriggered)
		{
			timeCount += Time.deltaTime;
			if (timeCount <= 5.0f)
			{
				// 上昇開始
				if (nextUpward)
				{
					// 上昇後の最終座標になるまで直線移動
					if (this.transform.position != targetUp.position)
					{
						this.transform.position = Vector3.MoveTowards(this.transform.position, targetUp.position, moveSpeed);
					}
					// 上昇終了、降下状態を準備
					else if (timeCount > timeCountMax)
					{
						isTriggered = false;
						nextUpward = false;
						timeCount = 1.0f;
						// 回転が必要であれば、回転する
						//if (needTurn) this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
					}
				}
				// 降下開始
				else if (!nextUpward)
				{
					// 降下後の最終座標になるまで直線移動
					if (this.transform.position != targetDown.position)
					{
						this.transform.position = Vector3.MoveTowards(this.transform.position, targetDown.position, moveSpeed);
					}
					// 降下終了、上昇状態を準備
					else if (timeCount > timeCountMax)
					{
						isTriggered = false;
						nextUpward = true;
						timeCount = 1.0f;
						// 回転が必要であれば、回転する
						//if (needTurn) this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);
					}
				}
			}

		}
	}

	public void DeviceOnTriggered(string msg)
	{
		// ObjectController.csからのメッセージを受け
		if (msg == "sFootPlate")
		{
			if (!isTriggered) isTriggered = true;
			//Debug.Log("Receive the message.");
		}
	}
}
