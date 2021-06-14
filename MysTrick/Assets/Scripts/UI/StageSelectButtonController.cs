using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class StageSelectButtonController : MonoBehaviour
{
    public bool canSelected;
    public Button btn;
    public Image img;
    private Vector3 startPos;

    void Awake()
    {
        img = gameObject.GetComponent<Image>();

        btn = gameObject.GetComponent<Button>();

        btn.onClick.AddListener(StageSelect);

        startPos = transform.parent.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!canSelected)
        {
            img.color = Color.grey;
        }
        else
        {
            img.color = Color.white;
        }

        if (EventSystem.current.currentSelectedGameObject.name == "Stage04")
        {
            gameObject.transform.parent.position = Vector3.MoveTowards(transform.parent.position, startPos - new Vector3(300.0f, 0.0f, 0.0f), 500.0f * Time.deltaTime);
        }
        else if (EventSystem.current.currentSelectedGameObject.name == "Stage03")
        {
            gameObject.transform.parent.position = Vector3.MoveTowards(transform.parent.position, startPos, 500.0f * Time.deltaTime);
        }
    }

    public void StageSelect()
    {
        if (canSelected)
        {
            if (gameObject.name == "Stage01")
            {
                SceneManager.LoadScene("Stage01");
            }
            else if (gameObject.name == "Stage02")
            {
                SceneManager.LoadScene("Stage02");
            }
            else if (gameObject.name == "Stage03")
            {
                SceneManager.LoadScene("Stage03");
            }
            else if (gameObject.name == "Stage04")
            {
                SceneManager.LoadScene("Stage04");
            }
        }
    }
}
