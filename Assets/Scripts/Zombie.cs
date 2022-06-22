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


    //�߰� ���� �Ÿ� 100, ���� ���� 50
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
            //���� �ð����� �Ÿ��� ���
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
        //Idle�϶��� ����ϰ��ִٰ�, �Ÿ��� �����ϸ� �߰��Ѵ�
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
            //�߰ݻ��°� �Ǹ� �Ÿ��� ���� ����� �÷��̾ Ÿ������ ��´�.
            //destination = CalculateDistances();
            nav.SetDestination(destination.position);
            nav.isStopped = false;
            anim.SetBool("isMoving", true);
            transform.LookAt(destination.position);

            //������ �÷��̾ ��ȿ�ϸ� �����ϰ�, �ƴϸ� �ٸ� �߰ݴ���� ��ƾ���..
            if (destination)
            {
                //�Ÿ��� ����ġ �̻� ��������� �����Ѵ�
                float distance = Vector3.Distance(transform.position, destination.position);
                if (distance <= attackRange)
                {
                    state = State.Attacking;
                }
                //�ʹ� �־����� �߰��� �ߴ��Ѵ�
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
        //�÷��̾ ��ȿ�ؾ���
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
        //�÷��̾�� ���� �Ÿ��� ����ϴ� �Լ�
        int i = 0;
        GameObject temp = null;
        minDistance = 1000000f;
        foreach (GameObject player in targetList)
        {
            //�÷��̾ �״� �� �ؼ� ��������, ���� �ʴ´�
            if (!player)
            {
                distances[i] = 1000000f;
                targetList[i] = null;
            }
            else
            {   //�Ÿ����� ����ؼ� �����Ѵ�
                distances[i] = Vector3.Distance(transform.position, player.transform.position);

                if (distances[i] < minDistance)
                {   //�Ÿ��� ���� ����� �÷��̾ Ÿ����
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
        RaycastHit hit; //Ray���� 
        Vector3 dir = destination.position - transform.position;

        Vector3 zombiePosition = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);

        Debug.DrawRay(zombiePosition, dir, Color.green, 3.0f);
        if (Physics.Raycast(zombiePosition, dir, out hit, attackRange + 5f)) //�浹�Ǵ� object�� �ִ��� �˻� 
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
