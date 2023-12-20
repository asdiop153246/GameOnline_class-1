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
    public TextMeshProUGUI PlayerText;
    public float sleepTransitionTime = 1.0f;

    [Header("Island Spawning")]
    public IslandSpawnScript islandSpawnScript;

    private HashSet<NetworkBehaviour> playersInCollider = new HashSet<NetworkBehaviour>();
    public NetworkVariable<int> dayCount = new NetworkVariable<int>(1);
    public NetworkedDayNightCycle dayNightCycle;
    private bool canPressKey = false;
    //private bool isSleeping = false;


    private void Start()
    {
        sleepCanvas.gameObject.SetActive(false);
        PlayerText.text = "";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {           
            NetworkBehaviour enteringPlayer = other.gameObject.GetComponent<NetworkBehaviour>();
            if (enteringPlayer != null)
            {
                playersInCollider.Add(enteringPlayer);
                Debug.Log($"Total players in collider: {playersInCollider.Count}");
                Debug.Log($"Total players connected: {NetworkManager.Singleton.ConnectedClients.Count}");
                message.text = "Press E to sleep";
                Debug.Log($"{enteringPlayer.gameObject.name} entered. Press 'E' to sleep.");
                canPressKey = true;
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
                Debug.Log($"Total players in collider: {playersInCollider.Count}");
                Debug.Log($"Total players connected: {NetworkManager.Singleton.ConnectedClients.Count}");
                Debug.Log($"{exitingPlayer.gameObject.name} left the sleeping area.");
            }
            if (playersInCollider.Count == 0)
            {
                canPressKey = false;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canPressKey == true)
        {
            PlayerWantsToSleepServerRpc();
        }
        else if (dayNightCycle.isDayTime())
        {
            Debug.Log("It's day time advance day please");
            AdvanceDay();
        }
    }

    private IEnumerator Sleep()
    {
        Debug.Log("In sleep function");

        // Set the time to just after night begins
        dayNightCycle.SetTimeToMorning();

        AdvanceDay();
        message.text = "";
        islandSpawnScript.SpawnIsland();
        yield return null;
    }

    private void AdvanceDay()
    {
        IncrementDayServerRpc();
    }

    private IEnumerator DisplayDayCount()
    {
        yield return new WaitForSeconds(2);
        message.text = "Day " + dayCount.Value;
        yield return new WaitForSeconds(2);
        sleepCanvas.gameObject.SetActive(false); 
    }

    public bool AreAllPlayersInCollider()
    {
        return playersInCollider.Count == NetworkManager.Singleton.ConnectedClients.Count;
    }

    private void CheckAllPlayersInBed()
    {
        if (!IsServer) return;
        if (AreAllPlayersInCollider())
        {
            Debug.Log("Everyone in Bed area");
            InitiateSleepForAllClientRpc();
        }
        else
        {
            StartCoroutine(textDelayBeforeDisappear("Waiting for all players to be in bed."));
            Debug.Log("Waiting for all players to be in bed.");
        }
    }
    [ClientRpc]
    private void InitiateSleepForAllClientRpc()
    {
        StartCoroutine(Sleep());
    }
    [ServerRpc(RequireOwnership = false)]
    private void PlayerWantsToSleepServerRpc()
    {
        Debug.Log("E key pressed by " + gameObject.name);

        if (IsServer && dayNightCycle != null && dayNightCycle.IsNightTime() && dayNightCycle.monstersSpawned == false)
        {
            CheckAllPlayersInBed();
        }
        else
        {
            StartCoroutine(textDelayBeforeDisappear("You can only sleep at night."));
            Debug.Log("You can only sleep at night.");
            InformPlayerItIsNotNightClientRpc();
        }
    }

    private IEnumerator textDelayBeforeDisappear(string text)
    {
        PlayerText.text = text;
        yield return new WaitForSeconds(2f);
        PlayerText.text = "";
    }
    [ServerRpc]
    private void IncrementDayServerRpc()
    {
        dayCount.Value++;
        UpdateDayCountClientRpc(dayCount.Value);
    }

    [ClientRpc]
    private void UpdateDayCountClientRpc(int newDay)
    {
        message.text = "Day " + newDay;
        sleepCanvas.gameObject.SetActive(true);
        StartCoroutine(DisplayDayCount());
    }
    [ClientRpc]
    private void InformPlayerItIsNotNightClientRpc()
    {
        message.text = "You can only sleep at night.";
    }
}
