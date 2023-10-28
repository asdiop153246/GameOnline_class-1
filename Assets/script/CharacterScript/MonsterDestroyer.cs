using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class MonsterDestroyer : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        
        //if (!IsServer) return;

        
        if (other.CompareTag("Enemy"))
        {
            
            NetworkObject netObject = other.gameObject.GetComponent<NetworkObject>();

            
            if (netObject != null)
            {
                netObject.Despawn(); 
                Destroy(other.gameObject); 
            }
        }
    }
}
