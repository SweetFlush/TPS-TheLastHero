using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Civillian : MonoBehaviour
{
    public Transform trainStation;
    public AudioClip[] breathSound;
    public AudioClip deadSound;

    public int health = 1;
    public bool isDead = false;

    private NavMeshAgent nav;
    private Animator anim;
    private GameManager gameManager;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        trainStation = GameObject.Find("TrainStationTransform").transform;
        nav.SetDestination(trainStation.position);
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        StartCoroutine(Breath());
    }

    private void Update()
    {
        //transform.LookAt(transform.forward);
    }

    public void TakeDamage(int damage)
    {
        if(!isDead)
            gameManager.SurvivorDead();

        health -= damage;
        isDead = true;
        anim.SetBool("isDead", isDead);
        nav.isStopped = true;
        Destroy(gameObject, 4f);
    }

    public void Arrived()
    {
        Destroy(gameObject);
    }

    private IEnumerator Breath()
    {
        int i = Random.Range(0, 3);
        float r = Random.Range(20f, 120f);

        audioSource.PlayOneShot(breathSound[i]);
        yield return new WaitForSeconds(r);

        i = Random.Range(0, 3);
        r = Random.Range(10f, 80f);

        audioSource.PlayOneShot(breathSound[i]);
        yield return new WaitForSeconds(r);
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log("TriggerEnter");
    //    if(other.gameObject.tag == "TrainStation")
    //    {
    //        gameManager.SurvivorArrived();
    //        Destroy(gameObject);
    //    }
    //}
}
