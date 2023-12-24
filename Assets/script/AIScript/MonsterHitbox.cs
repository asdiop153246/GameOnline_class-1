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
        if (other.gameObject.CompareTag("Player") && !playersHitThisAttack.Contains(other.gameObject))
        {
            var playerHealth = other.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                Debug.Log("Detected player in hitbox");
                float attackDamage = parentMonster.GetComponent<EnemyAi>().attackDamage;
                playerHealth.RequestTakeDamageServerRpc(attackDamage, OwnerClientId);
                playersHitThisAttack.Add(other.gameObject);
            }
        }
        else if (other.gameObject.CompareTag("HomeCore"))
        {
            var CoreHealth = other.gameObject.GetComponent<HomeCoreScript>();
            if(CoreHealth != null)
            {
                Debug.Log("Successfully hit Core");
                float CoreDamage = parentMonster.GetComponent<EnemyAi>().attackDamage;
                CoreHealth.DecreaseHealthServerRpc(CoreDamage);
            }
        }
    }
    public void ResetHitList()
    {
        playersHitThisAttack.Clear();
    }
}
