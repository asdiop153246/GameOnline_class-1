using UnityEngine;
using Unity.Netcode;

public class PickupSpear : NetworkBehaviour
{
    public KeyCode pickUpKey = KeyCode.E;
    public bool isNearSpear = false;
    private GameObject Pspear;
    public bool HaveSpear = false;

    private void Update()
    {
        if (!IsOwner)
            return;

        if (isNearSpear && Input.GetKeyDown(pickUpKey))
        {
            Debug.Log("Client: Attempting to pick up spear");
            TryPickUpSpearServerRpc();
        }
    }

    [ServerRpc]
    void TryPickUpSpearServerRpc()
    {
        Debug.Log("Server: Received pick up request");

        if (HaveSpear)
        {
            Debug.Log("Server: Already have a spear");
            return;
        }

        if (isNearSpear && Pspear != null)
        {
            Debug.Log("Server: Picking up spear");
            HaveSpear = true;

            // Communicate the new game state to all clients
            PickUpSpearClientRpc();
        }
        else
        {
            Debug.Log("Server: Cannot pick up - isNearSpear: " + isNearSpear + ", Pspear: " + Pspear);
        }
    }

    [ClientRpc]
    void PickUpSpearClientRpc()
    {
        Debug.Log("Client: Received confirmation to pick up spear");

        if (Pspear != null)
        {
            Destroy(Pspear);
        }
        else
        {
            Debug.Log("Client: Pspear is null");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner)
            return;

        if (other.CompareTag("PSpear"))
        {
            isNearSpear = true;
            Pspear = other.gameObject;
            Debug.Log("In Spear Area");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsOwner)
            return;

        if (other.CompareTag("PSpear"))
        {
            isNearSpear = false;
            Pspear = null;
        }
    }
}