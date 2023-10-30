using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SpearAttack : NetworkBehaviour
{
    public PlayerControllerScript playerController;
    public EquipItems EquipItem;
    public int attackDamage = 25;  
    public float attackDelay = 1f; 
    public bool isAttacking = false;
    public bool canAttack = true;
    public Animator animator;
    private void Start()
    {
        if (!IsOwner) return;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canAttack && playerController.stamina > 20 && EquipItem.Isequip)
        {

            isAttacking = true;
            animator.SetTrigger("Stab");
            playerController.UseStamina(20); 
            StartCoroutine(AttackDelay());
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
    public void InformServerOfAttackServerRpc(ulong monsterNetworkId, int damage, ServerRpcParams rpcParams = default)
    {
        
        Debug.Log("RPC called with monster ID: " + monsterNetworkId);
        var networkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[monsterNetworkId];
        if (networkObject != null)
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
            return;
        }
    }

}
