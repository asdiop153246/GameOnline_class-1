using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class DestroyPlayerZoneScript : NetworkBehaviour
{   
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerSpawnScript>().Respawn();           
        }
    }
}
