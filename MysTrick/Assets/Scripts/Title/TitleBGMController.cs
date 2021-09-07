using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleBGMController : MonoBehaviour
{
    public static TitleBGMController tbc;

    void Awake()
    {
        if (tbc == null)
        {
            tbc = this;
        }
        else if (tbc != this)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
}
