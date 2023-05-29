using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public GameObject sleepCanvas;
    public TextMeshProUGUI sleepText;
    private Dictionary<NetworkBehaviour, bool> playersInArea = new Dictionary<NetworkBehaviour, bool>();

    public void PlayerEnterArea(NetworkBehaviour player)
    {
        if (!IsServer) return;
        if (!playersInArea.ContainsKey(player))
        {
            playersInArea[player] = false;
        }
    }

    public void PlayerLeaveArea(NetworkBehaviour player)
    {
        if (!IsServer) return;
        if (playersInArea.ContainsKey(player))
        {
            playersInArea.Remove(player);
        }
    }

    public void PlayerReady(NetworkBehaviour player)
    {
        if (!IsServer) return;
        if (playersInArea.ContainsKey(player))
        {
            playersInArea[player] = true;
            if (!playersInArea.ContainsValue(false))
            {
                StartCoroutine(Sleep());
            }
        }
    }

    private IEnumerator Sleep()
    {
        SleepClientRpc();
        yield return new WaitForSeconds(5);
        foreach (var player in playersInArea.Keys)
        {
            playersInArea[player] = false;
        }
        AwakeClientRpc();
    }

    [ClientRpc]
    void SleepClientRpc()
    {
        sleepCanvas.SetActive(true);
        sleepText.text = "The current day is " + System.DateTime.Now.DayOfWeek.ToString();
    }

    [ClientRpc]
    void AwakeClientRpc()
    {
        sleepCanvas.SetActive(false);
        sleepText.text = "";
    }
}
