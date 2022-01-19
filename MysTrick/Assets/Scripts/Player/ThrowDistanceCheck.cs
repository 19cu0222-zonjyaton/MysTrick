using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowDistanceCheck : MonoBehaviour
{
    public PlayerInput pi;      //  プレイヤーの入力コントローラー

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            pi.overDistance = true;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            pi.overDistance = false;
        }
    }
}
