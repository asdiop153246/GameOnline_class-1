using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class WaterScript : NetworkBehaviour
{
    PlayerControllerScript movement;
    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner) return;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && other.GetComponent<PlayerControllerScript>() != null)
        {
            Debug.Log("Player enter water area");
            movement = other.GetComponent<PlayerControllerScript>();
            movement.isSwimming = true;         
            movement.canJump = false;
        }
        if (other.CompareTag("EyeLevel"))
        {
            other.GetComponentInParent<Rigidbody>().useGravity = false;
            if (movement != null)
            {
                movement.ResetVector();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<PlayerControllerScript>() != null)
        {
            Debug.Log("Player leave water area");
            PlayerControllerScript movement = other.GetComponent<PlayerControllerScript>();
            movement.isSwimming = false;
            movement.canJump = true;
            movement.ResetVector();
        }
    }
}
