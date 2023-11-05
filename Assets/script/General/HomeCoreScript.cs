using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
public class HomeCoreScript : NetworkBehaviour
{
    public GameObject HomeCoreUI;
    public PlayerControllerScript playerMovement;
    public MoveCamera cameraControl;

    [Header("Values")]
    [Range(0, 500)] public float startingHealth = 500f;
    [Range(0, 500)] public float startingEnergy = 500f;

    [Header("Rate of Decrease")]
    public float HealthDecreaseRate = 0f;
    public float EnergyDecreaseRate = 0.80f;

    [Header("UI Components")]
    public Image HealthBar;
    public Image BackHealthBar;
    public Image EnergyBar;
    public Image BackEnergyBar;

    private float lerptimer;
    public float chipSpeed = 2f;
    public NetworkVariable<float> Health = new NetworkVariable<float>(default,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<float> Energy = new NetworkVariable<float>(default,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private void Start()
    {
        //if (!IsOwner)
        //{
        //    Destroy(HealthBar);
        //    Destroy(BackHealthBar);
        //    Destroy(EnergyBar);
        //    Destroy(BackEnergyBar);
        //    return;
        //};
        if (IsServer)
        {
            Health.Value = startingHealth;
            Energy.Value = startingEnergy;
        }

        Health.OnValueChanged += (oldValue, newValue) => UpdateHungerUI();
        Energy.OnValueChanged += (oldValue, newValue) => UpdateThirstUI();
    }

    private void Update()
    {
        if (!IsOwner)
            return;


        if (IsServer)
        {
            DecreaseHealth(Time.deltaTime * HealthDecreaseRate);
            DecreaseEnergy(Time.deltaTime * EnergyDecreaseRate);
        }
        HealthBar.fillAmount = Health.Value / startingHealth;
        EnergyBar.fillAmount = Energy.Value / startingEnergy;
        
        UpdateHungerUI();
        UpdateThirstUI();
    }

    public void DecreaseHealth(float amount)
    {
        Health.Value -= amount;
        Health.Value = Mathf.Clamp(Health.Value, 0, 500);

        if (Health.Value <= 0)
        {
            Debug.Log("Island is out of Health");
        }
    }

    public void DecreaseEnergy(float amount)
    {
        Energy.Value -= amount;
        Energy.Value = Mathf.Clamp(Energy.Value, 0, 500);
        
        if (Energy.Value <= 0)
        {
            Debug.Log("Island is out of Energy");
        }
    }

    public void IncreaseHealth(float amount)
    {
        Health.Value += amount;
        Health.Value = Mathf.Clamp(Health.Value, 0, 500);
    }

    public void IncreaseEnergy(float amount)
    {
        Energy.Value += amount;
        Energy.Value = Mathf.Clamp(Energy.Value, 0, 500);
    }
    public void UpdateHungerUI()
    {
        float fillF = HealthBar.fillAmount;
        float fillB = BackHealthBar.fillAmount;
        float hFraction = Health.Value / startingHealth;
        UpdateUIValues(hFraction, ref fillF, ref fillB, HealthBar, BackHealthBar);
    }

    public void UpdateThirstUI()
    {
        float fillF = EnergyBar.fillAmount;
        float fillB = BackEnergyBar.fillAmount;
        float hFraction = Energy.Value / startingEnergy;
        UpdateUIValues(hFraction, ref fillF, ref fillB, EnergyBar, BackEnergyBar);
    }
    private void UpdateUIValues(float hFraction, ref float fillF, ref float fillB, Image frontBar, Image backBar)
    {
        if (fillB > hFraction)
        {
            frontBar.fillAmount = hFraction;
            backBar.color = Color.white;
            lerptimer += Time.deltaTime;
            float percentComplete = lerptimer / chipSpeed;
            backBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }
        if (fillF < hFraction)
        {
            frontBar.fillAmount = hFraction;
            backBar.color = Color.white;
            lerptimer += Time.deltaTime;
            float percentComplete = lerptimer / chipSpeed;
            backBar.fillAmount = Mathf.Lerp(fillF, hFraction, percentComplete);
        }
    }

    public void OpenHomeCoreUI()
    {
        HomeCoreUI.SetActive(true);
        playerMovement.canMove = false;
        cameraControl.canRotate = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseHomeCoreUI()
    {
        HomeCoreUI.SetActive(false);
        playerMovement.canMove = true;
        cameraControl.canRotate = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
