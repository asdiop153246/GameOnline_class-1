using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;
public class IslandSpawnScript : NetworkBehaviour
{
    [SerializeField] private GameObject[] islandPrefabs; 
    [SerializeField] private Vector3 spawnPosition1; 
    [SerializeField] private Vector3 spawnPosition2;
    [SerializeField] private Vector3 spawnPosition3;
    [SerializeField] private Vector3 spawnPosition4;
    [SerializeField] private Vector3 spawnPosition5;
    [SerializeField] private Vector3 spawnPosition6;

    public GameObject OtherIsland;
    public bool isSpawned = false;
    [Header("Item Spawning")]
    [SerializeField] private GameObject[] itemPrefabs;
    [SerializeField] private GameObject woodPrefabs;
    [SerializeField] private float[] itemSpawnChances;  
    [SerializeField] private float noSpawnChance = 0.3f;  


    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            SpawnIsland();
        }
    }

    private void Update()
    {
        OtherIsland = GameObject.FindWithTag("OtherIsland");        
    }
    public void SpawnIsland()
    {        
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening || isSpawned == true)
        {
            Debug.LogError("Network is not ready.");
            return;
        }
        int randomIndex = Random.Range(1, islandPrefabs.Length);
        Debug.Log("Currentlly island = "+ randomIndex);
        GameObject island = null;
        if (randomIndex == 0)
        {
            island = Instantiate(islandPrefabs[randomIndex], spawnPosition1, Quaternion.identity);
                    
            island.GetComponent<NetworkObject>().Spawn();
            
            SpawnRandomItemsAround(island);
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
        else if (randomIndex == 1) //normal
        {
            island = Instantiate(islandPrefabs[randomIndex], spawnPosition2, Quaternion.identity);
            
            island.GetComponent<NetworkObject>().Spawn();
            
            island.transform.Rotate(Vector3.up, 182.15f);
            SpawnRandomItemsAround(island);
            SpawnWoodAround(island);
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
        else if (randomIndex == 2) //Tower
        {
            island = Instantiate(islandPrefabs[randomIndex], spawnPosition3, Quaternion.identity);
           
            island.GetComponent<NetworkObject>().Spawn();
            
            SpawnRandomItemsAround(island);
            SpawnWoodAround(island);
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
        else if (randomIndex == 3) //Temple
        {
            island = Instantiate(islandPrefabs[randomIndex], spawnPosition4, Quaternion.identity);
            
            island.GetComponent<NetworkObject>().Spawn();
            
            SpawnRandomItemsAround(island);
            SpawnWoodAround(island);
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
        else if (randomIndex == 4) //House2
        {
            island = Instantiate(islandPrefabs[randomIndex], spawnPosition5, Quaternion.identity);
            
            island.GetComponent<NetworkObject>().Spawn();
            
            SpawnRandomItemsAround(island);
            SpawnWoodAround(island);
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
        else if (randomIndex == 5) //Mart
        {
            island = Instantiate(islandPrefabs[randomIndex], spawnPosition6, Quaternion.identity);
            
            island.GetComponent<NetworkObject>().Spawn();
            
            SpawnRandomItemsAround(island);
            SpawnWoodAround(island);
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
        if(OtherIsland != null)
        {
            isSpawned = true;
        }
        else
        {
            isSpawned = false;
        }

    }
    private void SpawnRandomItemsAround(GameObject island)
    {
        Transform[] itemSpawnPoints = island.GetComponentsInChildren<Transform>(true).Where(t => t.name.StartsWith("ItemSpawnPoint")).ToArray();

        foreach (Transform spawnPoint in itemSpawnPoints)
        {
            float randomChance = Random.Range(0f, 1f);

            if (randomChance < noSpawnChance)
            {
                continue;
            }

            for (int j = 0; j < itemPrefabs.Length; j++)
            {
                if (randomChance < itemSpawnChances[j] + noSpawnChance)
                {
                    var item = Instantiate(itemPrefabs[j], spawnPoint.position, Quaternion.identity);
                    item.GetComponent<NetworkObject>().Spawn();
                 
                    item.transform.SetParent(island.transform);

                    break;
                }
                randomChance -= itemSpawnChances[j];
            }
        }
    }
    private void SpawnWoodAround(GameObject island)
    {
        Transform[] WoodSpawnPoints = island.GetComponentsInChildren<Transform>(true).Where(t => t.name.StartsWith("WoodSpawnPoint")).ToArray();
        foreach (Transform spawnPoint in WoodSpawnPoints)
        {
            float randomChance = Random.Range(0f, 1f);
            Debug.Log($"The value chance for Wood to spawn is {randomChance}");
            // If randomChance is greater than 0.5, spawn wood.
            if (randomChance > 0.5f)
            {
                var item = Instantiate(woodPrefabs, spawnPoint.position, Quaternion.identity);
                item.GetComponent<NetworkObject>().Spawn();
                item.transform.SetParent(island.transform);
            }
        }
    }
}