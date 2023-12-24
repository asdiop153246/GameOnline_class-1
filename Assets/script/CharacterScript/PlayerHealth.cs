using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] GameObject Ui;
    public NetworkVariable<float> Health = new NetworkVariable<float>(100f);
    public NetworkVariable<float> maxHealth = new NetworkVariable<float>(100f);
    public NetworkVariable<float> respawnHealth = new NetworkVariable<float>(50f);
    public GameObject Frame;
    public Image HealthBar;
    private HungerThirstScript HungerThirst;
    private Coroutine damageCoroutine;
    private bool isDamaged;
    private float lerptimer;
    private bool canRegen;
    public float chipSpeed = 2f;
    public Image BackHealthBar;    

    void Start()
    {
        if (IsOwner)
        {
            //Frame.SetActive(true);
            Health.Value = maxHealth.Value;
            HungerThirst = GetComponent<HungerThirstScript>();
        }
        else
        {
            //Destroy(Ui);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        HealthBar.fillAmount = Health.Value / maxHealth.Value;
        CanRegenHealth();
        if (CanRegenHealth() == true && isDamaged == false)
        {
            RequestRestoreHealthServerRpc(0.05f);
        }
        UpdateHealthUI();
    }

    public bool CanRegenHealth()
    {
        return HungerThirst.hunger.Value >= 70 && HungerThirst.thirst.Value >= 50;        
    }

    public void UpdateHealthUI()
    {
        float fillF = HealthBar.fillAmount;
        float fillB = BackHealthBar.fillAmount;
        float hFraction = Health.Value / maxHealth.Value;
        if (fillB > hFraction)
        {
            HealthBar.fillAmount = hFraction;
            BackHealthBar.color = Color.white;
            lerptimer += Time.deltaTime;
            float percentComplete = lerptimer / chipSpeed;
            BackHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }
        if (fillF < hFraction)
        {
            HealthBar.fillAmount = hFraction;
            BackHealthBar.color = Color.white;
            lerptimer += Time.deltaTime;
            float percentComplete = lerptimer / chipSpeed;
            BackHealthBar.fillAmount = Mathf.Lerp(fillF, hFraction, percentComplete);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestTakeDamageServerRpc(float damage, ulong clientId)
    {
        Health.Value -= damage;
        Health.Value = Mathf.Max(Health.Value, 0);

        Debug.Log("Current Health: " + Health.Value);
        if (Health.Value <= 0)
        {
            Debug.Log("Player should die now");
            DieServerRpc();
        }

        // Stop the existing coroutine if it is running
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
        }

        // Start a new coroutine
        damageCoroutine = StartCoroutine(DelayBeforeCanRegen());
    }
    IEnumerator DelayBeforeCanRegen()
    {
        isDamaged = true;
        yield return new WaitForSeconds(10f);
        isDamaged = false;
    }
    [ServerRpc(RequireOwnership = false)]
    public void RequestRestoreHealthServerRpc(float health)
    {
        Health.Value += health;
        Health.Value = Mathf.Min(Health.Value, maxHealth.Value);
        //RestoreHealth(health);
    }

    [ServerRpc]
    private void DieServerRpc()
    {
        GetComponent<PlayerSpawnScript>().Respawn();
        NotifyClientOfDeathClientRpc();        
    }
    [ClientRpc]
    private void NotifyClientOfDeathClientRpc()
    {
        Debug.Log("You're dead");
    }
}


