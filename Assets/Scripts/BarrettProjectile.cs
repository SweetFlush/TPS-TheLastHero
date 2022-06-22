using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrettProjectile : MonoBehaviour
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
        float speed = 150f;
        rigid.velocity = transform.forward * speed;

        Invoke("ActivateLine", 0.1f);
        Invoke("DestroySelf", 10f);
    }


    private void OnTriggerEnter(Collider other)
    {
        DamagableObject obj = other.GetComponent<DamagableObject>();

        if (obj && obj.gameObject.tag != "Civillian")
        {
            Instantiate(bloodVfx, transform.position, Quaternion.identity);
            obj.TakeDamage(damage);
        }
        else
        {
            Instantiate(hitVfx, transform.position, Quaternion.identity);
        }
    }

    public void ActivateLine()
    {
        bulletLine.SetActive(true);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    } 
}
