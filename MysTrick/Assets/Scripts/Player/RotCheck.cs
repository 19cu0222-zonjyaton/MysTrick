using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotCheck : MonoBehaviour
{
    public MoveBoxController mbc;
    public GameObject player;
    public GameObject playerModule;
    public ActorController ac;
    public BoxCollider tempCollider;

    private bool doOnce;

    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (ac.isPushBox && doOnce)
        {
            playerModule.transform.rotation = transform.rotation;
            mbc.gameObject.transform.SetParent(playerModule.transform);
        }
    }

	private void OnTriggerStay(Collider other)
	{
		if (other.transform.tag == "Player")
		{
            if (mbc.moveWithPlayer && !doOnce)
            {
                Physics.IgnoreLayerCollision(11, 19, true);
                tempCollider.enabled = true;
                ac.isPushBox = true;
                player.transform.position = new Vector3(transform.position.x, player.transform.position.y, transform.position.z);                  
                doOnce = true;
            }
            else if(!mbc.moveWithPlayer)
            {
                Physics.IgnoreLayerCollision(11, 19, false);
                tempCollider.enabled = false;
                mbc.gameObject.transform.SetParent(null);
                ac.isPushBox = false;
                doOnce = false;
            }
		}
	}
}
