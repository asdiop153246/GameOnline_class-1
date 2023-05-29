using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MonsterHP : NetworkBehaviour
{
    public int maxHealth = 100;
    public NetworkVariable<int> currentHealth = new NetworkVariable<int>
        (100,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);

    private void Start()
    {
        currentHealth.Value = maxHealth;
    }

    [ServerRpc]
    public void RequestTakeDamageServerRpc(int damage, ServerRpcParams rpcParams = default)
    {
        if (!IsServer)
            return;

        TakeDamage(damage);
    }

    private void TakeDamage(int damage)
    {
        if (!IsServer)
            return;

        currentHealth.Value -= damage;

        if (currentHealth.Value <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Perform death logic
        // For example, destroy the monster object
        NetworkObject.Destroy(gameObject);
    }
}
