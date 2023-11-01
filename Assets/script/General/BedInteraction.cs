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

    [Header("Island Spawning")]
    public IslandSpawnScript islandSpawnScript;

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
                Debug.Log($"Total players in collider: {playersInCollider.Count}");
                Debug.Log($"Total players connected: {NetworkManager.Singleton.ConnectedClients.Count}");                
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
                Debug.Log($"Total players in collider: {playersInCollider.Count}");
                Debug.Log($"Total players connected: {NetworkManager.Singleton.ConnectedClients.Count}");
                Debug.Log($"{exitingPlayer.gameObject.name} left the sleeping area.");
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            PlayerWantsToSleepServerRpc();
        }

    }

    private IEnumerator Sleep()
    {
        Debug.Log("In sleep function");
        isSleeping = true;
        AdvanceDay();      
        message.text = "";
        isSleeping = false;
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
        CheckAllPlayersInBed();
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
}
