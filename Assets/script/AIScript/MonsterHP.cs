using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Animator))]
public class MonsterHP : NetworkBehaviour
{
    public int maxHealth = 100;
    private Animator animator;
    public NetworkVariable<int> currentHealth = new NetworkVariable<int>();

    private void Start()
    {
        Debug.Log("MonsterHP object owned by: " + NetworkObject.OwnerClientId);
        currentHealth.Value = maxHealth;
        animator = GetComponent<Animator>();
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestTakeDamageServerRpc(int damage, ServerRpcParams rpcParams = default)
    {
        
        Debug.Log("Request to damage monster received");
        TakeDamage(damage);
    }

    public void TakeDamage(int damage)
    {
        //if (!IsServer)
        //    return;

        currentHealth.Value -= damage;

        if (currentHealth.Value <= 0)
        {
            animator.SetTrigger("Die");
            Die();
        }
    }

    private void Die()
    {
        DieClientRpc();
        if (IsServer)
        {
            NetworkObject.Destroy(gameObject);
        }
    }

    [ClientRpc]
    private void DieClientRpc()
    {
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer)
            return;

        if (other.gameObject.CompareTag("DeadZone"))
        {
            Debug.Log("monster hit deadzone");
            Die();  
        }
    }
}
