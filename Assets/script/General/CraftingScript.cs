using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro;
using Unity.Netcode;

[System.Serializable]
public class CraftingRecipe : INetworkSerializable
{
    public Item result;
    public List<Item> requiredItems;
    public float craftingTime;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        // Serialize the crafting time
        serializer.SerializeValue(ref craftingTime);

        // Serialize the result item
        result.NetworkSerialize(serializer);

        // Serialize the number of required items
        int count = requiredItems.Count;
        serializer.SerializeValue(ref count);


        if (serializer.IsReader)
        {
            requiredItems = new List<Item>(count);
        }

        for (int i = 0; i < count; i++)
        {
            Item item = serializer.IsReader ? new Item() : requiredItems[i];
            item.NetworkSerialize(serializer);
            if (serializer.IsReader)
            {
                requiredItems.Add(item);
            }
        }
    }
}

public class CraftingScript : NetworkBehaviour
{
    public InventoryScript inventoryScript; 

    [Header("Crafting Configuration")]
    public List<CraftingRecipe> craftingRecipes;

    [Header("UI References")]
    public Button craftButton;
    public Slider craftingTimerSlider;
    public TextMeshProUGUI requiredItemsText;

    private CraftingRecipe selectedRecipe; 
    private bool isCrafting = false;
    private int selectedRecipeIndex = -1;

    void Start()
    {
        Debug.Log("CraftingScript has started.");
        craftButton.onClick.AddListener(StartCrafting);
    }

    public void SelectRecipe(int recipeIndex)
    {
        Debug.Log($"SelectRecipe called with index: {recipeIndex}");
        if (recipeIndex >= 0 && recipeIndex < craftingRecipes.Count)
        {
            selectedRecipeIndex = recipeIndex;
            selectedRecipe = craftingRecipes[recipeIndex];
            UpdateRequiredItemsText(selectedRecipe);
            Debug.Log($"Recipe selected: {selectedRecipe.result.name}");
        }
        else
        {
            Debug.LogError($"Invalid recipe index: {recipeIndex}");
        }
    }
    public void StartCrafting()
    {
        Debug.Log("StartCrafting called.");
        if (selectedRecipe != null && !isCrafting && selectedRecipeIndex >= 0)
        {
            if (HasRequiredItems(selectedRecipe))
            {
                Debug.Log($"Starting crafting process for {selectedRecipe.result.name}.");
                int recipeIndex = craftingRecipes.IndexOf(selectedRecipe);
                craftingTimerSlider.maxValue = selectedRecipe.craftingTime;
                craftingTimerSlider.value = craftingTimerSlider.maxValue;
                StartCraftingServerRpc(recipeIndex);
            }
            else
            {
                Debug.LogError("Not enough required items to craft.");
            }
        }
        else
        {
            Debug.LogError("Crafting process cannot start. Either crafting is already in process or no recipe is selected.");
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void StartCraftingServerRpc(int recipeIndex, ServerRpcParams rpcParams = default)
    {
        Debug.Log($"StartCraftingServerRpc called on server with recipe index: {recipeIndex}");
        if (!isCrafting)
        {
            CraftingRecipe recipe = craftingRecipes[recipeIndex];
            StartCoroutine(CraftingProcess(recipe));
        }
        else
        {
            Debug.LogError("ServerRPC called, but is already crafting.");
        }
    }

    private IEnumerator CraftingProcess(CraftingRecipe recipe)
    {
        Debug.Log("CraftingProcess coroutine started.");
        isCrafting = true;

        UpdateCraftingStatusClientRpc(true, recipe.craftingTime);

        float time = recipe.craftingTime;
        while (time > 0)
        {
            Debug.Log($"Crafting... {time} seconds remaining.");
            time -= Time.deltaTime;
            yield return null;
        }

        Debug.Log("Crafting time completed. Deducting items and adding result to inventory.");
        foreach (var item in recipe.requiredItems)
        {
            inventoryScript.DeductItemServerServerRpc(item.name, item.amount);
        }

        inventoryScript.AddItemServerServerRpc(recipe.result.name, recipe.result.amount);

        UpdateCraftingStatusClientRpc(false, 0);

        isCrafting = false;
        Debug.Log("CraftingProcess coroutine finished.");
    }


    public void AddItem(string itemName, int amount)
    {
        
        Item itemToAdd = inventoryScript.items.Find(item => item.name == itemName);
        if (itemToAdd != null)
        {
            itemToAdd.amount += amount;            
        }      
        inventoryScript.UpdateInventoryUI();
    }
    public void DeductItem(string itemName, int amount)
    {
        
        Item itemToDeduct = inventoryScript.items.Find(item => item.name == itemName);
        if (itemToDeduct != null)
        {
            itemToDeduct.amount -= amount;

        }
       
        inventoryScript.UpdateInventoryUI();
    }
    [ClientRpc]
    private void UpdateCraftingStatusClientRpc(bool crafting, float time, ClientRpcParams rpcParams = default)
    {
        Debug.Log($"UpdateCraftingStatusClientRpc called on client with crafting: {crafting} and time: {time}");
        isCrafting = crafting;
        craftButton.interactable = !crafting;

        if (crafting)
        {
            craftingTimerSlider.gameObject.SetActive(true);
            StartCoroutine(UpdateCraftingTimer(time));
        }
        else
        {
            craftingTimerSlider.gameObject.SetActive(false);
        }
    }
    private IEnumerator UpdateCraftingTimer(float maxTime)
    {
        Debug.Log($"UpdateCraftingTimer coroutine started with maxTime: {maxTime}");
        craftingTimerSlider.maxValue = maxTime;
        craftingTimerSlider.value = 0;
        float timeElapsed = 0;

        while (timeElapsed < maxTime)
        {
            timeElapsed += Time.deltaTime;
            craftingTimerSlider.value = timeElapsed;
            Debug.Log($"Crafting timer updated: {timeElapsed}/{maxTime}");
            yield return null;
        }

        craftingTimerSlider.gameObject.SetActive(false);
        Debug.Log("UpdateCraftingTimer coroutine finished.");
    }
    private void UpdateRequiredItemsText(CraftingRecipe recipe)
    {
        string requiredItemsInfo = "Required Items:\n";
        foreach (var item in recipe.requiredItems)
        {
            requiredItemsInfo += $"{item.amount}x {item.name}\n";
        }
        requiredItemsText.text = requiredItemsInfo;
        Debug.Log($"Required items text updated: {requiredItemsInfo}");
    }
    private bool HasRequiredItems(CraftingRecipe recipe)
    {
        foreach (var item in recipe.requiredItems)
        {
            Item inventoryItem = inventoryScript.items.Find(x => x.name == item.name);
            if (inventoryItem == null || inventoryItem.amount < item.amount)
            {
                Debug.Log("You not have enough items");
                return false;
            }
        }
        
        return true;
    }
}

