using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class OtherCoreScript : NetworkBehaviour
{
    public GameObject CoreUI;

    [Header("Values")]
    [SerializeField] private float startingEnergyRangeMin = 300;
    [SerializeField] private float startingEnergyRangeMax = 500;
    [SerializeField] private NetworkVariable<float> Energy = new NetworkVariable<float>();

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
    public bool energyInitialized = false; // Flag to check if energy is initialized

    public override void OnNetworkSpawn()
    {
        if (IsServer && !energyInitialized)
        {
            Energy.Value = Random.Range(startingEnergyRangeMin, startingEnergyRangeMax);
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

    public void TransferEnergy(float amount)
    {
        if (IsServer)
        {
            Debug.Log("[Server]Is in TransferEnergy Function");
            if (Energy.Value >= amount)
            {
                Debug.Log("Value > amount transfering");
                Energy.Value -= amount;
                HomeCore.IncreaseEnergy(amount);
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
