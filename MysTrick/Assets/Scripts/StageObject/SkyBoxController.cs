using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBoxController : MonoBehaviour
{
    public Material[] boxs;
    public ActorInStageSelect aiss;

    void Awake()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        //if (aiss != null)
        //{
        //    for (int i = 0; i < boxs.Length; i++)
        //    {
        //        if (aiss.skyboxIndex == i)
        //        {
        //            RenderSettings.skybox = boxs[i];
        //        }
        //    }
        //}
        //else
        //{
        //    RenderSettings.skybox = boxs[0];
        //}
    }
}
