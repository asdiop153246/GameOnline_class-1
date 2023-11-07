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
    private GameObject currentResource;
    public bool HaveSpear = false;
    public bool isOpeningUI = false;
    public TextMeshProUGUI notificationText;
    public GameObject notificationPanel;
    public MoveCamera cameraControl;
    public GameObject HomeCoreUI;
    public PlayerControllerScript playerMovement;
    private HomeCoreScript homeCoreScript;
    public InventoryScript inventory;
    [Header("Audio")]
    [SerializeField] private AudioSource pickupSound;

    private void Start()
    {
        FindHomeCoreObject();
    }
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
            Debug.Log("Client: Attempting to Open HomeCore UI");
            isOpeningUI = true;
            homeCoreScript.OpenHomeCoreUI(); 
        }
        else if (Input.GetKeyDown(interactKey) && isOpeningUI)
        {
            Debug.Log("Client: Closing HomeCore UI");
            isOpeningUI = false;
            homeCoreScript.CloseHomeCoreUI(); 
        }
        if (inventory.spearCount.Value >= 1)
        {
            HaveSpear = true;
        }

        ResourcesScript.ResourceType? resourceType = IsLookingAtResource();
        if (Input.GetKeyDown(interactKey) && resourceType.HasValue)
        {
            Debug.Log($"Client: Attempting to pick up {resourceType.Value}");
            TryPickUpResourceServerRpc(currentResource.GetComponent<NetworkObject>().NetworkObjectId, resourceType.Value);
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
    private ResourcesScript.ResourceType? IsLookingAtResource()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit) && hit.distance < 3f)
        {
            ResourcesScript resourceScript = hit.collider.gameObject.GetComponent<ResourcesScript>();
            if (resourceScript)
            {
                currentResource = hit.collider.gameObject;
                Debug.Log("Raycast hit: " + hit.collider.gameObject.name);
                Debug.Log("Detected resource: " + resourceScript.resourceType);
                return resourceScript.resourceType;
            }
        }

        return new ResourcesScript.ResourceType?();
    }
    private void FindHomeCoreObject()
    {
        GameObject homeCoreObject = GameObject.FindWithTag("HomeCore"); // Make sure your HomeCore has the tag "HomeCore"
        if (homeCoreObject != null)
        {
            homeCoreScript = homeCoreObject.GetComponent<HomeCoreScript>();
            if (homeCoreScript == null)
            {
                Debug.LogError("HomeCoreScript component not found on HomeCore object!");
            }
        }
        else
        {
            Debug.LogError("HomeCore object not found in the scene!");
        }
    }
    [ServerRpc]
    void TryPickUpResourceServerRpc(ulong resourceNetworkObjectId, ResourcesScript.ResourceType resourceType, ServerRpcParams rpcParams = default)
    {
        Debug.Log($"Server: Attempting to pick up {resourceType}");
        NetworkObject targetResource = NetworkManager.Singleton.SpawnManager.SpawnedObjects[resourceNetworkObjectId];
        ResourcesScript resourceScript = targetResource.GetComponent<ResourcesScript>();

        if (resourceScript && resourceScript.CanBePickedUp())
        {
            InventoryScript inventory = GetComponent<InventoryScript>();

            if (inventory != null)
            {
                pickupSound.Play();
                resourceScript.PickUp();
                Debug.Log("Resource to pick up: " + resourceType);
                switch (resourceType)
                {
                    case ResourcesScript.ResourceType.Wood:
                        inventory.IncreaseWoodCount(resourceScript.amountPerPickup);
                        Debug.Log("Increasing inventory for: " + resourceType);
                        break;
                    case ResourcesScript.ResourceType.Food:
                        inventory.IncreaseFoodCount(resourceScript.amountPerPickup);
                        Debug.Log("Increasing inventory for: " + resourceType);
                        break;
                    case ResourcesScript.ResourceType.Water:
                        inventory.IncreaseWaterCount(resourceScript.amountPerPickup);
                        Debug.Log("Increasing inventory for: " + resourceType);
                        break;
                    case ResourcesScript.ResourceType.Cola:
                        inventory.IncreaseColaCount(resourceScript.amountPerPickup);
                        Debug.Log("Increasing inventory for: " + resourceType);
                        break;
                    case ResourcesScript.ResourceType.Rope:
                        inventory.IncreaseRopeCount(resourceScript.amountPerPickup);
                        Debug.Log("Increasing inventory for: " + resourceType);
                        break;

                }             
            }
        }
    }

}
