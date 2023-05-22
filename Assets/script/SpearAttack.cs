using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class SpearAttack : NetworkBehaviour
{
    public GameObject spearHitbox;
    public LayerMask enemyLayers;

    private bool isAttacking = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        // Start attack
        isAttacking = true;
        spearHitbox.SetActive(true);  // Enable hitbox

        // Animation or delay for the attack
        yield return new WaitForSeconds(1.5f);

        // End attack
        isAttacking = false;
        spearHitbox.SetActive(false); // Disable hitbox
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isAttacking) return;

        if (((1 << other.gameObject.layer) & enemyLayers) != 0)
        {
            // Assuming the enemy has a PlayerHealth script
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.RequestTakeDamageServerRpc(40); // Deal 1 damage to the player
            }
        }
    }
}

