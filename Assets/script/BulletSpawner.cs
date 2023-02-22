using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BulletSpawner : NetworkBehaviour
{
    public GameObject bulletPrefab;
    private List<GameObject> spawnedBullet = new List<GameObject>();

    void Update()
    {
        if (!IsOwner) return;
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpawnBulletServerRpc();
            }
        }
    }

    [ServerRpc]
    void SpawnBulletServerRpc()
    {
        Vector3 spawnPos = transform.position + (transform.forward * -1.5f) + (transform.up * 0.8f);
        Quaternion spawnRot = transform.rotation;

        GameObject bullet = Instantiate(bulletPrefab, spawnPos, spawnRot);

        //Add to list
        spawnedBullet.Add(bullet);
        bullet.GetComponent<BulletScript>().bulletSpawner = this;

        bullet.GetComponent<NetworkObject>().Spawn();
    }

    [ServerRpc (RequireOwnership = false)]
    public void DestroyServerRpc(ulong networkObjectID)
    {
        GameObject toDestroy = findBulletFromNetworkID(networkObjectID);
        if (toDestroy = null)
        {
            return;
        }
        toDestroy.GetComponent<NetworkObject>().Despawn();
        spawnedBullet.Remove(toDestroy);
        Destroy(toDestroy);
    }

    private GameObject findBulletFromNetworkID(ulong networkObjectID)
    {
        foreach (GameObject bullet in spawnedBullet)
        {
            ulong bulletId = bullet.GetComponent<NetworkObject>().NetworkObjectId;
            if (bulletId == networkObjectID)
            {
                return bullet;
            }
        }
        return null;
    }
}


