using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] GameObject Ui;
    public NetworkVariable<float> Health = new NetworkVariable<float>(100f);
    public NetworkVariable<float> maxHealth = new NetworkVariable<float>(100f);
    public GameObject Frame;
    public Image HealthBar;
    private float lerptimer;
    public float chipSpeed = 2f;
    public Image BackHealthBar;
    //private bool isInitialized = false;

    void Start()
    {
        if (IsOwner)
        {
            Frame.SetActive(true);
            Health.Value = maxHealth.Value;
        }
        else
        {
            Destroy(Ui);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        HealthBar.fillAmount = Health.Value / maxHealth.Value;
        if (Input.GetKeyDown(KeyCode.G))
        {
            RequestTakeDamageServerRpc(10);
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            RequestRestoreHealthServerRpc(10);
        }
        UpdateHealthUI();
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
    public void RequestTakeDamageServerRpc(float damage)
    {
        TakeDamage(damage);
    }
    [ServerRpc(RequireOwnership = false)]
    public void RequestRestoreHealthServerRpc(float health)
    {
        RestoreHealth(health);
    }
    public void TakeDamage(float damage)
    {
        if (!IsServer) return;  
        Health.Value -= damage;
        Health.Value = Mathf.Max(Health.Value, 0);
        Debug.Log("Current Health: " + Health.Value);
        if (Health.Value <= 0)
        {
            Debug.Log("Player should die now");
            Die();
        }
        Debug.Log(Health.Value);
    }

    public void RestoreHealth(float health)
    {
        if (!IsServer) return;  
        Health.Value += health;
        Health.Value = Mathf.Min(Health.Value, maxHealth.Value);  
    }
    private void Die()
    {
        NotifyClientOfDeathClientRpc();
        GetComponent<PlayerSpawnScript>().Respawn();
    }
    [ClientRpc]
    private void NotifyClientOfDeathClientRpc()
    {
        
    }
}


