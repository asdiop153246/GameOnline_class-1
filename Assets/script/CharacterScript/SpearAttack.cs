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
    private void Start()
    {
        if (!IsOwner) return;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canAttack && playerController.stamina > 20)
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
                monster.RequestTakeDamageServerRpc(attackDamage);
            }
            isAttacking = false;
        }
    }

    private IEnumerator AttackDelay()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackDelay);
        canAttack = true;
    }





    //public GameObject spearHitbox;
    //public LayerMask enemyLayers;
    //public Animator animator;
    //private bool isAttacking = false;

    //private void Update()
    //{
    //    if (!IsOwner) return;

    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        StartCoroutine(Attack());
    //    }
    //}

    //private IEnumerator Attack()
    //{
    //    if (isAttacking)
    //        yield break;

    //    // Start attack
    //    isAttacking = true;
    //    animator.SetTrigger("Stab");
    //    spearHitbox.SetActive(true);  // Enable hitbox

    //    // Animation or delay for the attack
    //    yield return new WaitForSeconds(2f);

    //    // End attack
    //    isAttacking = false;
    //    spearHitbox.SetActive(false); // Disable hitbox
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (!isAttacking)
    //        return;

    //    MonsterHP monsterHP = other.GetComponent<MonsterHP>();
    //    Debug.Log("Hitbox hit Enemy");
    //    if (monsterHP != null)
    //    {
    //        monsterHP.RequestTakeDamageServerRpc(25);
    //    }
    //}
}
