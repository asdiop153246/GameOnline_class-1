using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Movetoward : NetworkBehaviour
{
    [SerializeField] private float speed = 10.0f;
    [SerializeField] private Transform target1;
    [SerializeField] private Transform target2;
    [SerializeField] private GameObject monsterPrefab;
    [SerializeField] private Transform[] monsterSpawnPoints;
    private List<NetworkObject> spawnedMonsters = new List<NetworkObject>();
    public GameObject OtherCore;
    public OtherCoreScript OtherCoreScript;
    private Vector3 target1Position;
    private Vector3 target2Position;
    private float stoppingDistance;
    public NetworkedDayNightCycle DayTime;
    public Transform currentTarget;
    private bool isWaiting = false;
    //private bool isFlooding = false;

    private void Start()
    {
        //if (IsServer)
        //{
        if (target1 != null && target2 != null)
        {
            DayTime = GameObject.FindWithTag("DayNightCycle").GetComponent<NetworkedDayNightCycle>();
            OtherCoreScript = GameObject.FindWithTag("OtherCoreManager").GetComponent<OtherCoreScript>();           
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
        if (IsServer)
        {
            if (!isWaiting)
            {
                MoveToTarget();

                Vector3 targetPosition = currentTarget == target1 ? target1Position : target2Position;
                // Check if the island is close to the target position
                if (Vector3.Distance(transform.position, targetPosition) < 1f)
                {

                    Debug.Log($"The island will stay for {OtherCoreScript.Energy.Value} Seconds");
                    StartCoroutine(WaitAndMove(OtherCoreScript.Energy.Value));
                }
                // If it is close to target2, despawn the island
                if (currentTarget == target2 && Vector3.Distance(transform.position, target2Position) < 1f)
                {
                    DespawnIslandServerRpc();

                }
            }
            if (DayTime.IsNightTime() == true)
            {
                isWaiting = false;
                currentTarget = target2;
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

    IEnumerator WaitAndMove(float initialWaitTime)
    {
        isWaiting = true;
        SpawnMonsters();
        float elapsedTime = 0;
        float remainingTime = initialWaitTime;

        while (elapsedTime < initialWaitTime)
        {
            float currentWaitTime = OtherCoreScript.Energy.Value / 2f;
            if (currentWaitTime != remainingTime)
            {
                remainingTime = currentWaitTime;
                initialWaitTime = elapsedTime + remainingTime;
            }

            yield return new WaitForSeconds(1.0f);
            elapsedTime += 1.0f;
        }

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
            DespawnIslandServerRpc();
        }

        if (currentTarget == null)
        {
            Debug.LogError("The new current target is not assigned.");
        }
    }
    [ServerRpc]
    private void DespawnIslandServerRpc()
    {
        DespawnAllMonsters();
        if (GetComponent<NetworkObject>())
        {
            DespawnNetworkObjectAndChildren(GetComponent<NetworkObject>());
            Destroy(gameObject);
            Debug.Log("Island and its resources despawned");
        }
        else
        {
            Debug.LogError("No NetworkObject component found on the island.");
        }
    }

    private void DespawnNetworkObjectAndChildren(NetworkObject networkObject)
    {
        // Despawn all child NetworkObjects first
        foreach (Transform child in networkObject.transform)
        {
            NetworkObject childNetworkObject = child.GetComponent<NetworkObject>();
            if (childNetworkObject != null)
            {
                DespawnNetworkObjectAndChildren(childNetworkObject);
                Destroy(childNetworkObject);
            }
        }

        // Despawn this NetworkObject
        networkObject.Despawn();
    }
    private void SpawnMonsters()
    {
        if (monsterSpawnPoints.Length > 0 && monsterPrefab != null)
        {
            foreach (var spawnPoint in monsterSpawnPoints)
            {
                var monster = Instantiate(monsterPrefab, spawnPoint.position, spawnPoint.rotation);
                var networkObject = monster.GetComponent<NetworkObject>();
                if (networkObject != null)
                {
                    networkObject.Spawn();
                    spawnedMonsters.Add(networkObject); // Add the monster to the list
                }
                else
                {
                    Debug.LogError("Monster prefab does not have a NetworkObject component.");
                }
            }
        }
        else
        {
            Debug.LogError("Monster Spawn Points or Monster Prefab is not assigned.");
        }
    }
    private void DespawnAllMonsters()
    {
        foreach (var monster in spawnedMonsters)
        {
            if (monster != null && monster.IsSpawned)
            {
                monster.Despawn();
            }
        }
        spawnedMonsters.Clear(); 
    }

}