using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private Transform hitVfx;
    [SerializeField] private Transform bloodVfx;
    [SerializeField] private GameObject bulletLine;

    public int damage;

    private Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        float speed = 70f;
        rigid.velocity = transform.forward * speed;

        Invoke("ActivateLine", 0.1f);
    }


    private void OnTriggerEnter(Collider other)
    {
        DamagableObject obj = other.GetComponent<DamagableObject>();

        if (obj && obj.gameObject.tag != "Civillian" && obj.gameObject.tag != "Player")
        {
            Instantiate(bloodVfx, transform.position, Quaternion.identity);
            obj.TakeDamage(damage);
        }
        else if(other.tag != "Civillian")
        {
            Destroy(gameObject);
            Instantiate(hitVfx, transform.position, Quaternion.identity);
        }

    }

    public void ActivateLine()
    {
        bulletLine.SetActive(true);
    }
}
