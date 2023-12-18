using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using System;

public class TutorialScript : NetworkBehaviour
{
    public TextMeshProUGUI TutorialText;
    public GameObject CraftingUI;
    private InteractionScript HomeUI;
    private bool isInitialised = false;
    private bool isOpenInventory = false;
    private bool isOpenCrafting = false;
    private bool isOpenHomeUI = false;

    private void Start()
    {
        if (!IsOwner) return;
        isInitialised = true;
        HomeUI = gameObject.GetComponent<InteractionScript>();
        TutorialText.gameObject.SetActive(true);
    }
    private void Update()
    {
       
        if (isInitialised == true)
        {
            TutorialText.gameObject.SetActive(true);
            StartCoroutine(TypeText("Welcome to the Island. You can press WASD to walk around. You can press SHIFT to run", 0.05f));
            StartCoroutine(DelaybeforeNextText(10f));
            isInitialised = false;
        }
        else if (isOpenInventory == false && Input.GetKeyDown(KeyCode.I))
        {
            TutorialText.gameObject.SetActive(true);
            StartCoroutine(TypeText("Welcome to Inventory UI", 0.05f));
            StartCoroutine(DelaybeforeNextText(10f));
            isOpenInventory = true;
        }
        else if (isOpenCrafting == false && CraftingUI.activeInHierarchy)
        {
            TutorialText.gameObject.SetActive(true);
            StartCoroutine(TypeText("Welcome to Crafting UI", 0.05f));
            StartCoroutine(DelaybeforeNextText(10f));
            isOpenCrafting = true;
        }
        else if (isOpenHomeUI == false && HomeUI.isOpeningHouseUI == true )
        {
            TutorialText.gameObject.SetActive(true);
            StartCoroutine(TypeText("Welcome to HomeCore UI", 0.05f));
            StartCoroutine(DelaybeforeNextText(10f));
            isOpenHomeUI = true;
        }
        




    }

    private IEnumerator DelaybeforeNextText(float time)
    {
        yield return new WaitForSeconds(time);
        TutorialText.gameObject.SetActive(false);
    }
    private IEnumerator TypeText(string text, float typingSpeed)
    {
        TutorialText.text = ""; // Start with an empty text
        foreach (char letter in text.ToCharArray())
        {
            TutorialText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(3f); // Wait for a moment after the complete text is displayed
    }
}
