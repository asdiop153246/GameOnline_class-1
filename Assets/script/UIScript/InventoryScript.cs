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
}
public class InventoryScript : NetworkBehaviour
{
    public MoveCamera cameraControl;
    [Header("Items")]
    public List<Item> items = new List<Item>();

    [Header("UI References")]
    public GameObject inventoryUI;
    public List<UnityEngine.UI.Image> itemImages; 
    public List<TMPro.TextMeshProUGUI> itemAmountTexts;

    public NetworkVariable<int> woodCount = new NetworkVariable<int>();
    public NetworkVariable<int> foodCount = new NetworkVariable<int>();
    public NetworkVariable<int> spearCount = new NetworkVariable<int>();

    private void Start()
    {
        if (IsOwner)  
        {
            woodCount.OnValueChanged += OnWoodCountChanged;
            spearCount.OnValueChanged += OnSpearCountChanged;
            foodCount.OnValueChanged += OnFoodCountChanged;
        }
    }
    private void Update()
    {
        if (!IsOwner)
        {
            return;
        };
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
            UpdateInventoryUI();
        }
    }

    private void ToggleInventory()
    {
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);

            if (inventoryUI.activeSelf)
            {                
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                cameraControl.canRotate = false;
            }
            else
            {               
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                cameraControl.canRotate = true;
            }
        }
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

}
