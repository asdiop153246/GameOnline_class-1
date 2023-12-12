using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class Movetoward : NetworkBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private Transform target1;
    [SerializeField] private Transform target2;
    [SerializeField] private GameObject monsterPrefab;
    [SerializeField] private Transform[] monsterSpawnPoints;
    private Vector3 target1Position;
    private Vector3 target2Position;
    private float stoppingDistance;

    public Transform currentTarget;
    private bool isWaiting = false;
    //private bool isFlooding = false;

    private void Start()
    {
        //if (IsServer)
        //{
        if (target1 != null && target2 != null)
        {
            target1Position = target1.position;  // Store global coordinates
            target2Position = target2.position;  // Store global coordinates
            currentTarget = target1;
        }
        else
        {
            Debug.LogError("Targets are not assigned.");
            return;
        }
        //}
    }

    private void Update()
    {
        if (IsServer && !isWaiting)
        {
            MoveToTarget();

            Vector3 targetPosition = currentTarget == target1 ? target1Position : target2Position;

            // Check if the island is close to the target position
            if (Vector3.Distance(transform.position, targetPosition) < 1f) 
            {
                StartCoroutine(WaitAndMove(300));
            }

            // If it is close to target2, despawn the island
            if (currentTarget == target2 && Vector3.Distance(transform.position, target2Position) < 1f) 
            {
                DespawnIsland();
            }
        }
    }

    private void MoveToTarget()
    {
        if (currentTarget != null)
        {
            Vector3 targetPosition = currentTarget == target1 ? target1Position : target2Position; 
            var step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
        }
        else
        {
            Debug.LogError("Current target is not assigned.");
        }
    }

    IEnumerator WaitAndMove(float seconds)
    {
        isWaiting = true;
        SpawnMonsters();

        yield return new WaitForSeconds(seconds);

        isWaiting = false;
        SwitchTarget();
    }

    private void SwitchTarget()
    {
        if (currentTarget == target1)
        {
            currentTarget = target2;
            Debug.Log("Switched to Target 2");
        }
        else
        {
            currentTarget = target1;
            Debug.Log("Switched to Target 1");
        }

        if (currentTarget == null)
        {
            Debug.LogError("The new current target is not assigned.");
        }
    }

    private void DespawnIsland()
    {
        if (GetComponent<NetworkObject>())
        {
            GetComponent<NetworkObject>().Despawn();
            Debug.Log("Island despawned");
        }
        else
        {
            Debug.LogError("No NetworkObject component found on the island.");
        }
    }

    private void SpawnMonsters()
    {
        if (monsterSpawnPoints.Length > 0 && monsterPrefab != null)
        {
            Transform parentTransform = transform.parent;  // Get the parent of the current GameObject

            if (parentTransform != null)
            {
                Debug.Log("Parent detected: " + parentTransform.name);
            }
            else
            {
                Debug.Log("No parent detected.");
            }

            foreach (var spawnPoint in monsterSpawnPoints)
            {
                var monster = Instantiate(monsterPrefab, spawnPoint.position, spawnPoint.rotation);
                monster.GetComponent<NetworkObject>().Spawn();

                if (parentTransform != null)  // Check if a parent exists
                {
                    monster.transform.SetParent(parentTransform);  
                    Debug.Log("Monster parent set to: " + monster.transform.parent.name);
                }
            }
        }
        else
        {
            Debug.LogError("Monster Spawn Points or Monster Prefab is not assigned.");
        }
    }
}