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
    private void Update()
    {      
        if (OtherCore == null)
        {
            OtherCore = GameObject.FindWithTag("OtherCoreManager").GetComponent<OtherCoreScript>();
        }
    }

    public void OnTransferEnergyButtonPressed(float amount)
    {
        OnEnergyButtonPressed?.Invoke(amount);
    }
}
