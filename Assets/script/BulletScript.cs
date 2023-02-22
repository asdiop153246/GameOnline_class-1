using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BulletScript : NetworkBehaviour
{
    public BulletSpawner bulletSpawner;
    public GameObject bombEffectPrefab;

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsOwner) return;
        if (collision.gameObject.tag == "Player")
        {
            ulong networkObjectID = GetComponent<NetworkObject>().NetworkObjectId;
            SpawnBombEffect();
            bulletSpawner.DestroyServerRpc(networkObjectID);
        }
    }
    private void SpawnBombEffect()
    {
        GameObject bombEffect = Instantiate(bombEffectPrefab, transform.position, Quaternion.identity);
        bombEffect.GetComponent<NetworkObject>().Spawn();
    }
}