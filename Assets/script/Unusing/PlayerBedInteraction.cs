using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;

public class PlayerBedInteraction : NetworkBehaviour
{
    [Header("UI Components")]
    public Image screenOverlay;
    public TextMeshProUGUI message;
    public float sleepTransitionTime = 1.0f;

    [Header("Networking")]
    private GameManager gameManager;
    private HashSet<NetworkBehaviour> playersInCollider = new HashSet<NetworkBehaviour>();
    private NetworkVariable<int> dayCount = new NetworkVariable<int>(1);

    private bool isSleeping = false;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            NetworkBehaviour enteringPlayer = other.gameObject.GetComponent<NetworkBehaviour>();
            if (enteringPlayer != null)
            {
                playersInCollider.Add(enteringPlayer);
                gameManager.PlayerEnterArea(enteringPlayer);
                Debug.Log($"{enteringPlayer.gameObject.name} has entered. Total players in collider: {playersInCollider.Count}");
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
                gameManager.PlayerLeaveArea(exitingPlayer);
                Debug.Log($"{exitingPlayer.gameObject.name} has exited. Total players in collider: {playersInCollider.Count}");
            }
        }
    }

    private void Update()
    {
        if (isSleeping)
            return;
        //Debug.Log($"IsLocalPlayer: {IsLocalPlayer}, isSleeping: {isSleeping}, Is in Collider: {playersInCollider.Contains(this)}");
        if (playersInCollider.Contains(this) && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"IsLocalPlayer: {IsLocalPlayer}, isSleeping: {isSleeping}, Is in Collider: {playersInCollider.Contains(this)}");
            Debug.Log("Player pressed E");
            PlayerReadyServerRpc();
            StartCoroutine(Sleep());
        }
    }

    [ServerRpc]
    void PlayerReadyServerRpc()
    {
        if (gameManager != null)
        {
            gameManager.PlayerReady(this);
        }
    }

    private IEnumerator Sleep()
    {
        Debug.Log("In sleep fucntion");
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
        // The '++' will increment dayCount after it's used
        StartCoroutine(DisplayDayCount());
    }

    private IEnumerator DisplayDayCount()
    {
        yield return new WaitForSeconds(2);
        message.text = "Day " + dayCount.Value;
        yield return new WaitForSeconds(2);
    }

    public bool AreAllPlayersInCollider()
    {
        return playersInCollider.Count == NetworkManager.Singleton.ConnectedClients.Count;
    }
}

