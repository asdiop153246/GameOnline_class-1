using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
public class ButtonInteractionScript : NetworkBehaviour
{
    public OtherCoreScript OtherCore;
    private void Update()
    {
        OtherCore = GameObject.FindWithTag("OtherCore").GetComponent<OtherCoreScript>();
    }
    public void OnTransferEnergyButtonPressed(float amount)
    {
        Debug.Log("Player press the transfer button");
        RequestTransferEnergyServerRpc(amount);
        
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestTransferEnergyServerRpc(float amount)
    {
        // Find the OtherCoreScript instance and call TransferEnergy
        // You might need to adjust how you get the OtherCoreScript instance
        
        if (OtherCore != null)
        {
            OtherCore.TransferEnergy(amount);
        }
        else
        {
            Debug.LogError("OtherCoreScript not found.");
        }
    }

}
