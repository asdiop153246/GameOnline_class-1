using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SpearAttack : NetworkBehaviour
{
    public PlayerControllerScript playerController;
    public int attackDamage = 25;  
    public float attackDelay = 1f; 
    public bool isAttacking = false;
    public bool canAttack = true;
    public Animator animator;
    public EquipItems equipItems;
    private void Update()
    {
        // Only the owner can perform attacks.
        if (!IsOwner)
            return;

        if (!equipItems.Isequip)
            return;

        if (Input.GetMouseButtonDown(0) && canAttack && playerController.stamina > 20)
        {
            isAttacking = true;
            animator.SetTrigger("Stab");
            playerController.UseStamina(20);
            StartCoroutine(AttackDelay());
        }
        else
        {
            Debug.Log("You're not having spear");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isAttacking && other.gameObject.CompareTag("Enemy"))
        {
            var monster = other.gameObject.GetComponent<MonsterHP>();
            if (monster != null)
            {
                Debug.Log("Detected enemy for attack");
                InformServerOfAttackServerRpc(monster.NetworkObject.NetworkObjectId, attackDamage);
                isAttacking = false;
            }
        }
    }

    private IEnumerator AttackDelay()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackDelay);
        canAttack = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void InformServerOfAttackServerRpc(ulong monsterNetworkId, int damage)
    {
        Debug.Log("RPC called with monster ID: " + monsterNetworkId);

        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(monsterNetworkId, out var networkObject))
        {
            var monsterHp = networkObject.GetComponent<MonsterHP>();
            if (monsterHp != null)
            {
                monsterHp.RequestTakeDamageServerRpc(damage);
            }
        }
        else
        {
            Debug.LogError("Failed to find NetworkObject for monster with ID: " + monsterNetworkId);
        }
    }
}

