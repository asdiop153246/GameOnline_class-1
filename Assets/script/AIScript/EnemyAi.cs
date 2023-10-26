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
    public float attackRange = 3f;
    public float timeBetweenAttacks = 3f;
    private bool isAttacking = false;
    public GameObject hitbox;
    private NetworkVariable<Vector3> walkPoint = new NetworkVariable<Vector3>(new Vector3());
    private bool walkPointSet = false;
    public float attackDamage = 15f;
    private NetworkVariable<Vector3> position = new NetworkVariable<Vector3>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<Quaternion> rotation = new NetworkVariable<Quaternion>(new Quaternion());

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!IsServer) return;
        animator.SetBool("Walk", false);
        // Check if player is in sight range
        Collider[] playersInSightRange = Physics.OverlapSphere(transform.position, sightRange, playerMask);
        if (playersInSightRange.Length != 0)
        {
            float playerDistance = Vector3.Distance(transform.position, playersInSightRange[0].transform.position);

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
            if (!walkPointSet) SearchWalkPoint();
            if (walkPointSet)
            {
                agent.SetDestination(walkPoint.Value);
                animator.SetBool("Walk", true);
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

    private void SearchWalkPoint()
    {
        // Calculate random point in range
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

        hitbox.SetActive(true);
        Debug.Log("Attacking player!");
        animator.SetTrigger("Punch");

        isAttacking = true;
        Invoke(nameof(ResetAttack), timeBetweenAttacks);
    }

    private void ResetAttack()
    {
        hitbox.SetActive(false);
        isAttacking = false;
    }
}
