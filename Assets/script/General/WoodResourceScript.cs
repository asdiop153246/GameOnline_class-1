using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class WoodResourceScript : NetworkBehaviour
{
    public int maxPickups = 3;
    private NetworkVariable<int> currentPickups = new NetworkVariable<int>(0); 

    public bool CanBePickedUp()
    {
        return currentPickups.Value < maxPickups;
    }

    public void PickUp()
    {
        if (IsServer) 
        {
            currentPickups.Value++;
            if (currentPickups.Value >= maxPickups)
            {
                
                NetworkObject networkObject = GetComponent<NetworkObject>();
                if (networkObject)
                {
                    networkObject.Despawn();
                    Destroy(gameObject);
                }
                
            }
        }
    }
}
