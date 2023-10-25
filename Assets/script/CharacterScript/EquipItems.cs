using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EquipItems : NetworkBehaviour
{
    public PickupSpear Havespear;
    public bool Isequip = false;
    [SerializeField]
    public NetworkObject Spear; // Changed GameObject to NetworkObject

    void Start()
    {
        
        Havespear = this.GetComponent<PickupSpear>();
        Spear.gameObject.SetActive(false); // Ensure spear is inactive at the start
    }

    void Update()
    {
        if (!IsOwner)
            return; // Ensure only the owner can process the input.

        if (Havespear.HaveSpear && Input.GetKeyDown(KeyCode.Q))
        {
            if (!Isequip)
            {
                Debug.Log("Trying to equip spear");
                RequestEquipSpearServerRpc();
            }
            else
            {
                Debug.Log("Trying to unequip spear");
                RequestUnEquipSpearServerRpc();
            }
        }
    }

    [ServerRpc]
    public void RequestEquipSpearServerRpc()
    {
        EquipSpearClientRpc();
    }

    [ServerRpc]
    public void RequestUnEquipSpearServerRpc()
    {
        UnEquipSpearClientRpc();
    }

    [ClientRpc]
    public void EquipSpearClientRpc()
    {
        Spear.gameObject.SetActive(true);
        Isequip = true;
    }

    [ClientRpc]
    public void UnEquipSpearClientRpc()
    {
        Spear.gameObject.SetActive(false);
        Isequip = false;
    }
}
