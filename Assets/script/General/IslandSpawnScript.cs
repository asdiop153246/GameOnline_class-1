using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;
public class IslandSpawnScript : NetworkBehaviour
{
    [SerializeField] private GameObject[] islandPrefabs; // Assign your island prefabs in the inspector
    [SerializeField] private Vector3 spawnPosition1; // Set the specific spawn position in the inspector
    [SerializeField] private Vector3 spawnPosition2;
    [SerializeField] private Vector3 spawnPosition3;

    // Ensure this is called only on the server
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            SpawnIsland();
        }
    }

    public void SpawnIsland()
    {
        // Ensure network is ready
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening)
        {
            Debug.LogError("Network is not ready.");
            return;
        }

        int randomIndex = 0;//Random.Range(0, islandPrefabs.Length);
        Debug.Log("Currentlly island = "+ randomIndex);
        
        if (randomIndex == 0)
        {
            var island = Instantiate(islandPrefabs[randomIndex], spawnPosition1, Quaternion.identity);
            island.GetComponent<NetworkObject>().Spawn();

            var navMeshSurface = island.GetComponent<NavMeshSurface>();
            if (navMeshSurface != null)
            {
                navMeshSurface.BuildNavMesh();
                Debug.Log("NavMesh built at runtime.");
            }
            else
            {
                Debug.LogWarning("No NavMeshSurface component found on the island prefab.");
            }
        }
        else if (randomIndex == 1)
        {
            var island = Instantiate(islandPrefabs[randomIndex], spawnPosition2, Quaternion.identity);
            island.GetComponent<NetworkObject>().Spawn();
            island.transform.Rotate(Vector3.up, 182.15f);

            var navMeshSurface = island.GetComponent<NavMeshSurface>();
            if (navMeshSurface != null)
            {
                navMeshSurface.BuildNavMesh();
                Debug.Log("NavMesh built at runtime.");
            }
            else
            {
                Debug.LogWarning("No NavMeshSurface component found on the island prefab.");
            }
        }
        else if (randomIndex == 2)
        {
            var island = Instantiate(islandPrefabs[randomIndex], spawnPosition3, Quaternion.identity);
            island.GetComponent<NetworkObject>().Spawn();

            var navMeshSurface = island.GetComponent<NavMeshSurface>();
            if (navMeshSurface != null)
            {
                navMeshSurface.BuildNavMesh();
                Debug.Log("NavMesh built at runtime.");
            }
            else
            {
                Debug.LogWarning("No NavMeshSurface component found on the island prefab.");
            }
        }

    }
}
