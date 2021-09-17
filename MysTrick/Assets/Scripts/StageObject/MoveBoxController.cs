using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBoxController : MonoBehaviour
{
	public GameObject hintUI;
	public GameObject player;
	public bool moveWithPlayer;
	public Animator anim;

	private PlayerInput pi;
	private ActorController ac;
	private Rigidbody rigid;
	private Vector3 movingVec;              //	移動方向

	void Awake()
    {
		pi = player.GetComponent<PlayerInput>();

		ac = player.GetComponent<ActorController>();

		rigid = gameObject.GetComponent<Rigidbody>();
	}

    // Update is called once per frame
    void FixedUpdate()
    {
		if (moveWithPlayer)
		{
			if (pi.Dmag > 0.1f)     //	1.移動の入力値が0.1を超える時	2.狙う状態ではない時	->	 移動方向を設定する
			{
				anim.speed = 1.0f;
				anim.SetBool("Push", true);
				movingVec = pi.Dmag * pi.Dvec;
				transform.position += movingVec * 3.0f * Time.fixedDeltaTime;
			}
			else
			{
				movingVec = new Vector3(0, 0, 0);
				if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && anim.GetCurrentAnimatorStateInfo(0).IsName("Push"))
				{
					anim.speed = 0.0f;
				}
			}
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.transform.tag == "Player")
		{
			if (!moveWithPlayer)
			{
				hintUI.SetActive(true);
			}

			if (pi.isTriggered)
			{
				if (moveWithPlayer)
				{
					anim.SetBool("Push", false);
					anim.SetBool("PrePush", false);
					rigid.isKinematic = true;
					moveWithPlayer = false;
					anim.speed = 1.0f;
					ac.moveSpeed = 3.0f;
				}
				else
				{
					anim.SetBool("PrePush", true);
					pi.ResetSignal();
					rigid.isKinematic = false;
					hintUI.SetActive(false);
					moveWithPlayer = true;
					ac.moveSpeed = 7.0f;
				}

				pi.isTriggered = false;
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.transform.tag == "Player")
		{
			hintUI.SetActive(false);
		}
	}
}
