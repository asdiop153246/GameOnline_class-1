using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
public class PlayerHealth : NetworkBehaviour
{
    public NetworkVariable<float> Health = new NetworkVariable<float>(100f);
    public float maxHealth = 100f;
    public GameObject Frame;
    public Image HealthBar;
    public Image BackHealthBar;
    private bool isInitialized = false;

    void Start()
    {
        if (!IsOwner) return;
        Frame = GameObject.FindGameObjectWithTag("HP");
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInitialized && IsOwner)
        {
            GameObject healthBarObject = GameObject.FindGameObjectWithTag("FrontHP");
            GameObject BackBarObject = GameObject.FindGameObjectWithTag("BackHP");
            if (healthBarObject != null && BackBarObject != null)
            {
                HealthBar = healthBarObject.GetComponent<Image>();
                BackHealthBar = BackBarObject.GetComponent<Image>();
                Frame.SetActive(true);
                isInitialized = true;
            }
        }

        if (HealthBar != null && IsOwner)
        {
            HealthBar.fillAmount = Health.Value / maxHealth;
        }
    }

    public void TakeDamage(float damage)
    {
        if (!IsServer) return;  // Only the server can damage the player
        Health.Value -= damage;
        Health.Value = Mathf.Max(Health.Value, 0);  // Prevent health from going below 0
    }

    public void RestoreHealth(float amount)
    {
        if (!IsServer) return;  // Only the server can heal the player
        Health.Value += amount;
        Health.Value = Mathf.Min(Health.Value, maxHealth);  // Prevent health from going above max
    }
}
    
    
    
    
    
    //private float health;
    //private float lerptimer;
    //public float maxHealth = 100f;
    //public float chipSpeed = 2f;
    //public Image frontHealthBar;
    //public Image backHealthBar;
    //public GameObject HPBar;
    //private bool hasInitialized = false;
    //void Start()
    //{
    //    HPBar = GameObject.FindGameObjectWithTag("HP");
    //    GameObject FrontHP = GameObject.FindGameObjectWithTag("FrontHP");
    //    GameObject BackHP = GameObject.FindGameObjectWithTag("BackHP");
    //    frontHealthBar = FrontHP.GetComponent<Image>();
    //    backHealthBar = BackHP.GetComponent<Image>();
    //    if (!IsOwner) return;
    //    HPBar.SetActive(true);
    //    health = maxHealth;
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if (!hasInitialized && IsOwner)
    //    {
    //        HPBar = GameObject.FindGameObjectWithTag("HP");
    //        GameObject FrontHP = GameObject.FindGameObjectWithTag("FrontHP");
    //        GameObject BackHP = GameObject.FindGameObjectWithTag("BackHP");

    //        if (HPBar != null && FrontHP != null && BackHP != null)
    //        {
    //            frontHealthBar = FrontHP.GetComponent<Image>();
    //            backHealthBar = BackHP.GetComponent<Image>();
    //            HPBar.SetActive(true);
    //            hasInitialized = true;
    //        }

    //        // Initialize health at the first time
    //        health = maxHealth;
    //    }
    //    health = Mathf.Clamp(health, 0, maxHealth);
    //    UpdateHealthUI();
    //    if (Input.GetKeyDown(KeyCode.G))
    //    {
    //        TakeDamage(Random.Range(5, 10));
    //    }
    //    if (Input.GetKeyDown(KeyCode.H))
    //    {
    //        RestoreHealth(Random.Range(5, 10));
    //    }
    //}

    //public void UpdateHealthUI()
    //{
    //    float fillF = frontHealthBar.fillAmount;
    //    float fillB = backHealthBar.fillAmount;
    //    float hFraction = health / maxHealth;
    //    if(fillB > hFraction)
    //    {
    //        frontHealthBar.fillAmount = hFraction;
    //        backHealthBar.color = Color.red;
    //        lerptimer += Time.deltaTime;
    //        float percentComplete = lerptimer / chipSpeed;
    //        backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
    //    }
    //    if (fillF < hFraction)
    //    {
    //        frontHealthBar.fillAmount = hFraction;
    //        backHealthBar.color = Color.green;
    //        lerptimer += Time.deltaTime;
    //        float percentComplete = lerptimer / chipSpeed;
    //        backHealthBar.fillAmount = Mathf.Lerp(fillF, hFraction, percentComplete);
    //    }
    //}
    //public void TakeDamage(float damage)
    //{
    //    health -= damage;
    //    lerptimer = 0f;

    //}
    //public void RestoreHealth(float healamount)
    //{
    //    health += healamount;
    //    lerptimer = 0f;
    //}

