using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
public class HungerThirstScript : NetworkBehaviour
{
    [Header("Values")]
    [Range(0, 100)] public float startingHunger = 100f;
    [Range(0, 100)] public float startingThirst = 100f;

    [Header("Rate of Decrease")]
    public float hungerDecreaseRate = 0.25f;
    public float thirstDecreaseRate = 0.10f;

    [Header("UI Components")]
    public Image HungerBar;
    public Image BackHungerBar;
    public Image thirstBar;
    public Image BackThirstBar;

    private float lerptimer;
    public float chipSpeed = 2f;
    public NetworkVariable<float> hunger = new NetworkVariable<float>(default,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<float> thirst = new NetworkVariable<float>(default,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    
    private void Start()
    {      
        if (IsServer)
        {
            hunger.Value = startingHunger;
            thirst.Value = startingThirst;
        }
        if (!IsOwner) return;

        hunger.OnValueChanged += (oldValue, newValue) => UpdateHungerUI();
        thirst.OnValueChanged += (oldValue, newValue) => UpdateThirstUI();
    }

    private void Update()
    {
        
        if (IsServer)
        {
            DecreaseHunger(Time.deltaTime * hungerDecreaseRate);
            DecreaseThirst(Time.deltaTime * thirstDecreaseRate);
        }
        HungerBar.fillAmount = hunger.Value / startingHunger;
        thirstBar.fillAmount = thirst.Value / startingThirst;
        UpdateHungerUI();
        UpdateThirstUI();
    }


    public void DecreaseHunger(float amount)
    {
        if (IsServer)
        {
            hunger.Value -= amount;
            hunger.Value = Mathf.Clamp(hunger.Value, 0, 100);

            if (hunger.Value <= 0)
            {
                Debug.Log("Player is starving!");
            }
        }
    }

    public void DecreaseThirst(float amount)
    {
        if (IsServer)
        {
            thirst.Value -= amount;
            thirst.Value = Mathf.Clamp(thirst.Value, 0, 100);

            if (thirst.Value <= 0)
            {
                Debug.Log("Player is dehydrated!");
            }
        }
    }

    public void IncreaseHunger(float amount)
    {
        if (IsServer)
        {
            hunger.Value = Mathf.Clamp(hunger.Value + amount, 0, startingHunger);
        }
    }

    public void IncreaseThirst(float amount)
    {
        if (IsServer)
        {
            thirst.Value = Mathf.Clamp(thirst.Value + amount, 0, startingThirst);
        }
    }
    public void UpdateHungerUI()
    {
        float fillF = HungerBar.fillAmount;
        float fillB = BackHungerBar.fillAmount;
        float hFraction = hunger.Value / startingHunger;
        if (fillB > hFraction)
        {
            HungerBar.fillAmount = hFraction;
            BackHungerBar.color = Color.white;
            lerptimer += Time.deltaTime;
            float percentComplete = lerptimer / chipSpeed;
            BackHungerBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }
        if (fillF < hFraction)
        {
            HungerBar.fillAmount = hFraction;
            BackHungerBar.color = Color.white;
            lerptimer += Time.deltaTime;
            float percentComplete = lerptimer / chipSpeed;
            BackHungerBar.fillAmount = Mathf.Lerp(fillF, hFraction, percentComplete);
        }
    }

    public void UpdateThirstUI()
    {
        float fillF = thirstBar.fillAmount;
        float fillB = BackThirstBar.fillAmount;
        float hFraction = thirst.Value / startingThirst;
        if (fillB > hFraction)
        {
            thirstBar.fillAmount = hFraction;
            BackThirstBar.color = Color.white;
            lerptimer += Time.deltaTime;
            float percentComplete = lerptimer / chipSpeed;
            BackThirstBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }
        if (fillF < hFraction)
        {
            thirstBar.fillAmount = hFraction;
            BackThirstBar.color = Color.white;
            lerptimer += Time.deltaTime;
            float percentComplete = lerptimer / chipSpeed;
            BackThirstBar.fillAmount = Mathf.Lerp(fillF, hFraction, percentComplete);
        }
    }
}
