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
        // Find the OtherCore object once instead of doing it every frame
        if (OtherCore == null)
        {
            OtherCore = GameObject.FindWithTag("OtherCoreManager").GetComponent<OtherCoreScript>();
        }
    }

    public void OnTransferEnergyButtonPressed(float amount)
    {
        OnEnergyButtonPressed?.Invoke(amount);
    }

    //[ServerRpc]
    //private void RequestTransferEnergyServerRpc(float amount)
    //{
    //    if (OtherCore != null)
    //    {
    //        OtherCore.TransferEnergyServer(amount);
    //    }
    //    else
    //    {
    //        Debug.LogError("OtherCoreScript not found.");
    //    }
    //}
}
