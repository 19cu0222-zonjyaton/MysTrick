using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerInput pi;
    public GoalController goal;
    public float horizontalSpeed = 25.0f;
    public float verticalSpeed = 20.0f;
    public StairController[] stair;
    public GameObject temp;
    public string cameraStatic = "Idle";

    private GameObject cameraHandle;
    private GameObject playerHandle;
    private float tempEulerX;
    private GameObject model;
    private bool doOnce;
    private float countTime = 20.0f;

    // Start is called before the first frame update
    void Awake()
    {
        cameraHandle = transform.parent.gameObject;

        playerHandle = cameraHandle.transform.parent.gameObject;

        model = playerHandle.GetComponent<ActorController>().model;

        goal = GameObject.Find("Goal").GetComponent<GoalController>();

        temp = GameObject.Find("CameraPos");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (stair[0].moveToHere && !stair[0].hasDone)
        {
            cameraStatic = "MoveToStair1";

            pi.inputEnabled = false;

            stair[0].moveToHere = false;

            stair[0].hasDone = true;
        }

        if (cameraStatic == "MoveToStair1")
        {
            countTime -= Time.fixedDeltaTime;

            if (countTime <= 20.0f && countTime > 12.0f)
            {
                transform.parent = null;

                transform.localPosition = Vector3.Slerp(transform.localPosition, new Vector3(25.0f, 40.0f, 15.0f), 0.01f);

                transform.LookAt(stair[0].transform.position);
            }
            else if (countTime <= 8.0f && countTime >= 0.0f)
            {
                transform.SetParent(cameraHandle.transform);

                transform.localPosition = Vector3.Slerp(transform.localPosition, temp.transform.localPosition, 0.01f);

                transform.rotation = Quaternion.Slerp(transform.rotation, temp.transform.rotation, Time.fixedDeltaTime * 1.0f);
            }
            else if(countTime < 0.0f)
            {                
                cameraStatic = "Idle";

                countTime = 20.0f;
            
                pi.inputEnabled = true;
            }

        }

        if (!goal.gameClear && cameraStatic == "Idle")
        {
            Vector3 tempModelEuler = model.transform.eulerAngles;

            playerHandle.transform.Rotate(Vector3.up, pi.Jright * horizontalSpeed * Time.fixedDeltaTime);
            tempEulerX -= pi.Jup * verticalSpeed * Time.fixedDeltaTime;
            tempEulerX = Mathf.Clamp(tempEulerX, -25, 15);

            cameraHandle.transform.localEulerAngles = new Vector3(tempEulerX, 0, 0);   //  縦の回転角を制限する

            model.transform.eulerAngles = tempModelEuler;
        }
        //else
        //{
        //    if (!doOnce)
        //    {
        //        transform.SetParent(null);

        //        //print(model.transform.forward.normalized);
        //        //transform.rotation = Quaternion.identity;
        //        transform.rotation = Quaternion.LookRotation(-model.transform.forward); //  カメラをプレイヤーの正方向に置く
        //        transform.Rotate(Vector3.right, 30.0f);                                 //  カメラをX軸に沿って30度を回転する
        //        //transform.LookRotation(model.transform);
        //        transform.localPosition = new Vector3(model.transform.position.x, model.transform.position.y, model.transform.position.z);
        //        transform.Translate(Vector3.forward * -15.0f);
        //        //model.transform.parent.transform.LookAt(transform);            

        //        doOnce = true;
        //        //transform.localScale = Vector3.one;
        //    }
            
        //}
    }
}
