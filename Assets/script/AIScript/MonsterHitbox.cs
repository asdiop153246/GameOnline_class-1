using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;

public class MonsterHitbox : NetworkBehaviour
{
    public GameObject parentMonster;
    private List<GameObject> playersHitThisAttack = new List<GameObject>();
    private void OnTriggerEnter(Collider other)
    {
        if (IsServer && other.gameObject.CompareTag("Player") && !playersHitThisAttack.Contains(other.gameObject))
        {
            var playerHealth = other.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                float attackDamage = parentMonster.GetComponent<EnemyAi>().attackDamage;
                playerHealth.RequestTakeDamageServerRpc(attackDamage);
                playersHitThisAttack.Add(other.gameObject);
            }
        }
    }

    public void ResetHitList()
    {
        playersHitThisAttack.Clear();
    }
}
