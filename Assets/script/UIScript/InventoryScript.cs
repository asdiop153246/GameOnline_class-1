using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;

[System.Serializable]
public class Item
{
    public string name;
    public Sprite icon;
    public int amount;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        // Implement the serialization logic for each field
        // For example, serializing a string:
        serializer.SerializeValue(ref name);
        // For Sprite, you might want to serialize a reference ID or path instead
        // Since you cannot directly serialize a Sprite object
        // ...
        serializer.SerializeValue(ref amount);
    }
}
public class InventoryScript : NetworkBehaviour
{
    public MoveCamera cameraControl;
    [Header("Items")]
    public List<Item> items = new List<Item>();

    [Header("UI References")]
    public GameObject inventoryUI;
    public GameObject CraftingUI;
    public GameObject MenuUI;
    public List<UnityEngine.UI.Image> itemImages; 
    public List<TMPro.TextMeshProUGUI> itemAmountTexts;
    public GameObject MenuSelectorUI;
    private bool IsOpeningUI = false;

    public NetworkVariable<int> woodCount = new NetworkVariable<int>();
    public NetworkVariable<int> foodCount = new NetworkVariable<int>();
    public NetworkVariable<int> spearCount = new NetworkVariable<int>();
    public NetworkVariable<int> waterCount = new NetworkVariable<int>();
    public NetworkVariable<int> colaCount = new NetworkVariable<int>();
    public NetworkVariable<int> ropeCount = new NetworkVariable<int>();


    private void Start()
    {
        if (IsOwner)  
        {
            woodCount.OnValueChanged += OnWoodCountChanged;
            spearCount.OnValueChanged += OnSpearCountChanged;
            foodCount.OnValueChanged += OnFoodCountChanged;
            waterCount.OnValueChanged += OnWaterCountChanged;
            colaCount.OnValueChanged += OnColaCountChanged;
            ropeCount.OnValueChanged += OnRopeCountChanged;
        }
    }
    private void Update()
    {
        if (!IsOwner)
        {
            return;
        };
        if (Input.GetKeyDown(KeyCode.I) && IsOpeningUI == false)
        {
            ToggleInventory();
            UpdateInventoryUI();
        }
        else if(IsOpeningUI == true && Input.GetKeyDown(KeyCode.I))
        {
            inventoryUI.SetActive(false);
            CraftingUI.SetActive(false);
            MenuUI.SetActive(false);
            MenuSelectorUI.SetActive(false);
            IsOpeningUI = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            cameraControl.canRotate = true;
        }
    }

    private void ToggleInventory()
    {
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
            MenuSelectorUI.SetActive(!MenuSelectorUI.activeSelf);
            if (inventoryUI.activeSelf)
            {                
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                cameraControl.canRotate = false;
                IsOpeningUI = true;
            }
            else
            {               
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                cameraControl.canRotate = true;
                
            }
        }
    }
    public void OpenInventory()
    {
        inventoryUI.SetActive(true);
        CraftingUI.SetActive(false);
        MenuUI.SetActive(false);
        IsOpeningUI = true;
    }
    public void OpenCrafting()
    {
        inventoryUI.SetActive(false);
        CraftingUI.SetActive(true);
        MenuUI.SetActive(false);
        IsOpeningUI = true;
    }
    public void OpenMenu()
    {
        inventoryUI.SetActive(false);
        CraftingUI.SetActive(false);
        MenuUI.SetActive(true);
        IsOpeningUI = true;
    }
    public void closeMenu()
    {
        inventoryUI.SetActive(false);
        CraftingUI.SetActive(false);
        MenuUI.SetActive(false);
        MenuSelectorUI.SetActive(false);
        IsOpeningUI = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraControl.canRotate = true;
    }
    public void UpdateInventoryUI()
    {
        if (!IsOwner)
            return;
        Debug.Log("Updating inventory UI for client: " + NetworkManager.Singleton.LocalClientId);
        for (int i = 0; i < items.Count; i++)
        {
            if (i >= itemImages.Count)
            {
                Debug.LogWarning("Not enough item images to match item count!");
                break;
            }
            var currentItem = items[i];
            var currentImage = itemImages[i];
            currentImage.sprite = currentItem.icon;

            if (items[i].amount > 0)
            {
                
                itemAmountTexts[i].text = items[i].amount.ToString();
                itemAmountTexts[i].gameObject.SetActive(true);

                var tempColor = itemImages[i].color;
                
                tempColor = new Color32(255, 255, 255, 255);
                itemImages[i].color = tempColor;
            }
            else
            {
                
                itemAmountTexts[i].gameObject.SetActive(false);

                var tempColor = itemImages[i].color;
                
                tempColor = new Color32(255, 255, 255, 20);
                itemImages[i].color = tempColor;
            }

            
            itemImages[i].gameObject.SetActive(items[i].icon != null);
        }
    }
    private void OnWoodCountChanged(int oldCount, int newCount)
    {
        if (items[0].name == "Wood")
        {
            items[0].amount = newCount;
        }
        UpdateInventoryUI();
    }
    private void OnSpearCountChanged(int oldCount, int newCount)
    {
        if (items[6].name == "Spear")
        {
            items[6].amount = newCount;
        }
        UpdateInventoryUI();
    }
    private void OnFoodCountChanged(int oldCount, int newCount)
    {
        if (items[3].name == "Food")
        {
            items[3].amount += newCount;
            
        }
        UpdateInventoryUI();
    }
    private void OnWaterCountChanged(int oldCount, int newCount)
    {
        if (items[4].name == "Water")
        {
            items[4].amount += newCount;

        }
        UpdateInventoryUI();
    }
    private void OnColaCountChanged(int oldCount, int newCount)
    {
        if (items[5].name == "Cola")
        {
            items[5].amount += newCount;

        }
        UpdateInventoryUI();
    }
    private void OnRopeCountChanged(int oldCount, int newCount)
    {
        if (items[1].name == "Rope")
        {
            items[1].amount += newCount;

        }
        UpdateInventoryUI();
    }
    public void IncreaseWoodCount(int amount)
    {
        woodCount.Value += amount;
    }
    public void IncreaseFoodCount(int amount)
    {
        foodCount.Value += amount;
    }
    public void IncreaseSpearCount(int amount)
    {
        spearCount.Value += amount;
    }
    public void IncreaseWaterCount(int amount)
    {
        waterCount.Value += amount;
    }
    public void IncreaseColaCount(int amount)
    {
        colaCount.Value += amount;
    }
    public void IncreaseRopeCount(int amount)
    {
        ropeCount.Value += amount;
    }

    [ServerRpc]
    public void AddItemServerServerRpc(string itemName, int amount, ServerRpcParams rpcParams = default)
    {
        var item = items.FirstOrDefault(i => i.name == itemName);
        if (item != null)
        {
            item.amount += amount;

            // Update the corresponding NetworkVariable
            UpdateItemCount(itemName, item.amount);
        }
    }

    [ServerRpc]
    public void DeductItemServerServerRpc(string itemName, int amount, ServerRpcParams rpcParams = default)
    {
        // Find the item in the list and deduct the amount
        var item = items.FirstOrDefault(i => i.name == itemName);
        if (item != null && item.amount >= amount) // Ensure we have enough to deduct
        {
            item.amount -= amount;

            
            UpdateItemCount(itemName, item.amount);
        }
    }
    private void UpdateItemCount(string itemName, int newAmount)
    {
        switch (itemName)
        {
            case "Wood":
                woodCount.Value = newAmount;
                break;
            case "Food":
                foodCount.Value = newAmount;
                break;
            case "Spear":
                spearCount.Value = newAmount;
                break;
            case "Water":
                waterCount.Value = newAmount;
                break;
            case "Cola":
                colaCount.Value = newAmount;
                break;
            case "Rope":
                ropeCount.Value = newAmount;
                break;
            default:
                Debug.LogWarning($"Item {itemName} does not have a corresponding NetworkVariable.");
                break;
        }
    }
}
