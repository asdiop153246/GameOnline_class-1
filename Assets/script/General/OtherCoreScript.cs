using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
public class OtherCoreScript : NetworkBehaviour
{
    private GameObject CoreUI;
    private PlayerControllerScript playerMovement;
    private MoveCamera cameraControl;

    [Header("Values")]
    private float startingEnergy;

    [Header("Rate of Decrease")]
    public float EnergyDecreaseRate = 0.80f;

    private Image EnergyBar;
    private Image BackEnergyBar;

    private float lerptimer;
    public float chipSpeed = 2f;
    private NetworkVariable<float> Energy = new NetworkVariable<float>();
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            startingEnergy = Random.Range(300, 500);
            Energy.Value = startingEnergy;
            CoreUI = CoreUIManager.Instance.CoreUI;
            EnergyBar = CoreUIManager.Instance.EnergyBar;
            BackEnergyBar = CoreUIManager.Instance.BackEnergyBar;
            playerMovement = CoreUIManager.Instance.PlayerMovement;
            cameraControl = CoreUIManager.Instance.CameraMovement;
            //FindPlayer();
            //FindCamera();
        }
        Energy.OnValueChanged += (oldValue, newValue) => UpdateEnergyUI();
    }
    private void Update()
    {
        if (!IsOwner)
            return;


        if (IsServer)
        {

            DecreaseEnergy(Time.deltaTime * EnergyDecreaseRate);
        }
        EnergyBar.fillAmount = Energy.Value / startingEnergy;

        ;
        UpdateEnergyUI();
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
    public void TransferEnergy(float amount)
    {
        if (Energy.Value >= amount)
        {
            Energy.Value -= amount;
            // Call HomeCoreScript to increase its energy
            FindObjectOfType<HomeCoreScript>().IncreaseEnergy(amount);
        }
    }
    public void OpenCoreUI()
    {
        Debug.Log("Opening CoreUI");
        Debug.Log($"Current Energy is {Energy.Value}");
        CoreUI.SetActive(true);
        playerMovement.canMove = false;
        cameraControl.canRotate = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseCoreUI()
    {
        Debug.Log("Closing CoreUI");
        CoreUI.SetActive(false);
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
            Debug.Log("Detected Player for OtherCore");
            playerMovement = Player.GetComponent<PlayerControllerScript>();
            if (playerMovement == null)
            {
                Debug.LogError("PlayerControllerScript Component not found on OtherCore object!");
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
            Debug.Log("Detected Camera for OtherCore");
            cameraControl = Camera.GetComponent<MoveCamera>();
            if (cameraControl == null)
            {
                Debug.LogError("CameraScript component not found on OtherCore object!");
            }
        }
        else
        {
            Debug.LogError("Camera Object not found in the scene!");
        }
    }
}
