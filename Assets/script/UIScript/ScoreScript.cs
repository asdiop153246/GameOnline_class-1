using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;


public class ScoreScript : NetworkBehaviour
{
    TMP_Text p1Text;
    TMP_Text p2Text;
    MainPlayerScript mainPlayer;

    public NetworkVariable<int> scoreP1 = new NetworkVariable<int>(5, NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    public NetworkVariable<int> scoreP2 = new NetworkVariable<int>(5, NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Owner);

    void Start()
    {
        p1Text = GameObject.Find("P1ScoreText").GetComponent<TMP_Text>();
        p2Text = GameObject.Find("P2ScoreText").GetComponent<TMP_Text>();
        mainPlayer = GetComponent<MainPlayerScript>();
    }

    private void updateScore()
    {
        if (IsOwnedByServer)
        {
            p1Text.text = $"{mainPlayer.playerNameA.Value} : {scoreP1.Value}";
        }
        else 
        {
            p2Text.text = $"{mainPlayer.playerNameB.Value} : {scoreP2.Value}";
        }
    }

    void Update()
    {
        updateScore();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsLocalPlayer) return;
        if (collision.gameObject.tag == "Deadzone")
        {
            if (IsOwnedByServer) { scoreP1.Value--; }
            else { scoreP2.Value--;}
            GetComponent<PlayerSpawnScript>().Respawn();
        }

    }
}
