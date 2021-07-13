using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class TitleButtonController : MonoBehaviour
{
    public float perRadian;             //  毎回変化の弧度
    public float radius;                //  半径
    public float radian;                //  弧度
    public Vector3 oldPos;             //  初期位置

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == this.gameObject)
        {
            radian += perRadian;                //  弧度をプラスする
            float dy = Mathf.Cos(radian) * radius;
            transform.position = oldPos + new Vector3(0, dy, 0);

            if (radian >= 360.0f)
            {
                radian = 0.0f;
            }
        }
        else
        {
            radian = 0.0f;
        }
    }
}
