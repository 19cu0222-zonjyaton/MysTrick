	using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowImageTrigger : MonoBehaviour
{
    
	public GameObject hintUI;
	public int tutorialIndex;
	public float maxCount;

	private TutorialManager tutorManObj;
	private PlayerInput pi;
	private CameraController cc;
	private bool firstEnter = true;
	private bool isInCollider = false;

    // Start is called before the first frame update
    void Awake()
    {
        tutorManObj = GameObject.Find("TutorialUI").GetComponent<TutorialManager>();
		pi = GameObject.Find("PlayerHandle").GetComponent<PlayerInput>();
		cc = GameObject.Find("Main Camera").GetComponent<CameraController>();
	}

    // Update is called once per frame
    void Update()
    {

		// 接触式になったため、コメントします。
		// 元は接近ボタン式
		/*
		if (pi.isTriggered && isInCollider)
		{
			if (tutorManObj != null)
			{
				if (tutorialIndex < tutorManObj.uiObjects.Length && tutorManObj.uiObjects[tutorialIndex] != null)
				{
					tutorManObj.curUiIndex = tutorialIndex;
					tutorManObj.uiObjects[tutorialIndex].doneFlag = false;
					tutorManObj.ShowTutorial(true);
				} // end if()

				pi.isTriggered = false;
				Debug.Log(this.transform.name + " has touched.");
			} // end if()
		} // end if()
		*/

	}


	private void OnTriggerStay(Collider other)
	{
		// 接触式になったため、コメントします。
		// 元は接近ボタン式
		/*
		if (firstEnter == false)
		{
			if (other.transform.tag == "Player")
			{
				isInCollider = true;
				if (hintUI != null)
				{
					if (cc.cameraStatic == "Idle")
					{
						hintUI.SetActive(true);
					} // end if()
					else
					{
						hintUI.SetActive(false);
					} // end else
				} // end if()

			} // end if()
		} // end if()
		*/
	} // void OnTriggerStay()

	private void OnTriggerExit(Collider other)
	{
		if (other.transform.tag == "Player")
		{
			// 接触式になったため、コメントします。
			// 元は接近ボタン式
			/*
			if (hintUI != null)
			{
				hintUI.SetActive(false);
			} // end if()
			*/

			isInCollider = false;
		} // end if()
	} // void OnTriggerExit()

	private void OnTriggerEnter(Collider other)
	{
		// 接触式になったため、コメントします。
		// 元は接近ボタン式
		//if (firstEnter && other.transform.tag == "Player")

		if ( other.transform.tag == "Player")
		{
			isInCollider = true;
			if (tutorManObj != null)
			{
				if (tutorialIndex < tutorManObj.uiObjects.Length && tutorManObj.uiObjects[tutorialIndex] != null)
				{
					tutorManObj.curUiIndex = tutorialIndex;
					tutorManObj.uiObjects[tutorialIndex].doneFlag = false;
					tutorManObj.ShowTutorial(true);
					firstEnter = false;
				} // end if()
			} // end if()
		} // end if()

		Debug.Log(this.transform.name + " has Enter.!");
	}

}