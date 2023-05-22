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
    private bool isInitialized = false;

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
    }
    public void UpdateHealthUI()
    {
        float fillF = HealthBar.fillAmount;
        float fillB = BackHealthBar.fillAmount;
        float hFraction = Health.Value / maxHealth.Value;
        if (fillB > hFraction)
        {
            HealthBar.fillAmount = hFraction;
            BackHealthBar.color = Color.red;
            lerptimer += Time.deltaTime;
            float percentComplete = lerptimer / chipSpeed;
            BackHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }
        if (fillF < hFraction)
        {
            HealthBar.fillAmount = hFraction;
            BackHealthBar.color = Color.green;
            lerptimer += Time.deltaTime;
            float percentComplete = lerptimer / chipSpeed;
            BackHealthBar.fillAmount = Mathf.Lerp(fillF, hFraction, percentComplete);
        }
    }

    [ServerRpc]
    public void RequestTakeDamageServerRpc(float damage)
    {
        TakeDamage(damage);
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
        Health.Value = Mathf.Min(Health.Value, maxHealth.Value);  // Prevent health from going above max
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
//    if (fillB > hFraction)
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

