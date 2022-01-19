//-------------------------------------------------
// ファイル名		：FootPlate_PlayerCheck.cs
// 概要				：プレートとプレイヤーの接触状況
// 作成者			：鍾家同
// 更新内容			：2021/09/10 作成
//-------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootPlate_PlayerCheck : MonoBehaviour
{
	public bool isPlayer;			// ボックスがプレイヤーと接触するかどうかフラグ

	void OnTriggerStay(Collider other)
	{
		// プレイヤーがオブジェクトと接触した場合
		if (other.transform.tag == "Player")
		{
			isPlayer = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
		// プレイヤーがオブジェクトから離れた場合
		if (other.transform.tag == "Player")
		{
			isPlayer = false;
		}
	}
}
