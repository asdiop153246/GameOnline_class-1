using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

[RequireComponent(typeof(Animator))]
public class EnemyAi : NetworkBehaviour
{
    public NavMeshAgent agent;
    private Animator animator;
    public LayerMask groundMask, playerMask;
    public float patrolRange = 10f;
    public float sightRange = 20f;
    public float attackRange = 2f;
    public float timeBetweenAttacks = 3f;
    public float MoveSpeed = 5f;
    private bool isAttacking = false;
    public GameObject hitbox;
    private NetworkVariable<Vector3> walkPoint = new NetworkVariable<Vector3>(new Vector3());
    private bool walkPointSet = false;
    public float attackDamage = 15f;
    private bool hasPlayerBeenSpotted = false;
    public float dashSpeed = 20f;
    public float dashDuration = 1f;
    public float extraDashDistance = 1f;
    public float windUpDuration = 1.5f;
    private NetworkVariable<Vector3> position = new NetworkVariable<Vector3>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<Quaternion> rotation = new NetworkVariable<Quaternion>(new Quaternion());
    private GameObject homeCore;
    private bool isHomeCoreSet = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        SetMovementSpeed(MoveSpeed);
        FindHomeCore();
    }

    void Update()
    {
        if (!IsServer) return;
        animator.SetBool("Walk", false);        
        Collider[] playersInSightRange = Physics.OverlapSphere(transform.position, sightRange, playerMask);
        if (playersInSightRange.Length != 0)
        {
            float playerDistance = Vector3.Distance(transform.position, playersInSightRange[0].transform.position);
            if (!hasPlayerBeenSpotted)
            {
                hasPlayerBeenSpotted = true;
                StartCoroutine(DashAttack(playersInSightRange[0].transform.position));
                return;
            }

            // If player is within sight range but outside of attack range
            if (playerDistance > attackRange)
            {
                agent.SetDestination(playersInSightRange[0].transform.position);
                animator.SetBool("Walk", true);

            }
            else
            {
                // Player is within attack range, stop movement and look at player
                agent.ResetPath();
                animator.SetBool("Walk", false);
                transform.LookAt(playersInSightRange[0].transform);

                // Attack player
                AttackPlayer(playersInSightRange[0].gameObject);
            }
        }
        else
        {
            
            if (homeCore)
            {
                float distanceToHomeCore = Vector3.Distance(transform.position, homeCore.transform.position);
                if (distanceToHomeCore > attackRange)
                {
                    Debug.Log("Monster attempted to move to HomeCore");
                    MoveToTarget(homeCore.transform.position);
                }
                else
                {
                    AttackHomeCore(); 
                }
            }
            else
            {
                
                PatrolBehavior();
            }
        }
            if (IsServer)
        {
            position.Value = transform.position;
            rotation.Value = transform.rotation;
        }
        // Update position and rotation on the clients
        else
        {
            transform.position = position.Value;
            transform.rotation = rotation.Value;
        }

    }
    public void SetMovementSpeed(float newSpeed)
    {
        if (agent != null)
        {
            agent.speed = newSpeed;
        }
    }
    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-patrolRange, patrolRange);
        float randomX = Random.Range(-patrolRange, patrolRange);

        walkPoint.Value = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint.Value, -transform.up, 2f, groundMask))
            walkPointSet = true;
        else
            walkPointSet = false;
    }

    private void AttackPlayer(GameObject player)
    {
        if (isAttacking) return;

        agent.isStopped = true;
        
        hitbox.SetActive(true);
        Debug.Log("Attacking player!");
        animator.SetTrigger("Punch");

        isAttacking = true;

        Invoke(nameof(ResetAttack), timeBetweenAttacks);
    }
    private IEnumerator DashAttack(Vector3 targetPosition)
    {
        Vector3 directionToPlayer = (targetPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        yield return new WaitForSeconds(0.5f);
        agent.isStopped = true;
        animator.SetTrigger("Dash");
        yield return new WaitForSeconds(windUpDuration);

        float startTime = Time.time;

        Vector3 direction = (targetPosition - transform.position).normalized;
        Vector3 finalTargetPosition = targetPosition + direction * extraDashDistance;

        while (Time.time < startTime + dashDuration)
        {
            
            agent.velocity = direction * dashSpeed;
            hitbox.SetActive(true);
            yield return null;
        }

        agent.velocity = Vector3.zero;
        agent.isStopped = false;
        hitbox.SetActive(false);
        StartCoroutine(ResetDashAttackTime());

    }
    private void ResetAttack()
    {
        hitbox.GetComponent<MonsterHitbox>().ResetHitList();
        hitbox.SetActive(false);
        isAttacking = false;

        agent.isStopped = false;
    }
    private void FindHomeCore()
    {
        homeCore = GameObject.FindGameObjectWithTag("HomeCore");
        if (homeCore != null)
        {
            isHomeCoreSet = true;
        }
    }
    private void MoveToTarget(Vector3 target)
    {
        agent.SetDestination(target);
        animator.SetBool("Walk", true);
    }
    private void PatrolBehavior()
    {
        if (!walkPointSet) SearchWalkPoint();
        if (walkPointSet)
        {
            MoveToTarget(walkPoint.Value);
        }
    }
    private void AttackHomeCore()
    {
        if (isAttacking) return;

        agent.isStopped = true;

        hitbox.SetActive(true);
        Debug.Log("Attacking Core");
        animator.SetTrigger("Punch");

        isAttacking = true;

        Invoke(nameof(ResetAttack), timeBetweenAttacks);
    }
    IEnumerator ResetDashAttackTime()
    {
        yield return new WaitForSeconds(10f);
        hasPlayerBeenSpotted = false;
    }
}
