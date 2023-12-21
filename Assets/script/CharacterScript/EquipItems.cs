using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EquipItems : NetworkBehaviour
{
    public InteractionScript Havespear;
    public InventoryScript inventory;
    public bool Isequip = false;
    [SerializeField]
    public NetworkObject Spear; 

    void Start()
    {
        
        Havespear = this.GetComponent<InteractionScript>();
        inventory = this.GetComponent<InventoryScript>();
        Spear.gameObject.SetActive(false); 
    }

    void Update()
    {
        if (!IsOwner)
            return; 

        if (inventory.spearCount.Value >= 1 && Input.GetKeyDown(KeyCode.Q))
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
        Isequip = true;
        EquipSpearClientRpc();
    }

    [ServerRpc]
    public void RequestUnEquipSpearServerRpc()
    {
        Isequip = false;
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
