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
    public bool isOpeningHouseUI = false;
    public bool isOpeningOtherUI = false;
    public bool isLookingAtSomethingInteractable = false;
    public GameObject TextUI;
    public TextMeshProUGUI resourcesCollectedText;
    public TextMeshProUGUI notificationText;
    public TextMeshProUGUI sleepText;
    public GameObject notificationPanel;
    public MoveCamera cameraControl;
    public PlayerControllerScript playerMovement;
    private HomeCoreScript homeCoreScript;
    private OtherCoreScript CoreScript;
    public GameObject OtherCoreObject;
    private CoreUIManager CoreUIScript;
    public InventoryScript inventory;
    [Header("Audio")]
    [SerializeField] private AudioSource pickupSound;

    
    private void Start()
    {
        if (!IsOwner) return;
        FindHomeCoreObject();
    }
    
    private void Update()
    {
        FindCoreObject();
        TextUI.gameObject.SetActive(true);
        bool isLookingAtSomethingInteractable = false;
        if (Input.GetKeyDown(interactKey) && !HaveSpear && IsLookingAtSpear())
        {
            Debug.Log("Client: Attempting to pick up spear");
            NetworkObject spearNetworkObject = Pspear.GetComponent<NetworkObject>();
            if (spearNetworkObject)
            {
                TryPickUpSpearServerRpc(spearNetworkObject.NetworkObjectId);
            }
        }
        if (Input.GetKeyDown(interactKey) && !isOpeningHouseUI && IsLookingAtHomeCore())
        {
            Debug.Log("Client: Attempting to Open HomeCore UI");
            isOpeningHouseUI = true;
            playerMovement.canMove = false;
            cameraControl.canRotate = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            homeCoreScript.OpenHomeCoreUI(); 
        }      
        else if (Input.GetKeyDown(interactKey) && !isOpeningOtherUI && IsLookingAtCore())
        {
            Debug.Log("Client: Attempting to Open OtherCore UI");
            isOpeningOtherUI = true;
            CoreScript.OpenCoreUI();
            playerMovement.canMove = false;
            cameraControl.canRotate = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            

        }
        else if (Input.GetKeyDown(interactKey) && isOpeningHouseUI)
        {
            Debug.Log("Client: Closing HomeCore UI");
            isOpeningHouseUI = false;
            playerMovement.canMove = true;
            cameraControl.canRotate = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            homeCoreScript.CloseHomeCoreUI();
            
        }
        else if (Input.GetKeyDown(interactKey) && isOpeningOtherUI)
        {
            Debug.Log("Client: Closing HomeCore UI");
            isOpeningOtherUI = false;
            playerMovement.canMove = true;
            cameraControl.canRotate = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            CoreScript.CloseCoreUI();

        }
        if (inventory.spearCount.Value >= 1)
        {
            HaveSpear = true;
        }
        if (IsLookingAtHomeCore() || IsLookingAtCore() || IsLookingAtResource().HasValue)
        {
            ShowInteractionNotification("Press E to interact");
            isLookingAtSomethingInteractable = true;
        }
       
        if (!isLookingAtSomethingInteractable)
        {
            HideInteractionNotification();
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
                //Debug.Log("Looking at Spear");
                return true;
            }
        }

        return false;
    }
    private bool IsLookingAtHomeCore()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 3f))
        {
            if (hit.collider.CompareTag("HomeCore"))
            {
                HomeCore = hit.collider.gameObject;
                //Debug.Log("Ray Hit HomeCore");
                return true;
            }
        }
        return false;
    }
    private bool IsLookingAtCore()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 3f))
        {
            if (hit.collider.CompareTag("OtherCore"))
            {
                HomeCore = hit.collider.gameObject;
                //Debug.Log("Ray Hit Core");
                return true;
            }
        }
        return false;
    }
    private ResourcesScript.ResourceType? IsLookingAtResource()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 3f))
        {
            ResourcesScript resourceScript = hit.collider.gameObject.GetComponent<ResourcesScript>();
            if (resourceScript)
            {
                currentResource = hit.collider.gameObject;
                return resourceScript.resourceType;
            }
        }
        return null;
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
    private void FindCoreObject()
    {
        GameObject CoreObjectScript = GameObject.FindWithTag("OtherCoreManager");
        if (CoreObjectScript != null)
        {
            CoreScript = CoreObjectScript.GetComponent<OtherCoreScript>();
            if (CoreScript == null)
            {
                Debug.LogError("CoreScript component not found on Core object Manager!");
            }
        }
        else
        {
            Debug.LogError("Core object not found in the scene!");
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
                string message = $"Picked up {resourceScript.amountPerPickup} {resourceType}";
                float displayTime = 2.0f;
                ShowTemporaryMessageClientRpc(message, displayTime, rpcParams.Receive.SenderClientId);
            }
        }
    }
    private void ShowInteractionNotification(string message)
    {
        if (notificationPanel.activeSelf == false)
        {
            notificationPanel.SetActive(true);
            notificationText.text = message;
        }
    }
    private void HideInteractionNotification()
    {
        if (notificationPanel.activeSelf == true)
        {
            notificationPanel.SetActive(false);
        }
    }
    [ClientRpc]
    void ShowTemporaryMessageClientRpc(string message, float displayTime, ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            StartCoroutine(ShowTemporaryMessage(message, displayTime));
        }
    }
    private IEnumerator ShowTemporaryMessage(string message, float displayTime)
    {
        resourcesCollectedText.text = message;
        resourcesCollectedText.gameObject.SetActive(true);
        yield return new WaitForSeconds(displayTime);
        resourcesCollectedText.gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bed"))
        {
            sleepText.gameObject.SetActive(true);
            sleepText.text = "Press E to Sleep";
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bed"))
        {
            sleepText.gameObject.SetActive(false);
            sleepText.text = "";
        }
    }
}
