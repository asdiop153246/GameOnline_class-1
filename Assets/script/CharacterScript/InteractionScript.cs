using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
public class InteractionScript : NetworkBehaviour
{
    public KeyCode interactKey = KeyCode.E;
    private GameObject Pspear;
    private GameObject HomeCore;
    private GameObject currentWood;
    private GameObject currentFood;

    public bool HaveSpear = false;
    public bool isOpeningUI = false;
    public TextMeshProUGUI notificationText;
    public GameObject notificationPanel;
    public MoveCamera cameraControl;
    public GameObject HomeCoreUI;
    public PlayerControllerScript playerMovement;
    public HomeCoreScript HomeCoreScript;

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
            isOpeningUI = true;
            HomeCoreScript.OpenHomeCoreUI();
        }
        else if (Input.GetKeyDown(interactKey) && isOpeningUI)
        {
            Debug.Log("Client: Closing HomeCore UI");
            isOpeningUI = false;
            HomeCoreScript.CloseHomeCoreUI();
        }

        if (Input.GetKeyDown(interactKey) && IsLookingAtWood())
        {
            Debug.Log("Client: Attempting to pick up wood");
            TryPickUpWoodServerRpc();
        }
        if (Input.GetKeyDown(interactKey) && IsLookingAtFood())
        {
            NetworkObject foodNetworkObject = currentFood.GetComponent<NetworkObject>();
            Debug.Log("Client: Attempting to pick up Food");
            if (foodNetworkObject)
            {
                TryPickUpFoodServerRpc(foodNetworkObject.NetworkObjectId);
            }
        }
    }

    [ServerRpc]
    void TryPickUpSpearServerRpc(ulong spearNetworkObjectId, ServerRpcParams rpcParams = default)
    {
        NetworkObject targetSpear = NetworkManager.Singleton.SpawnManager.SpawnedObjects[spearNetworkObjectId];
        InventoryScript inventory = GetComponent<InventoryScript>();
        if (targetSpear != null)
        {
            Debug.Log("Server: Picking up spear");
            inventory.IncreaseSpearCount(1);
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
    private bool IsLookingAtWood()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            if (hit.collider.CompareTag("Woods") && hit.distance < 3f)
            {
                currentWood = hit.collider.gameObject;
                Debug.Log("Looking at Woods");
                return true;
            }
        }

        return false;
    }
    private bool IsLookingAtFood()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            if (hit.collider.CompareTag("Food") && hit.distance < 3f)
            {
                currentFood = hit.collider.gameObject;
                Debug.Log("Looking at Woods");
                return true;
            }
        }

        return false;
    }

    [ServerRpc]
    void TryPickUpWoodServerRpc(ServerRpcParams rpcParams = default)
    {
        Debug.Log("Server: Attempting to pick up wood");
        WoodResourceScript woodResource = currentWood.GetComponent<WoodResourceScript>();
        Debug.Log("Server: Wood can be picked up? " + woodResource.CanBePickedUp());
        if (woodResource.CanBePickedUp())
        {
            InventoryScript inventory = GetComponent<InventoryScript>();
            int randomWoodAmount = Random.Range(1, 3);
            if (inventory != null)
            {
                woodResource.PickUp();
                inventory.IncreaseWoodCount(randomWoodAmount);
                PickUpWoodClientRpc();
            }
        }
    }
    [ServerRpc]
    void TryPickUpFoodServerRpc(ulong foodNetworkObjectId,ServerRpcParams rpcParams = default)
    {
            Debug.Log("Server: Attempting to pick up Food");
            NetworkObject targetfood = NetworkManager.Singleton.SpawnManager.SpawnedObjects[foodNetworkObjectId];
            InventoryScript inventory = GetComponent<InventoryScript>();
            if (targetfood != null && inventory != null)
            {
            Debug.Log("Server: Picking up Food");
            inventory.IncreaseFoodCount(1);
            targetfood.Despawn();
            Destroy(targetfood.gameObject);

            //PickUpSpearClientRpc(rpcParams.Receive.SenderClientId);
            }
            
    }

    [ClientRpc]
    void PickUpWoodClientRpc()
    {
        
    }

    //private void OpenHomeCoreUI()
    //{
    //    HomeCoreUI.SetActive(true); 
    //    isOpeningUI = true;
    //    playerMovement.canMove = false;
    //    cameraControl.canRotate = false;
    //    Cursor.lockState = CursorLockMode.None;
    //    Cursor.visible = true;
    //}

    //private void CloseHomeCoreUI()
    //{
    //    HomeCoreUI.SetActive(false); 
    //    isOpeningUI = false;
    //    playerMovement.canMove = true;
    //    cameraControl.canRotate = true;
    //    Cursor.lockState = CursorLockMode.Locked;
    //    Cursor.visible = false;
    //}
}
