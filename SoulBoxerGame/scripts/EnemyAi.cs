using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public Transform rightHand;
    public LayerMask Groundlayer, Playerlayer;

    public float health;

    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;
    public Transform hp_bar;
    
    public float sightRange, attackRange;
    public bool playerInsightRange, playerInattackRange;
    
    private void Awake()
    {
        
    }

    void Start()
    {
        player = GameObject.Find("Head").transform;
        agent = GetComponent<NavMeshAgent>();
        
    }

    // Update is called once per frame
    void Update()
    {
        playerInsightRange = Physics.CheckSphere(transform.position, sightRange, Playerlayer);
        playerInattackRange = Physics.CheckSphere(transform.position, attackRange, Playerlayer);

        if (!playerInsightRange && !playerInattackRange) Patroling();
        if (playerInsightRange && !playerInattackRange) ChasePlayer();
        if (playerInsightRange && playerInattackRange) AttackPlayer();

        if (health == 0)
        {
            //print("dead");
            Destroy(gameObject);
        }
    }
    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, Groundlayer))
            walkPointSet = true;
    }
    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }
    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);       
        
    }
    /*private void ResetAttack()
    {
        rightHand.Rotate(new Vector3(-20f, 0f, 0f));
    }*/
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == 10)
        {
            Hit();
            hp_bar.localScale = new Vector3(0.2f, hp_bar.localScale.y - 1f, 0.2f);
        }
    }

    void Hit()
    {
        health = health - 50;
        print(health);
    }
}
