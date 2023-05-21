using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearAttack : MonoBehaviour
{
    public GameObject spearHitbox;
    public LayerMask enemyLayers;

    private bool isAttacking = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
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
        yield return new WaitForSeconds(0.5f);

        // End attack
        isAttacking = false;
        spearHitbox.SetActive(false); // Disable hitbox
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isAttacking) return;

        if (((1 << other.gameObject.layer) & enemyLayers) != 0)
        {
            // Assuming the enemy has a script with a function called 'TakeDamage'
            //other.GetComponent<Monster>().TakeDamage(1);
        }
    }
}

