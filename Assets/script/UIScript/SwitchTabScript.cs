using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class SwitchTabScript : NetworkBehaviour
{
    public bool isopeningMenu = false;
    public GameObject Inventory;
    public GameObject Crafting;
    public GameObject Menu;

    public void OpenInventory()
    {
        Inventory.SetActive(true);
        Crafting.SetActive(false);
        Menu.SetActive(false);
        isopeningMenu = true;
    }
    public void OpenCrafting()
    {
        Inventory.SetActive(false);
        Crafting.SetActive(true);
        Menu.SetActive(false);
        isopeningMenu = true;

    }
    public void OpenMenu()
    {
        Inventory.SetActive(false);
        Crafting.SetActive(false);
        Menu.SetActive(true);
        isopeningMenu = true;

    }
}
