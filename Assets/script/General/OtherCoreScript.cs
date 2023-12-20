using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;
using UnityEngine.UI;

public class OtherCoreScript : NetworkBehaviour
{
    public GameObject CoreUI;

    [Header("Values")]
    [SerializeField] private float startingEnergyRangeMin = 300;
    [SerializeField] private float startingEnergyRangeMax = 500;
    public NetworkVariable<float> Energy = new NetworkVariable<float>();

    [Header("Rate of Decrease")]
    public float EnergyDecreaseRate = 0.80f;

    [Header("UI Elements")]
    public Image EnergyBar;
    public Image BackEnergyBar;

    private float lerptimer;
    public float chipSpeed = 2f;
    public bool isUIReady = false;

    public HomeCoreScript HomeCore;
    public GameObject OtherCoreObject;
    public EnergyHolderScript PlayerScript;
    public bool energyInitialized = false; // Flag to check if energy is initialized

    private void OnEnable()
    {
        ButtonInteractionScript.OnEnergyButtonPressed += TransferEnergyServerRpc;
    }
    private void OnDisable()
    {
        ButtonInteractionScript.OnEnergyButtonPressed -= TransferEnergyServerRpc;
    }
    public override void OnNetworkSpawn()
    {
        if (IsServer && !energyInitialized)
        {
            Energy.Value = UnityEngine.Random.Range(startingEnergyRangeMin, startingEnergyRangeMax);
            Debug.Log($"[Server] Setting starting energy to: {Energy.Value}");
            energyInitialized = true;
        }

        Energy.OnValueChanged += UpdateEnergyUI;
    }

    private void Update()
    {
        StartCoroutine(FindUIElements());

        if (IsServer)
        {
            DecreaseEnergy(Time.deltaTime * EnergyDecreaseRate);
        }

        if (HomeCore == null)
        {
            HomeCore = GameObject.FindWithTag("HomeCore").GetComponent<HomeCoreScript>();
        }
        if (OtherCoreObject == null)
        {
            OtherCoreObject = GameObject.FindWithTag("OtherCore");
        }
        if (PlayerScript == null)
        {
            PlayerScript = GameObject.FindWithTag("Player").GetComponent<EnergyHolderScript>();
        }

        // Debugging the current energy value on the client
        //Debug.Log($"Current Energy (Update): {Energy.Value}");
    }

    private void DecreaseEnergy(float amount)
    {
        Energy.Value = Mathf.Clamp(Energy.Value - amount, 0, startingEnergyRangeMax);
    }

    private void UpdateEnergyUI(float oldValue, float newValue)
    {
        if (EnergyBar != null && BackEnergyBar != null)
        {
            float hFraction = newValue / startingEnergyRangeMax;
            UpdateUIValues(hFraction);
        }
    }

    private void UpdateUIValues(float hFraction)
    {
        if (BackEnergyBar.fillAmount > hFraction)
        {
            lerptimer += Time.deltaTime;
            BackEnergyBar.fillAmount = Mathf.Lerp(BackEnergyBar.fillAmount, hFraction, lerptimer / chipSpeed);
        }
        if (EnergyBar.fillAmount < hFraction)
        {
            lerptimer += Time.deltaTime;
            EnergyBar.fillAmount = Mathf.Lerp(EnergyBar.fillAmount, hFraction, lerptimer / chipSpeed);
        }
    }
    [ServerRpc]
    public void TransferEnergyServerRpc(float amount)
    {
        if (IsServer)
        {
            Debug.Log("[Server]Is in TransferEnergy Function");
            if (Energy.Value >= amount)
            {
                Debug.Log("Value > amount transfering");
                Energy.Value -= amount;
                PlayerScript.IncreaseEnergy(amount);
            }
        }
    }
    //public void TransferEnergyButton(float amount)
    //{
    //    if (IsLocalPlayer)
    //    {
    //        RequestTransferEnergyServerRpc(amount);
    //    }
    //}

    //[ServerRpc]
    //private void RequestTransferEnergyServerRpc(float amount)
    //{
    //    if (Energy.Value >= amount)
    //    {
    //        Debug.Log($"[Server] Before transfer, Energy: {Energy.Value}");
    //        Energy.Value -= amount;
    //        Debug.Log($"[Server] After transfer, Energy: {Energy.Value}");
    //        HomeCore?.IncreaseEnergy(amount);
    //    }
    //}

    public void OpenCoreUI()
    {
        Debug.Log($"Current Energy is {Energy.Value}");
        CoreUI.SetActive(true);
    }

    public void CloseCoreUI()
    {
        CoreUI.SetActive(false);
    }
    private IEnumerator FindUIElements()
    {
        while (!isUIReady)
        {
            if (CoreUIManager.Instance != null)
            {
                CoreUI = CoreUIManager.Instance.CoreUI;
                EnergyBar = CoreUIManager.Instance.EnergyBar;
                BackEnergyBar = CoreUIManager.Instance.BackEnergyBar;

                if (CoreUI != null && EnergyBar != null && BackEnergyBar != null)
                {
                    Debug.Log("UI Elements found successfully.");
                    isUIReady = true;
                }
                else
                {
                    Debug.LogWarning("UI Elements not found, retrying...");
                    yield return new WaitForSeconds(0.5f);
                }
            }
            else
            {
                Debug.LogWarning("CoreUIManager instance not found, retrying...");
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
