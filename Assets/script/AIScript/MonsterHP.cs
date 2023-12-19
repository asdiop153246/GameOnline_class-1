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
    public float stunDuration = 1.5f;
    public bool isStunned = false;
    public GameObject damagedParticle;
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
        TakeDamageClientRpc(damage);
    }

    [ClientRpc]
    public void TakeDamageClientRpc(int damage)
    {
        if (isStunned) return; 

        currentHealth.Value -= damage;

        if (currentHealth.Value <= 0)
        {
            DieServerRpc();
        }
        else
        {
            StartCoroutine(ApplyStunEffect());
            StartCoroutine(ApplyDamagedEffect());
        }
    }
    private IEnumerator ApplyStunEffect()
    {
        isStunned = true;
        yield return new WaitForSeconds(stunDuration);
        isStunned = false;
    }
    private IEnumerator ApplyDamagedEffect()
    {
        damagedParticle.SetActive(true);
        yield return new WaitForSeconds(1);
        damagedParticle.SetActive(false);
    }
    [ServerRpc(RequireOwnership = false)]
    private void DieServerRpc()
    {
        StartCoroutine(DieAfterDelay());
        NetworkObject.Destroy(gameObject);
        DieClientRpc();
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
            DieServerRpc();  
        }
    }
    private IEnumerator DieAfterDelay()
    {
        animator.SetTrigger("Die");
        yield return new WaitForSeconds(3);        
    }
}
