using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
public class HomeCoreScript : NetworkBehaviour
{
    public GameObject HomeCoreUI;
    private PlayerControllerScript playerMovement;
    private MoveCamera cameraControl;

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
    public NetworkVariable<float> Health = new NetworkVariable<float>(500f,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<float> Energy = new NetworkVariable<float>(500f,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Health.Value = startingHealth;
            Energy.Value = startingEnergy;
            FindPlayer();
            FindCamera();
        }                
        Health.OnValueChanged += (oldValue, newValue) => UpdateHealthUI();
        Energy.OnValueChanged += (oldValue, newValue) => UpdateEnergyUI();
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
        
        UpdateHealthUI();
        UpdateEnergyUI();
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
        if(Energy.Value > 500)
        {
            Energy.Value = 500;
        }
        Debug.Log($"Current Energy after Increaes = {Energy.Value}");
    }
    public void UpdateHealthUI()
    {
        float fillF = HealthBar.fillAmount;
        float fillB = BackHealthBar.fillAmount;
        float hFraction = Health.Value / startingHealth;
        UpdateUIValues(hFraction, ref fillF, ref fillB, HealthBar, BackHealthBar);
    }

    public void UpdateEnergyUI()
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
            lerptimer += Time.deltaTime;
            float percentComplete = lerptimer / chipSpeed;
            backBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }
        if (fillF < hFraction)
        {
            frontBar.fillAmount = hFraction;            
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
    private void FindPlayer()
    {
        GameObject Player = GameObject.FindWithTag("Player");
        if (Player != null)
        {
            Debug.Log("Detected Player for HomeCore");
            playerMovement = Player.GetComponent<PlayerControllerScript>();
            if (playerMovement == null)
            {
                Debug.LogError("PlayerControllerScript Component not found on HomeCore object!");
            }
        }
        else
        {
            Debug.LogError("Player object not found in the scene!");
        }
    }
    private void FindCamera()
    {
        GameObject Camera = GameObject.FindWithTag("MainCamera");
        if (Camera != null)
        {
            Debug.Log("Detected Camera for HomeCore");
            cameraControl = Camera.GetComponent<MoveCamera>();
            if (cameraControl == null)
            {
                Debug.LogError("CameraScript component not found on HomeCore object!");
            }
        }
        else
        {
            Debug.LogError("Camera Object not found in the scene!");
        }
    }
}
