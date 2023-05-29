using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class BedScript : NetworkBehaviour
{
    public Image screenOverlay;
    public TextMeshProUGUI message;
    public float sleepTransitionTime = 1.0f;

    private bool isPlayerNearBed = false;
    private bool isSleeping = false;
    private int dayCount = 1;

    private void Update()
    {
        if (!IsLocalPlayer)
            return;

        if (isPlayerNearBed && Input.GetKeyDown(KeyCode.E) && !isSleeping)
        {
            StartCoroutine(Sleep());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsLocalPlayer)
            return;

        if (other.CompareTag("Player"))
        {
            isPlayerNearBed = true;
            // Display instruction to sleep
            // For example, you can enable a UI element or show a message
            Debug.Log("Press 'E' to sleep");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsLocalPlayer)
            return;

        if (other.CompareTag("Player"))
        {
            isPlayerNearBed = false;
            // Hide the instruction to sleep
            // For example, you can disable a UI element or clear the message
            Debug.Log("You moved away from the bed");
        }
    }

    private IEnumerator Sleep()
    {
        // Set sleeping state to prevent multiple sleep sequences
        isSleeping = true;

        // Gradually darken the screen
        float transition = 0;
        while (transition < sleepTransitionTime)
        {
            screenOverlay.color = new Color(0, 0, 0, transition / sleepTransitionTime);
            transition += Time.deltaTime;
            yield return null;
        }

        // Wait for a moment (e.g., display the current day)
        message.text = "Day " + dayCount;
        yield return new WaitForSeconds(2);

        // Increase the day count
        dayCount++;

        // Wait for a moment (e.g., display the new day)
        message.text = "Day " + dayCount;
        yield return new WaitForSeconds(2);

        // Gradually lighten the screen
        transition = 0;
        while (transition < sleepTransitionTime)
        {
            screenOverlay.color = new Color(0, 0, 0, 1 - (transition / sleepTransitionTime));
            transition += Time.deltaTime;
            yield return null;
        }

        // Hide the message and reset sleeping state
        message.text = "";
        isSleeping = false;
    }
}
