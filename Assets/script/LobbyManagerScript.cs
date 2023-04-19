using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFSW.QC;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

public class LobbyManagerScript : MonoBehaviour
{
    private Lobby hostLobby;
    [Command]
    private async void CreateLobby()
    {
        try
        {
            string lobbyName = "MyLobby";
            int maxPlayer = 5;
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayer);
            hostLobby = lobby;
            StartCoroutine(HeartBeatLobby(hostLobby.Id, 15));
            Debug.Log("Lobby is created : " + lobby.Name + " : " + lobby.MaxPlayers);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private static IEnumerator HeartBeatLobby(string lobbyId, float waitTime)
    {
        var delay = new WaitForSeconds(waitTime);
        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }

    [Command]
    private async void ListLobbies()
    {
        try
        {
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync();
            Debug.Log("Lobbies found : " + queryResponse.Results.Count);
            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log("Lobby : " + lobby.Name + " , MaxPlayer = " + lobby.MaxPlayers);
            }
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
}
