using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherGranade : MonoBehaviour
{
    [SerializeField] private Transform hitVfx;

    public int damage;

    private Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        float speed = 50f;
        rigid.velocity = transform.forward * speed;
    }


    private void OnTriggerEnter(Collider other)
    {
        //DamagableObject obj = other.GetComponent<DamagableObject>();
        //
        //if (obj)
        //{
        //
        //    obj.TakeDamage(damage);
        //}
        //else
        //{
        //    Instantiate(hitVfx, transform.position, Quaternion.identity);
        //}

        Instantiate(hitVfx, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

}
