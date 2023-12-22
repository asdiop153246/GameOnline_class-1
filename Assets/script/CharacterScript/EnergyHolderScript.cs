using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class EnergyHolderScript : NetworkBehaviour
{
    private OtherCoreScript OtherCore;
    private HomeCoreScript HomeCore;
    public bool canHoldEnergy = true;
    public NetworkVariable<float> MaxEnergy = new NetworkVariable<float>(200);
    public NetworkVariable<float> energy = new NetworkVariable<float>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> _energy = new NetworkVariable<float>();
    public float Energy
    {
        get
        {
            return _energy.Value;
        }
        set
        {
            _energy.Value = value;
        }
    }

    private void Start()
    {
        Energy = 101f;
        HomeCore = GameObject.FindWithTag("HomeCore").GetComponent<HomeCoreScript>();
    }
    private void Update()
    {
        if (OtherCore == null)
        {
            OtherCore = GameObject.FindWithTag("OtherCoreManager").GetComponent<OtherCoreScript>();
        }        
        if (_energy.Value == MaxEnergy.Value)
        {
            canHoldEnergy = false;
        }
        else
        {
            canHoldEnergy = true;
        }
    }

    public void IncreaseEnergy(float amount)
    {
        Debug.Log($"Transfering {amount} Energy into PlayerHolderScript");
        IncreaseEnergyServerRpc(amount);
    }

    [ServerRpc]
    public void IncreaseEnergyServerRpc(float amount)
    {
        
        _energy.Value += amount;
        Debug.Log($"Current Energy in Holder = {_energy.Value}");
        if (_energy.Value > MaxEnergy.Value)
        {
            _energy.Value = MaxEnergy.Value;
            Debug.Log("You can't hold more Energy than 200");
        }

    }

}
