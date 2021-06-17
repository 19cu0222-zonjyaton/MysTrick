using UnityEngine;
using UnityEngine.UI;

public class HPUIController : MonoBehaviour
{
    public Sprite[] sprite;     //  画像オブジェクト
    public GameObject heart;    //  ハートのUI
    private GameObject[] hp;    //  作るUIの用意場所
    private GameObject hpUI;    //  親のオブジェクト
    private ActorController ac;

    void Awake()
    {
        ac = GameObject.Find("PlayerHandle").GetComponent<ActorController>();

        hp = new GameObject[ac.hp];

        hpUI = GameObject.Find("HP").gameObject;
    }

    void Start()
    {
            //   動態にHP UIを作る
            for (int i = 0; i < ac.hp; i++)
            {
                hp[i] = Instantiate(heart, transform.position + new Vector3(i * 80.0f, 0.0f, 0.0f), Quaternion.identity);

                hp[i].transform.SetParent(hpUI.transform);
            }
    }

    void Update()
    {
        //  減ったHPに応じて画像が変わる
        for (int i = 2; i >= ac.hp; i--)
        {
            hp[i].GetComponent<Image>().sprite = sprite[1];
        }
    }
}
