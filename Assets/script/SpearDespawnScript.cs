using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class SpearDespawnScript : NetworkBehaviour
{
    public bool hasPlayer = false;
    private void OnTriggerEnter(Collider other)
    {
        PlayerControllerScript player = other.GetComponent<PlayerControllerScript>();
        if (player != null)
        {
            // Player entered the collider area
            Debug.Log("Player entered the area!");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        PlayerControllerScript player = other.GetComponent<PlayerControllerScript>();
        if (player != null && Input.GetKeyDown(KeyCode.E))
        {
            // Player pressed E while inside the collider area
            Debug.Log("Player pressed E!");
        }
    }
}
