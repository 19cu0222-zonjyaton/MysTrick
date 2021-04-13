using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerInput pi;

    public GameObject cameraHandle;
    public GameObject playerHandle;

    // Start is called before the first frame update
    void Awake()
    {
        cameraHandle = transform.parent.gameObject;
        playerHandle = cameraHandle.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //cameraHandle.transform.Rotate(new Vector3(0, 0, 0), pi.Jright * 10.0f * Time.deltaTime);
        playerHandle.transform.Rotate(Vector3.up, pi.Jright * 50.0f * Time.deltaTime);
        //cameraHandle.transform.Rotate(Vector3.right, pi.Jup * 50.0f * Time.deltaTime);
    }
}
