using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;
public class BedInteraction : NetworkBehaviour
{
    [Header("UI Components")]
    public Canvas sleepCanvas;
    public Image screenOverlay;
    public TextMeshProUGUI message;
    public float sleepTransitionTime = 1.0f;

    private HashSet<NetworkBehaviour> playersInCollider = new HashSet<NetworkBehaviour>();
    private NetworkVariable<int> dayCount = new NetworkVariable<int>(1);
    private bool isSleeping = false;

    private void Start()
    {
        sleepCanvas.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            NetworkBehaviour enteringPlayer = other.gameObject.GetComponent<NetworkBehaviour>();
            if (enteringPlayer != null)
            {
                playersInCollider.Add(enteringPlayer);
                Debug.Log($"{enteringPlayer.gameObject.name} entered. Press 'E' to sleep.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            NetworkBehaviour exitingPlayer = other.gameObject.GetComponent<NetworkBehaviour>();
            if (exitingPlayer != null)
            {
                playersInCollider.Remove(exitingPlayer);
                Debug.Log($"{exitingPlayer.gameObject.name} left the sleeping area.");
            }
        }
    }

    private void Update()
    {
        if (playersInCollider.Contains(this) && Input.GetKeyDown(KeyCode.E))
        {
            // Check if all players in the server are in the collider
            if (AreAllPlayersInCollider())
            {
                StartCoroutine(Sleep());
            }
            else
            {
                Debug.Log("Waiting for all players to be in bed.");
            }
        }
    }

    private IEnumerator Sleep()
    {
        Debug.Log("In sleep function");
        isSleeping = true;
        Color originalColor = screenOverlay.color;

        for (float transition = 0; transition < sleepTransitionTime; transition += Time.deltaTime)
        {
            screenOverlay.color = Color.Lerp(originalColor, Color.black, transition / sleepTransitionTime);
            yield return null;
        }

        AdvanceDay();

        for (float transition = 0; transition < sleepTransitionTime; transition += Time.deltaTime)
        {
            screenOverlay.color = Color.Lerp(Color.black, originalColor, transition / sleepTransitionTime);
            yield return null;
        }

        message.text = "";
        isSleeping = false;
    }

    private void AdvanceDay()
    {
        message.text = "Day " + dayCount.Value++;
        sleepCanvas.gameObject.SetActive(true); // Activating the canvas when sleeping
        StartCoroutine(DisplayDayCount());
    }

    private IEnumerator DisplayDayCount()
    {
        yield return new WaitForSeconds(2);
        message.text = "Day " + dayCount.Value;
        yield return new WaitForSeconds(2);
        sleepCanvas.gameObject.SetActive(false); // Deactivating the canvas after displaying the day
    }

    public bool AreAllPlayersInCollider()
    {
        return playersInCollider.Count == NetworkManager.Singleton.ConnectedClients.Count;
    }
}
