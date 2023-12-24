using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerSpawnScript : NetworkBehaviour
{
    public Behaviour[] scripts;
    private Renderer[] renderers;
    int spawnedPoint = 2;
    LoginManagerScript loginManager;
    private PlayerHealth playerHealth;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        loginManager = GameObject.FindGameObjectWithTag("LoginManager").GetComponent<LoginManagerScript>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void SetPlayerState(bool state)
    {
        foreach (var script in scripts) { script.enabled = state; }
        foreach (var renderer in renderers) { renderer.enabled = state; }      
    }
    private Vector3 GetRandPos()
    {
        Vector3 randPos;
        int randomNumber = Random.Range(0, 2);
        spawnedPoint = randomNumber;
        randPos = new Vector3(loginManager.SpawnPoints[spawnedPoint].transform.position.x, loginManager.SpawnPoints[spawnedPoint].transform.position.y, loginManager.SpawnPoints[spawnedPoint].transform.position.z);
        return randPos;
    }

    //1 Client Send to Server
    public void Respawn()
    {
        Debug.Log("Client requesting respawn");
        RespawnServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    //2 Server Send to Client (Run on server)
    [ServerRpc(RequireOwnership = false)]
    private void RespawnServerRpc(ulong clientId)
    {
        Debug.Log("Server processing respawn");
        Vector3 spawnPos = GetRandPos();
        RespawnClientRpc(spawnPos, clientId);

        // Update the server-side position and health
        transform.position = spawnPos;
        playerHealth.Health.Value = playerHealth.maxHealth.Value;
    }

    //3 Client Set player
    [ClientRpc]
    private void RespawnClientRpc(Vector3 spawnPos, ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId != clientId) return;

        Debug.Log("Client received respawn RPC with position: " + spawnPos);
        StartCoroutine(RespawnCoroutine(spawnPos));
    }
    IEnumerator RespawnCoroutine(Vector3 spawnPos)
    {
        Debug.Log("Starting respawn coroutine with position: " + spawnPos);
        SetPlayerState(false);
        transform.position = spawnPos;
        yield return new WaitForSeconds(3);
        SetPlayerState(true);
        playerHealth.Health.Value = playerHealth.maxHealth.Value;
        playerHealth.UpdateHealthUI();
    }
}

