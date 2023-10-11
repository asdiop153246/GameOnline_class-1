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

    public Transform currentTarget;
    private bool isWaiting = false;
    private bool isFlooding = false;

    private void Start()
    {
        //if (IsServer)
        //{
            if (target1 != null)
            {
                currentTarget = target1;
            }
            else
            {
                Debug.LogError("Target1 is not assigned.");
                return;
            }
        //}
    }

    private void Update()
    {
        if (IsServer && !isWaiting)
        {
            //MovetoTargetServerRPC();
            MoveToTarget();

            if (currentTarget != null && Vector3.Distance(transform.position, currentTarget.position) < 0.001f)
            {
                StartCoroutine(WaitAndMove(5));
            }
            if (currentTarget == target2 && Vector3.Distance(transform.position, currentTarget.position) < 0.001f)
            {
                DespawnIsland();
            }
        }
    }

    private void MoveToTarget()
    {
        if (currentTarget != null)
        {
            var step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, step);
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
        currentTarget = currentTarget == target1 ? target2 : target1;

        //if (currentTarget == target2)
        //{
        //    DespawnIsland();
        //}

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
                    monster.transform.SetParent(parentTransform);  // Set the spawned monster's parent to the parent of the current GameObject
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