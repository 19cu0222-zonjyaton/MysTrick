//-------------------------------------------------
// ファイル名		：FootPlate_MoveBoxCheck.cs
// 概要				：足掛けとボックスの接触状況
// 作成者			：鍾家同
// 更新内容			：2021/09/10 作成
//-------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootPlate_MoveBoxCheck : MonoBehaviour
{
	public bool isMoveBox;      // ボックスがオブジェクトと接触するかどうかフラグ

	void OnTriggerStay(Collider other)
	{
		// ボックスがオブジェクトと接触した場合
		if (other.transform.tag == "MoveBox")
		{
			isMoveBox = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
		// ボックスがオブジェクトから離れた場合
		if (other.transform.tag == "MoveBox")
		{
			isMoveBox = false;
		}
	}
}
