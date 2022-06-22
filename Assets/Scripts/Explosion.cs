using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private SphereCollider sphereCollider;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        audioSource = GetComponent<AudioSource>();

        audioSource.Play();

        Explode();

        Invoke("DeactivateCollider", 1f);
    }


    private void DeactivateCollider()
    {
        sphereCollider.enabled = false;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //        DamagableObject obj = other.GetComponent<DamagableObject>();
    //
    //        if (obj)
    //        {
    //            Vector3 direction = (obj.transform.position - transform.position);
    //
    //            //표적 날리기
    //            obj.GetComponent<Rigidbody>()?.AddForce(Vector3.up * 30f, ForceMode.Acceleration);
    //            obj.TakeDamage(100);
    //        }
    //    
    //
    //    Destroy(gameObject, 7f);
    //}

    private void Explode()
    {
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 6, Vector3.up, 0f);

        //DamagableObject damagableObject = obj.transform.GetComponent<DamagableObject>();
        foreach (RaycastHit obj in rayHits)
        {
            if(obj.transform.tag == "Zombie")
            {
                Vector3 direction = (obj.transform.position - transform.position).normalized;

                obj.transform.GetComponent<Zombie>().Exploded();
                //표적 날리기
                obj.transform.GetComponent<Rigidbody>()?.AddForce(direction * 30f, ForceMode.Impulse);
                obj.transform.GetComponent<Rigidbody>()?.AddForce(Vector3.up * 30f, ForceMode.Impulse);
                obj.transform.GetComponent<DamagableObject>().TakeDamage(100);
            }
            
        }
    }
}
