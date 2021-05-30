using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageController : MonoBehaviour
{
    public bool isDamage;
    public int enemyHp;
    private Rigidbody rigid;
    private float timeCount;

    void Awake()
    {
        rigid = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isDamage)
        {
            timeCount += Time.deltaTime;
            if (timeCount > 1.0f)
            {
                isDamage = false;

                timeCount = 0.0f;
            }
        }

        if (enemyHp <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.tag == "Weapon" && !isDamage)
        {
            enemyHp--;
            isDamage = true;
            rigid.AddForce(0.0f, 500.0f, 0.0f);
            rigid.AddExplosionForce(300.0f, collider.transform.position, 5.0f);
        }
    }
}
