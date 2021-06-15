using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalController : MonoBehaviour
{
    private PlayerInput pi;
    public bool gameClear;

    public float perRadian;         //  毎回変化の弧度
    public float radius;
    public float radian;               //  弧度
    private Vector3 oldPos;

    // Start is called before the first frame update
    void Awake()
    {
        pi = GameObject.Find("PlayerHandle").GetComponent<PlayerInput>();

        oldPos = transform.position;        //  最初の位置    
    }

    void Update()
    {
        if (!gameClear && Time.deltaTime != 0)
        {
            transform.Rotate(0, 2.0f, 0);

            radian += perRadian;                //  毎回弧度を0.01をプラスする
            float dy = Mathf.Cos(radian) * radius;
            transform.position = oldPos + new Vector3(0, dy, 0);

            if (radian >= 360.0f)
            {
                radian = 0.0f;
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.tag == "Player")
        {
            transform.SetParent(collider.transform);

            gameClear = true;

            pi.inputEnabled = false;
        }
    }
}
