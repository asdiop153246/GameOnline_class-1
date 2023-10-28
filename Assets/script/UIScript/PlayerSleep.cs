using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class PlayerSleep : NetworkBehaviour
{
    private GameManager gameManager;
    private NetworkBehaviour networkPlayer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            networkPlayer = other.gameObject.GetComponent<NetworkBehaviour>();
            gameManager = FindObjectOfType<GameManager>();
            gameManager.PlayerEnterArea(networkPlayer);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.PlayerLeaveArea(networkPlayer);
        }
    }

    void Update()
    {
        if (networkPlayer != null && networkPlayer.IsLocalPlayer && Input.GetKeyDown(KeyCode.E))
        {
            PlayerReadyServerRpc();
        }
    }

    [ServerRpc]
    void PlayerReadyServerRpc()
    {
        if (gameManager != null)
        {
            gameManager.PlayerReady(networkPlayer);
        }
    }
}
    
    
    
    
    
    


