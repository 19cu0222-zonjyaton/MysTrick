using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarBoxController : MonoBehaviour
{
    public PlayerInput pi;
    public ActorController ac;
    public GameObject hintUI;
    public GameObject boxCover;
    public GameObject lockKey;
    public GameObject star;
    public MeshRenderer[] starMesh;
    public float rotateSpeed;           //  回転スピード
    public Vector3 finalRot;
    public float tempRot;
    public bool uiAnimStart;

    private AudioSource sound;          //  SEコンポーネント
    private Animation starAnim;
    private Rigidbody starRigid;        //  鋼体コンポーネント
    private bool getByPlayer;           //  プレイヤーと当たったflag
    private bool animStart;
    private bool animOver = true;
    private bool activeHintUI = true;
    private bool doOnce;

    //  初期化
    void Awake()
    {
        sound = gameObject.GetComponent<AudioSource>();

        starRigid = star.gameObject.GetComponent<Rigidbody>();

        starAnim = star.gameObject.GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        if (getByPlayer)
        {
            boxCover.transform.rotation = Quaternion.Lerp(boxCover.transform.rotation, Quaternion.Euler(new Vector3(110, tempRot, 0)), 5.0f * Time.deltaTime);

            if (boxCover.transform.localEulerAngles.x > 85.0f && !animStart)
            {
                animStart = true;
            }

            if (animStart && animOver)
            {
                if (!doOnce)
                {
                    sound.Play();
                    rotateSpeed = 60.0f;
                    starRigid.AddForce(0, 150.0f, 0);
                    doOnce = true;
                }

                star.transform.Rotate(0, rotateSpeed, 0);
                rotateSpeed -= 1.5f;

                if (rotateSpeed <= 0.0f)
                {
                    starRigid.constraints = RigidbodyConstraints.FreezePosition;
                    star.transform.localEulerAngles = finalRot;

                    starAnim.Play();
                    animOver = false;
                }
            }
            else if (!animOver && !starAnim.isPlaying)
            {
                for (int i = 0; i < starMesh.Length; i++)
                {
                    starMesh[i].material.color -= new Color32(0, 0, 0, 10);
                }

                uiAnimStart = true;
                if (starMesh[0].material.color.a < 0.0f)
                {
                    getByPlayer = false;
                    Destroy(star.gameObject);
                }
            }
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.transform.tag == "Player")     //プレイヤーと当たる処理
        {
            if (activeHintUI)
            {
                hintUI.SetActive(true);
            }
            else
            {
                hintUI.SetActive(false);
            }

            if (pi.isTriggered && activeHintUI)
            {
                getByPlayer = true;

                activeHintUI = false;

                lockKey.GetComponent<Rigidbody>().useGravity = true;

                ac.starCount++;

                pi.isTriggered = false;
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.transform.tag == "Player")     //プレイヤーと当たる処理
        {
            hintUI.SetActive(false);
        }
    }
}
