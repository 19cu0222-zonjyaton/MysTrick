using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotCheck : MonoBehaviour
{
    public MoveBoxController mbc;
    public GameObject player;
    public GameObject playerModule;
    public ActorController ac;
    public CapsuleCollider tempCollider;
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
            player.gameObject.transform.SetParent(mbc.transform);
        }
    }

	private void OnTriggerStay(Collider other)
	{
		if (other.transform.tag == "Player")
		{
            if (mbc.moveWithPlayer)
            {
                if (!doOnce)
                {
					tempCollider.enabled = true;
                    ac.isPushBox = true; 
                    doOnce = true;
                }
                player.transform.position = new Vector3(transform.position.x, player.transform.position.y, transform.position.z);                  
            }
            else if(!mbc.moveWithPlayer && doOnce)
            {
                tempCollider.enabled = false;
                player.gameObject.transform.SetParent(null);
                ac.isPushBox = false;
                doOnce = false;
            }
		}
	}
}
