using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerInput pi;
    public GoalController goal;
    public float horizontalSpeed = 25.0f;
    public float verticalSpeed = 20.0f;

    private GameObject cameraHandle;
    private GameObject playerHandle;
    private float tempEulerX;
    private GameObject model;
    private bool temp;

    // Start is called before the first frame update
    void Awake()
    {
        cameraHandle = transform.parent.gameObject;

        playerHandle = cameraHandle.transform.parent.gameObject;

        model = playerHandle.GetComponent<ActorController>().model;

        goal = GameObject.Find("Goal").GetComponent<GoalController>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!goal.gameClear)
        {
            Vector3 tempModelEuler = model.transform.eulerAngles;

            playerHandle.transform.Rotate(Vector3.up, pi.Jright * horizontalSpeed * Time.fixedDeltaTime);
            tempEulerX -= pi.Jup * verticalSpeed * Time.fixedDeltaTime;
            tempEulerX = Mathf.Clamp(tempEulerX, -25, 15);

            cameraHandle.transform.localEulerAngles = new Vector3(tempEulerX, 0, 0);   //  縦の回転角を制限する

            model.transform.eulerAngles = tempModelEuler;
        }
        else
        {
            if (!temp)
            {
                transform.SetParent(null);

                //print(model.transform.forward.normalized);
                //transform.rotation = Quaternion.identity;
                transform.rotation = Quaternion.LookRotation(-model.transform.forward); //  カメラをプレイヤーの正方向に置く
                transform.Rotate(Vector3.right, 30.0f);                                 //  カメラをX軸に沿って30度を回転する
                //transform.LookRotation(model.transform);
                transform.localPosition = new Vector3(model.transform.position.x, model.transform.position.y, model.transform.position.z);
                transform.Translate(Vector3.forward * -15.0f);
                //model.transform.parent.transform.LookAt(transform);            

                temp = true;
                //transform.localScale = Vector3.one;
            }
            
        }
    }
}
