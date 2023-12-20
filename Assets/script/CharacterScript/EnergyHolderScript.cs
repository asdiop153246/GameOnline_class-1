using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class EnergyHolderScript : NetworkBehaviour
{
    private OtherCoreScript OtherCore;
    private HomeCoreScript HomeCore;
    public NetworkVariable<float> MaxEnergy = new NetworkVariable<float>(200);
    public NetworkVariable<float> Energy = new NetworkVariable<float>();

    private void Start()
    {        
        HomeCore = GameObject.FindWithTag("HomeCore").GetComponent<HomeCoreScript>();
    }
    private void Update()
    {
        if (OtherCore == null)
        {
            OtherCore = GameObject.FindWithTag("OtherCoreManager").GetComponent<OtherCoreScript>();
        }
    }

    public void IncreaseEnergy(float amount)
    {
        Debug.Log("Transfering Energy is in PlayerHolderScript");
        IncreaseEnergyServerRpc(amount);
    }

    [ServerRpc]
    private void IncreaseEnergyServerRpc(float amount)
    {
        Energy.Value += amount;        
        if (Energy.Value > MaxEnergy.Value)
        {
            Energy.Value = MaxEnergy.Value;
            Debug.Log($"Current Energy after Increaes = {Energy.Value}");
        }
        else
        {
            Debug.Log("You're can't hold more Energy");
        }
        
    }

}
