using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialManager : MonoBehaviour
{
    public TutorialImageController[] uiObjects;
    public int curUiIndex;

    // Start is called before the first frame update
    void Start()
    {
    }


    // Update is called once per frame
    void Update()
    {
        for ( int i = 0; i < uiObjects.Length; ++i )
        {
            if (uiObjects[i] != null)
            { 
                if ( i != curUiIndex && uiObjects[i].doneFlag == false)
                {
                    uiObjects[i].isOpenMenu = false;
                    uiObjects[i].gameObject.SetActive(false);
                } // end if

                if (i == curUiIndex && uiObjects[i].doneFlag )
                {
                    uiObjects[i].isOpenMenu = false;
                    uiObjects[i].gameObject.SetActive(false);
                } // end if
            } // end if
        } // end for

        /*
        // 要改成ESC那邊做控制
        // 用CALL 這的FUNC 來關
        if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("menu")))
        {
            Time.timeScale = 1;
            uiObjects[curUiIndex].animator.SetBool("Menu", false);
            uiObjects[curUiIndex].isOpenMenu = false;
            //uiObjects[curUiIndex].gameObject.SetActive(false);
        } // end if

    */
    } // void Update()


    public void ShowTutorial( bool isActive )
    {
        if (curUiIndex < uiObjects.Length)
        {
            if (uiObjects[curUiIndex] != null)
            {
                if (isActive)
                {
                    uiObjects[curUiIndex].gameObject.SetActive(true);
                    uiObjects[curUiIndex].isOpenMenu = true;
                    EventSystem.current.SetSelectedGameObject(uiObjects[curUiIndex].selectButton);
                } // end if()
                else
                {
                    uiObjects[curUiIndex].isOpenMenu = false;
                    uiObjects[curUiIndex].gameObject.SetActive(false);
                } // end else
            } // end if()
        } // end if()

    } // void ShowTutorial()
}
