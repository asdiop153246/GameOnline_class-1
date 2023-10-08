using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class NetworkDestroy : NetworkBehaviour
{
    [ServerRpc]
    void DestroyObjectServerRpc()
    {
        // Destroy the object on all clients
        NetworkObject.Despawn(gameObject);
    }

    public void RequestDestroy()
    {
        if (IsServer)
        {
            DestroyObjectServerRpc();
        }
        else if (IsClient)
        {

        }
    }
}
