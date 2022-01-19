//-------------------------------------------------
// ファイル名		：PortalController.cs
// 概要				：ゲートの制御
// 作成者			：鍾家同
// 更新内容			：2021/08/16 作成
//-------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
	[Header("===調整用===")]
	public PortalController portalA;
	public PortalController portalB;
	public Transform targetA;
	public Transform targetB;
	public float moveTimeCount;
	public float moveSpeed = 5.0f;
	public bool needKey = false;
	[HideInInspector]
	public bool finMoving;

	private FootPlateDeviceController FootDevice;
	private Vector3 nextPosition;
	private bool isTriggered;

	void Start()
	{
		nextPosition = targetB.localPosition;
		finMoving = false;
	}
}
