//-------------------------------------------------
// ファイル名		：RailController.cs
// 概要				：フェンスの制御
// 作成者			：鍾家同
// 更新内容			：2021/06/15 作成
//-------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailController : MonoBehaviour
{
	[Header("===調整用===")]
	public LadderController Ladder;
	public int[] numToMove;			// 移動タイミング
	public float moveDis = 1.8f;	// 移動距離
	private int i = 0;				// 要素数
	private float moveSpeed;		// 移動スピード
	private bool canRailMove;		// フェンスの移動可能フラグ
	private Vector3 curPosition;	// 初期位置
	[Header("===監視用===")]
	public bool canPlayerMove;		// プレイヤーの移動可能フラグ
	[SerializeField]
	private float timeCount;		// 回転時間（初期値）
	[SerializeField]
	private float timeMax;			// 移動時間（最大値）
	[SerializeField]
	private float timeReset;		// 回転時間（リセット）

	void Start()
	{
		canRailMove = false;
		canPlayerMove = false;
		curPosition = this.transform.localPosition;
		timeCount = Ladder.timeCount;
		timeMax = Ladder.timeMax;
		timeReset = timeCount;
		moveSpeed = Ladder.speed;
	}

	void Update()
	{
		// 梯子の値により移動を行う
		if (Ladder.canRotate) canRailMove = true;
		else canRailMove = false;

		// 移動開始
		if (canRailMove)
		{
			timeCount += Time.deltaTime;
			if (timeCount <= timeMax && timeCount >= 0.0f && numToMove[i] == Ladder.i)
			{
				this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition,
					new Vector3(curPosition.x, curPosition.y + moveDis, curPosition.z),
					moveSpeed * Time.deltaTime);
				canPlayerMove = true;
			}
			else if (timeCount <= timeMax && timeCount >= 0.0f && this.transform.localPosition != curPosition)
			{
				this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition,
					new Vector3(curPosition.x, curPosition.y, curPosition.z),
					moveSpeed * Time.deltaTime);
				canPlayerMove = false;
			}
			else if (timeCount > timeMax)
			{
				i = (i + 1) % 4;
				timeCount = timeReset;
				canPlayerMove = false;
			}
		}
	}
}
