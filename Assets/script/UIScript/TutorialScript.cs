using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using System.Threading;

public class TutorialScript : NetworkBehaviour
{
    public TextMeshProUGUI TutorialText;
    public GameObject CraftingUI;
    private InteractionScript HomeUI;
    private bool isInitialised = false;
    private bool isOpenInventory = false;
    private bool isOpenCrafting = false;
    private bool isOpenHomeUI = false;
    private CancellationTokenSource typingCancellationTokenSource;

    private void Start()
    {
        if (!IsOwner) return;
        isInitialised = true;
        HomeUI = gameObject.GetComponent<InteractionScript>();
        TutorialText.gameObject.SetActive(true);
        typingCancellationTokenSource = new CancellationTokenSource();
    }
    private void Update()
    {

        if (isInitialised)
        {
            StartTypingText("Welcome to the Island. You can press WASD to walk around. You can press SHIFT to run", 0.05f);
            isInitialised = false;
        }
        else if (!isOpenInventory && Input.GetKeyDown(KeyCode.Tab))
        {
            StartTypingText("Welcome to Inventory UI", 0.05f);
            isOpenInventory = true;
        }
        else if (!isOpenCrafting && CraftingUI.activeInHierarchy)
        {
            StartTypingText("Welcome to Crafting UI", 0.05f);
            isOpenCrafting = true;
        }
        else if (!isOpenHomeUI && HomeUI.isOpeningHouseUI)
        {
            StartTypingText("Welcome to HomeCore UI", 0.05f);
            isOpenHomeUI = true;
        }





    }

    private IEnumerator DelaybeforeNextText(float time)
    {
        yield return new WaitForSeconds(time);
        TutorialText.gameObject.SetActive(false);
    }
    private void StartTypingText(string text, float typingSpeed)
    {
        typingCancellationTokenSource.Cancel(); // Cancel any ongoing typing
        typingCancellationTokenSource = new CancellationTokenSource(); // Create a new token source
        StartCoroutine(TypeText(text, typingSpeed, typingCancellationTokenSource.Token));
    }
    private IEnumerator TypeText(string text, float typingSpeed, CancellationToken token)
    {
        TutorialText.text = ""; // Start with an empty text
        foreach (char letter in text.ToCharArray())
        {
            if (token.IsCancellationRequested)
            {
                break; // Stop typing if cancellation is requested
            }
            TutorialText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        if (!token.IsCancellationRequested)
        {
            yield return new WaitForSeconds(3f); // Wait for a moment after the complete text is displayed
            TutorialText.gameObject.SetActive(false);
        }
    }
}
