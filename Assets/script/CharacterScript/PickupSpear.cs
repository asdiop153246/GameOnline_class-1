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
            Debug.Log("Attempting to pick up spear");
            TryPickUpSpearServerRpc();
        }
    }

    [ServerRpc]
    void TryPickUpSpearServerRpc()
    {
        if (HaveSpear)
        {
            Debug.Log("Already have spear");
            return;
        }

        if (isNearSpear && Pspear != null)
        {
            Debug.Log("Picked up spear");
            HaveSpear = true;

            // If necessary, communicate the new game state to all clients
            PickUpSpearClientRpc();
        }
    }

    [ClientRpc]
    void PickUpSpearClientRpc()
    {
        // Update the game state on all clients
        // (For example, make the spear disappear)
        if (Pspear != null)
        {
            Destroy(Pspear);
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