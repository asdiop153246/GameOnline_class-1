using UnityEngine;
using Unity.Netcode;

public class PickupSpear : NetworkBehaviour
{
    public KeyCode pickUpKey = KeyCode.E;  // Change this to set a different pickup key
    public bool isNearSpear = false;
    private GameObject Pspear;
    public bool HaveSpear = false;

    private void Update()
    {
        if (!IsOwner)
            return;

        if (isNearSpear == true && Input.GetKeyDown(pickUpKey))
        {
            Debug.Log("Attempting to pick up spear");
            PickUpSpearClientRpc();
        }
    }
    [ClientRpc]
    void PickUpSpearClientRpc()
    {
        if (HaveSpear)
        {
            Debug.Log("already Have spear");
            return;
        }
           

        if (isNearSpear == true && Pspear != null)
        {
            Debug.Log("Picked up spear");
            HaveSpear = true;
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
