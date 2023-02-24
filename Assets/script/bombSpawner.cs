using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class bombSpawner : NetworkBehaviour
{
    public GameObject BombPrefab;
    private List<GameObject> spawnedBomb = new List<GameObject>();

    void Update()
    {
        if (!IsOwner) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnBombServerRpc();
        }
    }
    [ServerRpc]
    void SpawnBombServerRpc()
    {
        Vector3 spawnPos = transform.position + (transform.forward * -1.5f) + (transform.up * 0.8f);
        Quaternion spawnRot = transform.rotation;

        GameObject Bomb = Instantiate(BombPrefab, spawnPos, spawnRot);
        spawnedBomb.Add(Bomb);
        Bomb.GetComponent<bombScript>().bombSpawner = this;
        Bomb.GetComponent<NetworkObject>().Spawn();
    }

    [ServerRpc (RequireOwnership = false)]
    public void DestroyServerRpc(ulong NetworkObjectId)
    {
        GameObject toDestroy = FindBombFromNetworkId(NetworkObjectId);
        if (toDestroy == null)
        {
            return;
        }
        toDestroy.GetComponent<NetworkObject>().Despawn();
        spawnedBomb.Remove(toDestroy);
        Destroy(toDestroy);
    }

    private GameObject FindBombFromNetworkId(ulong networkObjectId)
    {
        foreach(GameObject bomb in spawnedBomb)
        {
            ulong bombId = bomb.GetComponent<NetworkObject>().NetworkObjectId;
            if(bombId == networkObjectId) 
            { 
                return bomb; 
            }
        }
        return null;
    }
}