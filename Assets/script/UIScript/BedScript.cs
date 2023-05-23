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
    public TextMeshProUGUI instructionMessage;  // New instruction text variable
    public float sleepTransitionTime = 1.0f;
    public BoxCollider bedCollider;

    private NetworkVariable<int> sleepingClients = new NetworkVariable<int>(0);
    private NetworkVariable<int> dayCount = new NetworkVariable<int>(1);
    private bool isNearBed = false;

    // This is the ServerRpc method that clients will call when they start sleeping
    [ServerRpc]
    public void StartSleepingServerRpc(ServerRpcParams rpcParams = default)
    {
        sleepingClients.Value++;

        // If all clients are sleeping, start the sleep process
        if (sleepingClients.Value >= NetworkManager.Singleton.ConnectedClients.Count)
        {
            StartCoroutine(Sleep());
        }
    }

    [ClientRpc]
    public void SleepClientRpc(ClientRpcParams rpcParams = default)
    {
        StartCoroutine(Sleep());
    }

    private void Update()
    {
        // Only local player can trigger the sleep action
        if (!IsLocalPlayer) return;

        // Check for the 'E' key press to start sleeping, but only if the player is near the bed
        if (isNearBed && Input.GetKeyDown(KeyCode.E))
        {
            StartSleepingServerRpc();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        // Check if it's a player that entered the bed's collider
        if (other.gameObject.CompareTag("Player") && other.GetComponent<NetworkObject>().IsLocalPlayer)
        {
            isNearBed = true;
            instructionMessage.text = "Press E to sleep"; // Update instruction
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if it's a player that left the bed's collider
        if (other.gameObject.CompareTag("Player") && other.GetComponent<NetworkObject>().IsLocalPlayer)
        {
            isNearBed = false;
            instructionMessage.text = ""; // Clear instruction
        }
    }

    IEnumerator Sleep()
    {
        // Display the current day first
        message.text = "Day " + dayCount.Value;

        // Gradually darken the screen
        float transition = 0;
        while (transition < sleepTransitionTime)
        {
            screenOverlay.color = new Color(0, 0, 0, transition / sleepTransitionTime);
            transition += Time.deltaTime;
            yield return null;
        }

        // Wait for a moment
        yield return new WaitForSeconds(2);

        if (IsServer)
        {
            // Increase the day count and display the new day
            dayCount.Value++;
        }

        message.text = "Next Day: " + dayCount.Value;

        // Wait for a moment with the new day message visible
        yield return new WaitForSeconds(2);

        // Gradually lighten the screen
        transition = 0;
        while (transition < sleepTransitionTime)
        {
            screenOverlay.color = new Color(0, 0, 0, 1 - (transition / sleepTransitionTime));
            transition += Time.deltaTime;
            yield return null;
        }

        // Hide the message
        message.text = "";

        if (IsServer)
        {
            // Reset sleeping clients count
            sleepingClients.Value = 0;
        }
    }
}

