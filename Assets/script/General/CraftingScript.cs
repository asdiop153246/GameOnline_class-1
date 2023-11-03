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

        // Ensure capacity in the list
        if (serializer.IsReader)
        {
            requiredItems = new List<Item>(count);
        }

        // Serialize each item
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
    public InventoryScript inventoryScript; // Reference to the InventoryScript

    [Header("Crafting Configuration")]
    public List<CraftingRecipe> craftingRecipes; // List of possible recipes

    [Header("UI References")]
    public TMP_Dropdown craftingDropdown; // Dropdown to select the item to craft
    public Button craftButton; // Button to start crafting
    public TextMeshProUGUI craftingTimerText; // Text to display the crafting timer

    private CraftingRecipe selectedRecipe; 
    private bool isCrafting = false;
    private int selectedRecipeIndex = -1;

    void Start()
    {
        craftButton.onClick.AddListener(StartCrafting);
    }

    public void SelectRecipe(int recipeIndex)
    {
        if (recipeIndex >= 0 && recipeIndex < craftingRecipes.Count)
        {
            selectedRecipeIndex = recipeIndex;            
            selectedRecipe = craftingRecipes[recipeIndex];
        }
    }
    public void StartCrafting()
    {
        if (selectedRecipe != null && !isCrafting && selectedRecipeIndex >= 0)
        {
            int recipeIndex = craftingRecipes.IndexOf(selectedRecipe); // Get the index of the selected recipe
            if (recipeIndex >= 0)
            {
                StartCraftingServerRpc(recipeIndex); // Call the RPC with the index
            }
            else
            {
                Debug.LogError("Selected recipe is not in the list!");
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void StartCraftingServerRpc(int recipeIndex, ServerRpcParams rpcParams = default)
    {
        if (!isCrafting)
        {
            // Look up the recipe by index or some other key
            CraftingRecipe recipe = craftingRecipes[recipeIndex];
            StartCoroutine(CraftingProcess(recipe));
        }
    }

    private IEnumerator CraftingProcess(CraftingRecipe recipe)
    {
        isCrafting = true;

        // Inform all clients that crafting has started
        UpdateCraftingStatusClientRpc(true, recipe.craftingTime);

        // Start crafting timer
        float time = recipe.craftingTime;
        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        // Deduct resources and add the crafted item on the server
        foreach (var item in recipe.requiredItems)
        {
            inventoryScript.DeductItemServerServerRpc(item.name, item.amount);
        }

        inventoryScript.AddItemServerServerRpc(recipe.result.name, recipe.result.amount);

        // Inform all clients that crafting has finished
        UpdateCraftingStatusClientRpc(false, 0);

        isCrafting = false;
    }

    // Add item to the inventory and update the network variables
    public void AddItem(string itemName, int amount)
    {
        // Find the item in the inventory
        Item itemToAdd = inventoryScript.items.Find(item => item.name == itemName);
        if (itemToAdd != null)
        {
            itemToAdd.amount += amount;
            // Update the network variable here, similar to your Increase methods
            // For example: inventoryScript.IncreaseWoodCount(amount);
        }      
        inventoryScript.UpdateInventoryUI();
    }
    public void DeductItem(string itemName, int amount)
    {
        // Find the item in the inventory
        Item itemToDeduct = inventoryScript.items.Find(item => item.name == itemName);
        if (itemToDeduct != null)
        {
            itemToDeduct.amount -= amount;
            // Update the network variable here, similar to your Increase methods but deducting
            // For example: inventoryScript.DecreaseWoodCount(amount);
        }
        // Update UI
        inventoryScript.UpdateInventoryUI();
    }
    [ClientRpc]
    private void UpdateCraftingStatusClientRpc(bool crafting, float time, ClientRpcParams rpcParams = default)
    {
        isCrafting = crafting;
        craftButton.interactable = !crafting; // Disable the craft button during crafting

        if (crafting)
        {
            // Start updating the timer on all clients
            StartCoroutine(UpdateCraftingTimer(time));
        }
        else
        {
            craftingTimerText.text = ""; // Clear the crafting timer
        }
    }
    private IEnumerator UpdateCraftingTimer(float time)
    {
        while (time > 0)
        {
            time -= Time.deltaTime;
            craftingTimerText.text = "Crafting: " + time.ToString("F2") + "s";
            yield return null;
        }
        craftingTimerText.text = ""; // Clear the crafting timer once done
    }
}

