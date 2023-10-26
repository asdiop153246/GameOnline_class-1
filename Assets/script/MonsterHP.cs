using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Animator))]
public class MonsterHP : NetworkBehaviour
{
    public int maxHealth = 100;
    private Animator animator;
    public NetworkVariable<int> currentHealth = new NetworkVariable<int>
        (100,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);

    private void Start()
    {
        currentHealth.Value = maxHealth;
        animator = GetComponent<Animator>();
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
            animator.SetTrigger("Die");
            Die();
        }
    }

    private void Die()
    {
        // Notify the clients that this monster is dead
        DieClientRpc();

        // Perform death logic on the server
        // For example, destroy the monster object
        if (IsServer)
        {
            NetworkObject.Destroy(gameObject);
        }
    }

    // Notify clients of the monster's death
    [ClientRpc]
    private void DieClientRpc()
    {
        // Clients perform their own death logic here, for example:
        gameObject.SetActive(false);
    }
}
