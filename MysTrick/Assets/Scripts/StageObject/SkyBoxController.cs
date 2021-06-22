using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBoxController : MonoBehaviour
{
    public Material[] boxs;
    public ActorInStageSelect aiss;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < boxs.Length; i++)
        {
            if (aiss.skyboxIndex == i)
            {
                RenderSettings.skybox = boxs[i];
            }
        }
    }
}
