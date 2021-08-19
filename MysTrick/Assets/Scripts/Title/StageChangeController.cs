using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageChangeController : MonoBehaviour
{
    public GameObject[] stage;
    public GameObject skyBox;

    private int stageIndex;

    // Start is called before the first frame update
    void Start()
    {
        RenderSettings.skybox = skyBox.GetComponent<SkyBoxController>().boxs[stageIndex];
    }

    // Update is called once per frame
    void Update()
    {
        if (stage[stageIndex].transform.eulerAngles.y > 355.0f)
        {
            stage[stageIndex].SetActive(false);
            stage[stageIndex].transform.eulerAngles = new Vector3(0, 0, 0);

            if (stageIndex < 3)
            {
                stageIndex++;
                RenderSettings.skybox = skyBox.GetComponent<SkyBoxController>().boxs[stageIndex];
                stage[stageIndex].SetActive(true);
            }
            else
            {
                RenderSettings.skybox = skyBox.GetComponent<SkyBoxController>().boxs[0];
                stage[0].SetActive(true);
                stageIndex = 0;
            }
        }
    }
}
