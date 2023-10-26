using UnityEngine;
using Unity.Netcode;

public class MonsterHitbox : NetworkBehaviour
{
    public GameObject parentMonster;

    private void OnTriggerEnter(Collider other)
    {
        if (IsServer && other.gameObject.CompareTag("Player"))
        {
            var playerHealth = other.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                float attackDamage = parentMonster.GetComponent<EnemyAi>().attackDamage;
                playerHealth.RequestTakeDamageServerRpc(attackDamage);
            }
        }
    }
}
