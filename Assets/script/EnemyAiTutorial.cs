using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class EnemyAiTutorial : NetworkBehaviour
{
    public NavMeshAgent agent;
    public LayerMask groundMask, playerMask;
    public float patrolRange = 10f;
    public float sightRange = 20f;
    public float attackRange = 4f;
    public float timeBetweenAttacks = 1.3f;
    private bool isAttacking = false;

    private NetworkVariable<Vector3> walkPoint = new NetworkVariable<Vector3>(new Vector3());
    private bool walkPointSet = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (!IsServer) return;

        // Check if player is in sight range
        Collider[] playersInSightRange = Physics.OverlapSphere(transform.position, sightRange, playerMask);
        if (playersInSightRange.Length != 0)
        {
            float playerDistance = Vector3.Distance(transform.position, playersInSightRange[0].transform.position);

            // If player is within sight range but outside of attack range
            if (playerDistance > attackRange)
            {
                agent.SetDestination(playersInSightRange[0].transform.position);
            }
            else
            {
                // Player is within attack range, stop movement
                agent.ResetPath();

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
            }
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

        // Attack code here...
        Debug.Log("Attacking player!");

        isAttacking = true;
        Invoke(nameof(ResetAttack), timeBetweenAttacks);
    }

    private void ResetAttack()
    {
        isAttacking = false;
    }
}
