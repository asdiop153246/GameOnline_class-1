using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class ResourcesScript : NetworkBehaviour
{
    public enum ResourceType
    {
        Wood,
        Rope,
        Food,
        Water,
        Cola,
    }

    public ResourceType resourceType;
    public int maxPickups = 4; //For wood
    public NetworkVariable<int> currentPickups = new NetworkVariable<int>(0);
    public int amountPerPickup = 1; 

    public delegate void ResourcePickedUpAction(ResourceType type, int amount);
    public static event ResourcePickedUpAction OnResourcePickedUp;

    private void Start()
    {
        
        if (resourceType != ResourceType.Wood)
        {
            maxPickups = 1;
        }
    }

    public bool CanBePickedUp()
    {
        return currentPickups.Value < maxPickups;
    }

    public void PickUp()
    {
        if (IsServer)
        {
            currentPickups.Value++;
            if (resourceType == ResourceType.Wood)
            {
                amountPerPickup = Random.Range(1, 4);
            }

            if (currentPickups.Value >= maxPickups)
            {
                DespawnResourceServerRpc();
            }

            
            OnResourcePickedUp?.Invoke(resourceType, amountPerPickup);
        }
    }
    [ServerRpc]
    private void DespawnResourceServerRpc()
    {
        DespawnResourceClientRpc();
        NetworkObject networkObject = GetComponent<NetworkObject>();
        networkObject.gameObject.SetActive(false);
        //NetworkObject networkObject = GetComponent<NetworkObject>();
        //if (networkObject)
        //{
        //    networkObject.gameObject.SetActive(false);
        //    //networkObject.Despawn();
        //    //Destroy(gameObject);
        //}
    }
    [ClientRpc]
    private void DespawnResourceClientRpc()
    {
        NetworkObject networkObject = GetComponent<NetworkObject>();
        if (networkObject)
        {
            networkObject.gameObject.SetActive(false);
            //networkObject.Despawn();
            //Destroy(gameObject);
        }
    }
}
