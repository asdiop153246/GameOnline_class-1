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
    
    
    
    
    
    
    //int days = 1;
    //bool isNearBed = false;
    //public GameObject sleepCanvas; // Assign in inspector
    //public TextMeshProUGUI sleepText; // Assign in inspector

    //void Update()
    //{
    //    if (isNearBed && Input.GetKeyDown(KeyCode.E))
    //    {
    //        StartCoroutine(Sleep());
    //    }
    //}

    //private IEnumerator Sleep()
    //{
    //    sleepCanvas.SetActive(true);
    //    sleepText.text = "The current day is day" +" "+ days;
    //    yield return new WaitForSeconds(5);
    //    sleepCanvas.SetActive(false);
    //    sleepText.text = "";
    //    days++;
    //}

    //void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.CompareTag("Player"))
    //    {
    //        Debug.Log("Player Enter Bed");
    //        isNearBed = true;
    //    }
    //}

    //void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject.CompareTag("Player"))
    //    {
    //        isNearBed = false;
    //    }
    //}

