using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class InteractionScript : NetworkBehaviour
{
    public KeyCode interactKey = KeyCode.E;
    private GameObject Pspear;
    private GameObject HomeCore;
    
    public bool HaveSpear = false;
    public bool isOpeningUI = false;

    public MoveCamera cameraControl;
    public GameObject HomeCoreUI;
    public PlayerControllerScript playerMovement;
    private void Update()
    {
        if (Input.GetKeyDown(interactKey) && !HaveSpear && IsLookingAtSpear())
        {
            Debug.Log("Client: Attempting to pick up spear");
            NetworkObject spearNetworkObject = Pspear.GetComponent<NetworkObject>();
            if (spearNetworkObject)
            {
                TryPickUpSpearServerRpc(spearNetworkObject.NetworkObjectId);
            }
        }
        if (Input.GetKeyDown(interactKey) && !isOpeningUI && IsLookingAtHomeCore()) 
        {
            Debug.Log("Client: Attempting to Opening HomeCore UI");
            OpenHomeCoreUI();
        }
        else if (Input.GetKeyDown(interactKey) && isOpeningUI)
        {
            Debug.Log("Client: Closing HomeCore UI");
            CloseHomeCoreUI();
        }
    }

    [ServerRpc]
    void TryPickUpSpearServerRpc(ulong spearNetworkObjectId, ServerRpcParams rpcParams = default)
    {
        NetworkObject targetSpear = NetworkManager.Singleton.SpawnManager.SpawnedObjects[spearNetworkObjectId];
        if (targetSpear != null)
        {
            Debug.Log("Server: Picking up spear");

            HaveSpear = true;
            targetSpear.Despawn();
            Destroy(targetSpear.gameObject);

            PickUpSpearClientRpc(rpcParams.Receive.SenderClientId);
        }
    }

    [ClientRpc]
    void PickUpSpearClientRpc(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            HaveSpear = true;
        }
    }

    private bool IsLookingAtSpear()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            if (hit.collider.CompareTag("PSpear") && hit.distance < 3f)
            {
                Pspear = hit.collider.gameObject;
                Debug.Log("Looking at Spear");
                return true;
            }
        }

        return false;
    }
    private bool IsLookingAtHomeCore()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            if (hit.collider.CompareTag("HomeCore") && hit.distance < 3f)
            {
                HomeCore = hit.collider.gameObject;
                Debug.Log("Looking at HomeCore");
                return true;
            }
        }

        return false;
    }

    private void OpenHomeCoreUI()
    {
        HomeCoreUI.SetActive(true); 
        isOpeningUI = true;
        playerMovement.canMove = false;
        cameraControl.canRotate = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void CloseHomeCoreUI()
    {
        HomeCoreUI.SetActive(false); 
        isOpeningUI = false;
        playerMovement.canMove = true;
        cameraControl.canRotate = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
