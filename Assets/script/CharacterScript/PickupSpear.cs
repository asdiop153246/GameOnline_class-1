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
        if (isNearSpear && Input.GetKeyDown(pickUpKey) && !HaveSpear)
        {
            Debug.Log("Client: Attempting to pick up spear");
            TryPickUpSpearServerRpc();
        }
    }

    [ServerRpc]
    void TryPickUpSpearServerRpc(ServerRpcParams rpcParams = default)
    {
        if (isNearSpear && Pspear != null)
        {
            Debug.Log("Server: Picking up spear");
            HaveSpear = true;

            NetworkObject networkObj = Pspear.GetComponent<NetworkObject>();
            if (networkObj)
            {
                networkObj.Despawn();
            }
            Destroy(Pspear);

            PickUpSpearClientRpc(rpcParams.Receive.SenderClientId);
        }
    }

    [ClientRpc]
    void PickUpSpearClientRpc(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            HaveSpear = true;
            isNearSpear = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PSpear"))
        {
            isNearSpear = true;
            Pspear = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PSpear"))
        {
            isNearSpear = false;
            Pspear = null;
        }
    }
}