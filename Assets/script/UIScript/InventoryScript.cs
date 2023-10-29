using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


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
    private void Update()
    {
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
        for (int i = 0; i < items.Count; i++)
        {
            
            itemImages[i].sprite = items[i].icon;

            if (items[i].amount > 0)
            {
                
                itemAmountTexts[i].text = items[i].amount.ToString();
                itemAmountTexts[i].gameObject.SetActive(true);

                var tempColor = itemImages[i].color;
                //tempColor.a = 1f;
                tempColor = new Color32(255, 255, 255, 255);
                itemImages[i].color = tempColor;
            }
            else
            {
                
                itemAmountTexts[i].gameObject.SetActive(false);

                var tempColor = itemImages[i].color;
                //tempColor.a = 0.3f;
                tempColor = new Color32(255, 255, 255, 20);
                itemImages[i].color = tempColor;
            }

            
            itemImages[i].gameObject.SetActive(items[i].icon != null);
        }
    }
    public void IncreaseWoodCount(int amount)
    {
        
        if (items[0].name == "Wood")
        {
            items[0].amount += amount;
            UpdateInventoryUI();
        }
    }
    public void IncreaseFoodCount(int amount)
    {

        if (items[3].name == "Food")
        {
            items[3].amount += amount;
            UpdateInventoryUI();
        }
    }
    public void IncreaseSpearCount(int amount)
    {

        if (items[6].name == "Spear")
        {
            items[6].amount += amount;
            UpdateInventoryUI();
        }
    }

}
