using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalController : MonoBehaviour
{
    private PlayerInput pi;

    public bool gameClear;

    // Start is called before the first frame update
    void Awake()
    {
        pi = GameObject.Find("PlayerHandle").GetComponent<PlayerInput>();
    }

    void FixedUpdate()
    {

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
