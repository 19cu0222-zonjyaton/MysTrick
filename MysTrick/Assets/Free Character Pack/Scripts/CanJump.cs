using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanJump : MonoBehaviour
{
    public bool playerIsLanding;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.tag == "Stairs")
        {
            Debug.Log(collider.transform.tag);
            playerIsLanding = true;
        }
    }

    //void OnTriggerExit(Collider collider)
    //{
    //    if (collider.transform.gameObject != null)
    //    {
    //        playerIsLanding = false;
    //    }
    //}
}
