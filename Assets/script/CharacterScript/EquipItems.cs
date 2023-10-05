using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EquipItems : NetworkBehaviour
{
    public PickupSpear Havespear; // Changed the type to PickupSpear
    public bool Isequip = false;
    [SerializeField]
    GameObject Spear;

    void Start()
    {
        if (!IsOwner) return;
        Havespear = this.GetComponent<PickupSpear>();
    }

    void Update()
    {
        
        if (Havespear.HaveSpear && Input.GetKeyDown(KeyCode.Q) && !Isequip)
        {
            Debug.Log("Trying to equip spear");
            RequestEquipSpearServerRpc();
        }
        else if (Havespear.HaveSpear && Input.GetKeyDown(KeyCode.Q) && Isequip)
        {
            Debug.Log("Trying to unequip spear");
            RequestUnEquipSpearServerRpc();
        }
    }

    [ServerRpc]
    public void RequestEquipSpearServerRpc()
    {
        EquipSpear();
    }
    [ServerRpc]
    public void RequestUnEquipSpearServerRpc()
    {
        UnEquipSpear();
    }
    public void EquipSpear()
    {
        Spear.SetActive(true);
        Isequip = true;
    }
    public void UnEquipSpear()
    {
        Spear.SetActive(false);
        Isequip = false;
    }
}
