using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
public class ButtonInteractionScript : NetworkBehaviour
{
    public OtherCoreScript OtherCore;
    public static Action<float> OnEnergyButtonPressed;
    public static Action<float> OnReplenishButtonPressed;
    public EnergyHolderScript EnergyHolder;


    private void Update()
    {      
        if (OtherCore == null)
        {
            OtherCore = GameObject.FindWithTag("OtherCoreManager").GetComponent<OtherCoreScript>();
        }
        Debug.Log($"Energy in ButtonInter = {EnergyHolder._energy.Value}");
        //OnReplenishEnergyButtonPressed(1f);
    }

    public void OnTransferEnergyButtonPressed(float amount)
    {
        OnEnergyButtonPressed?.Invoke(amount);
    }
    public void OnReplenishEnergyButtonPressed(float amount)
    {
        Debug.Log($"Energy = {EnergyHolder._energy.Value}");

        if (EnergyHolder._energy.Value < 100f)
        {
            Debug.Log("Not have enough Energy");
            return;
        }
        else
        {
            Debug.Log("Stupid Netcode");
        }
        OnReplenishButtonPressed?.Invoke(amount);
    }
}
