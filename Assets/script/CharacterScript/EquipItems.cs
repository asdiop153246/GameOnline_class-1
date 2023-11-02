using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EquipItems : NetworkBehaviour
{
    public InteractionScript Havespear;
    public bool Isequip = false;
    [SerializeField]
    public NetworkObject Spear; 

    void Start()
    {
        
        Havespear = this.GetComponent<InteractionScript>();
        Spear.gameObject.SetActive(false); 
    }

    void Update()
    {
        if (!IsOwner)
            return; 

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
