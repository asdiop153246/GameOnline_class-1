using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
public class OtherCoreScript : NetworkBehaviour
{
    public GameObject CoreUI;

    [Header("Values")]
    private float startingEnergy;

    [Header("Rate of Decrease")]
    public float EnergyDecreaseRate = 0.80f;

    public Image EnergyBar;
    public Image BackEnergyBar;

    private float lerptimer;
    public float chipSpeed = 2f;
    public bool isUIReady = false;
    private NetworkVariable<float> Energy = new NetworkVariable<float>();
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            startingEnergy = Random.Range(300, 500);
            Energy.Value = startingEnergy;
        }
        Energy.OnValueChanged += (oldValue, newValue) => UpdateEnergyUI();        
    }
    private IEnumerator FindUIElements()
    {
        while (!isUIReady)
        {
            if (CoreUIManager.Instance != null)
            {
                Debug.Log("Finding UI Elements");
                CoreUI = CoreUIManager.Instance.CoreUI;
                EnergyBar = CoreUIManager.Instance.EnergyBar;
                BackEnergyBar = CoreUIManager.Instance.BackEnergyBar;
                isUIReady = true;
            }
            else
            {
                yield return new WaitForSeconds(0.5f); // Check again after a delay
            }
        }
    }
    private void Update()
    {
        //if (!IsOwner || !isUIReady)
        //    return;
        StartCoroutine(FindUIElements());
        if (IsServer)
        {
            DecreaseEnergy(Time.deltaTime * EnergyDecreaseRate);
        }

        EnergyBar.fillAmount = Energy.Value / startingEnergy;
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
    [ServerRpc]
    public void TransferEnergyServerRpc(float amount)
    {
        if (Energy.Value >= amount)
        {
            Debug.Log($"Transfering {amount} Energy to Home");
            Energy.Value -= amount;
            // Call HomeCoreScript to increase its energy
            FindObjectOfType<HomeCoreScript>().IncreaseEnergyServerRpc(amount);
        }
    }
    public void OpenCoreUI()
    {
        Debug.Log("Opening CoreUI");
        Debug.Log($"Current Energy is {Energy.Value}");
        CoreUI.SetActive(true);
    }

    public void CloseCoreUI()
    {
        Debug.Log("Closing CoreUI");
        CoreUI.SetActive(false);
    }

}
