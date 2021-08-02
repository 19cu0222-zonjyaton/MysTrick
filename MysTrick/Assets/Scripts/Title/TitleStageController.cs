using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleStageController : MonoBehaviour
{
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;//	FPSを60に固定する
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0.15f, 0);
    }
}
