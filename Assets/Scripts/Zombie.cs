using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    public GameManager gameManager;

    public enum State
    {
        Idle,
        Chasing,
        Moving,
        Attacking,
        Dead,
    }

    public AudioClip[] hitSound;
    public AudioClip[] deadSound;
    public AudioClip[] groanSound;

    public int health = 100;
    public int damage = 10;
    public State state = State.Idle;
    public Transform destination;

    private bool isDead;
    private bool alreadyAttacked = false;
    public GameObject[] targetList = new GameObject[25];
    public float[] distances = new float[25];
    public float timer;
    public float shootTimer;
    public float speedThreshold = 0.3f;


    private float minDistance;
    private float shootRate = 2.0f;
    private float chaseRange = 1000f;
    private float attackRange = 1f;

    private float timeBetweenAttacks = 1.0f;

    private NavMeshAgent nav;
    private AudioSource audioSource;
    private Animator anim;
    private CapsuleCollider col;
    private Rigidbody rigid;

    private void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();

        int targetCivillianOnly = UnityEngine.Random.Range(0, 10);
        if(targetCivillianOnly < 4)
        {
            targetList = GameObject.FindGameObjectsWithTag("Civillian");
        }
        else
            targetList = GameObject.FindGameObjectsWithTag("Player");


        anim = GetComponent<Animator>();
        destination = CalculateDistances();
        col = GetComponent<CapsuleCollider>();
        rigid = GetComponent<Rigidbody>();
        gameManager = FindObjectOfType<GameManager>();

        StartCoroutine(Groan());
    }


    //추격 시작 거리 100, 공격 시작 50
    private void Update()
    {
        if (!isDead)
        {
            switch (state)
            {
                case State.Idle: UpdateIdleState(); break;
                case State.Chasing: UpdateChaseState(); break;
                case State.Attacking: UpdateAttackState(); break;
                case State.Dead: UpdateDeadState(); break;
            }

            if (health <= 0)
                state = State.Dead;

            timer += Time.deltaTime;
            //일정 시간마다 거리값 계산
            if (timer >= 2.0f)
            {
                destination = CalculateDistances();
                timer = 0f;
            }

            if (shootTimer < shootRate)
            {
                shootTimer += Time.deltaTime;
            }
        }
    }

    private void UpdateIdleState()
    {
        //Idle일때는 대기하고있다가, 거리를 만족하면 추격한다
        nav.isStopped = true;
        anim.SetBool("isMoving", false);
        //destination = CalculateDistances();
        if (destination != null)
        {
            nav.SetDestination(destination.position);
            float distance = Vector3.Distance(transform.position, destination.position);
            if (distance <= chaseRange)
            {
                state = State.Chasing;
                nav.isStopped = false;
            }
        }
    }

    private void UpdateChaseState()
    {
        if (destination != null)
        {
            //추격상태가 되면 거리가 가장 가까운 플레이어를 타깃으로 삼는다.
            //destination = CalculateDistances();
            nav.SetDestination(destination.position);
            nav.isStopped = false;
            anim.SetBool("isMoving", true);
            transform.LookAt(destination.position);

            //목적지 플레이어가 유효하면 공격하고, 아니면 다른 추격대상을 삼아야함..
            if (destination)
            {
                //거리가 일정치 이상 가까워지면 공격한다
                float distance = Vector3.Distance(transform.position, destination.position);
                if (distance <= attackRange)
                {
                    state = State.Attacking;
                }
                //너무 멀어지면 추격을 중단한다
                else if (distance >= chaseRange)
                {
                    state = State.Idle;
                }
            }
            else
            {
                destination = CalculateDistances();
            }

        }

        else
        {
            state = State.Idle;
        }
    }

    private void UpdateAttackState()
    {
        //플레이어가 유효해야함
        if (destination)
        {
            transform.LookAt(destination.position);
            float distance = Vector3.Distance(transform.position, destination.position);
            if (distance <= attackRange)
            {
                transform.LookAt(destination.position);
                nav.isStopped = true;
                anim.SetBool("isMoving", false);

                AttackPlayer();
            }
            else if (distance > attackRange && distance < chaseRange && !alreadyAttacked)
            {
                nav.isStopped = false;
                state = State.Chasing;
            }
            else if (distance > chaseRange)
            {
                state = State.Idle;
            }
        }
    }

    private Transform CalculateDistances()
    {
        //플레이어들 간의 거리를 계산하는 함수
        int i = 0;
        GameObject temp = null;
        minDistance = 1000000f;
        foreach (GameObject player in targetList)
        {
            //플레이어가 죽는 등 해서 없어지면, 쫓지 않는다
            if (!player)
            {
                distances[i] = 1000000f;
                targetList[i] = null;
            }
            else
            {   //거리값을 계산해서 대입한다
                distances[i] = Vector3.Distance(transform.position, player.transform.position);

                if (distances[i] < minDistance)
                {   //거리가 제일 가까운 플레이어를 타겟팅
                    minDistance = distances[i];
                    temp = player;
                }
            }
            i++;
        }

        if (temp)
            return temp.transform;
        else
            return null;
    }

    private void UpdateDeadState()
    {
        nav.isStopped = true;
        anim.SetBool("isMoving", false);
        anim.SetBool("isDead", true);
        col.enabled = false;
        Destroy(gameObject, 10f);
    }

    private void AttackPlayer()
    {
        if (!alreadyAttacked)
        {
            Shoot();
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void Shoot()
    {
        RaycastHit hit; //Ray생성 
        Vector3 dir = destination.position - transform.position;

        Vector3 zombiePosition = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);

        Debug.DrawRay(zombiePosition, dir, Color.green, 3.0f);
        if (Physics.Raycast(zombiePosition, dir, out hit, attackRange + 5f)) //충돌되는 object가 있는지 검사 
        {
            if (hit.transform.gameObject.tag == "Player")
                anim.SetTrigger("doAttack");
            else
                anim.SetTrigger("doBite");

            DamagableObject obj = hit.transform.GetComponent<DamagableObject>();
            if (obj != null)
            {
                obj.TakeDamage(20);
            }

        }
    }

    public void Exploded()
    {
        rigid.AddForce(Vector3.up * 30f, ForceMode.Impulse);
    }

    public void TakeDamage(int damage) 
    {
        int r = Random.Range(0, 1);
        audioSource.PlayOneShot(hitSound[r]);
        health -= damage;

        if (health <= 0)
        {
            audioSource.volume = 0.5f;
            r = Random.Range(0, 4);
            audioSource.PlayOneShot(deadSound[r]);
            gameManager.KilledZombie();
        }

    }

    private IEnumerator Groan()
    {
        int i = Random.Range(0, 8);
        float r = Random.Range(10f, 60f);

        audioSource.PlayOneShot(groanSound[i]);
        yield return new WaitForSeconds(r);

        r = Random.Range(10f, 60f);
        audioSource.PlayOneShot(groanSound[i]);
        yield return new WaitForSeconds(r);

        r = Random.Range(10f, 60f);
        audioSource.PlayOneShot(groanSound[i]);
        yield return new WaitForSeconds(r);
    }
}
