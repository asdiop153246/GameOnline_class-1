using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
public class PickupSpear : NetworkBehaviour
{
    public KeyCode pickUpKey = KeyCode.E;  // Change this to set a different pickup key
    public Transform rightHand;  // Set this in the inspector to your RightHand object
    private bool isNearSpear = false;
    private GameObject spear;

    void Update()
    {
        // Check if the player is near the spear and the pickup key is pressed
        if (IsOwner && isNearSpear && Input.GetKeyDown(pickUpKey))
        {
            // Request to pick up the spear
            RequestPickUpSpearServerRpc();
        }
    }

    [ServerRpc]
    void RequestPickUpSpearServerRpc()
    {
        PickUpSpearServer();
    }

    void PickUpSpearServer()
    {
        if (spear != null)
        {
            // Attach the spear to the player
            spear.transform.SetParent(rightHand);
            spear.transform.localPosition = Vector3.zero;  // Position at the origin of the hand
            spear.transform.localRotation = Quaternion.identity;  // Reset rotation
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the player is entering the spear's trigger
        if (other.gameObject.CompareTag("Spear"))
        {
            isNearSpear = true;
            spear = other.gameObject;
            Debug.Log("In Spear Area");
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Check if the player is leaving the spear's trigger
        if (other.gameObject.CompareTag("Spear"))
        {
            isNearSpear = false;
            spear = null;
        }
    }
}

