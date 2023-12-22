using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using System;

public class EndDemoScript : NetworkBehaviour
{
    public Animator HomeIsland;
    public HomeCoreScript HomeCore;
    public BedInteraction Daycount;
    public TextMeshProUGUI DayText;
    public GameObject CameraSwitch;
    public GameObject Helicopter;
    public GameObject PlayerCamera;
    public GameObject CameraWin;
    public GameObject CameraLose;
    public TextMeshProUGUI EndDemoText;
    private bool isEnding = false;
    private bool isStart = false;
    public bool isCamReady = false;
    private void Start()
    {
        Daycount = GameObject.FindWithTag("Bed").GetComponent<BedInteraction>();
        HomeCore = GameObject.FindWithTag("HomeCore").GetComponent<HomeCoreScript>();
        HomeIsland = GameObject.FindWithTag("HomeIsland").GetComponent<Animator>();
    }
    private void Update()
    {
        if (IsServer)
            StartCoroutine(FindCamElements());
        {
            DayText.text = $"Day {Daycount.dayCount.Value}";
            if (isEnding == false)
            {               
                if (PlayerCamera != null)
                {
                    isStart = true;
                }
            }
            if (Daycount.dayCount.Value >= 10)
            {
                VictorySceneServerRpc();
            }
            if (isStart == true)
            {
                if (HomeCore.Health.Value <= 0 || HomeCore.Energy.Value <= 0 && isEnding == false)
                {
                    LoseSceneServerRpc();
                }
            }
        }
    }

    private IEnumerator FindCamElements()
    {
        while (!isCamReady)
        {
            if (CameraSwitchScript.Instance != null)
            {
                Debug.Log("Finding Camera/Helicopter");
                CameraSwitch = CameraSwitchScript.Instance.CameraSwitch;
                CameraWin = CameraSwitchScript.Instance.CameraWin;
                CameraLose = CameraSwitchScript.Instance.CameraLose;
                Helicopter = CameraSwitchScript.Instance.Helicopter;
                isCamReady = true;
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
    [ServerRpc]
    private void LoseSceneServerRpc()
    {
        isEnding = true;
        EndDemoText.text = "Game Over";
        HomeIsland.gameObject.SetActive(false);
        HomeIsland.SetTrigger("EndAnimation");
        StartCoroutine(HandleVictorySequence(false));
    }
    [ServerRpc]
    private void VictorySceneServerRpc()
    {
        isEnding = true;
        Helicopter.gameObject.SetActive(true);
        EndDemoText.text = "Thanks for playing Demo";
        StartCoroutine(HandleVictorySequence(true));
    }
    [ClientRpc]
    private void VictorySceneClientRpc()
    {
        isEnding = true;
        CameraWin.gameObject.SetActive(true);
        EndDemoText.gameObject.SetActive(true);
        PlayerCamera.gameObject.SetActive(false);       
    }
    [ClientRpc]
    private void LoseSceneClientRpc()
    {
        isEnding = true;
        CameraLose.gameObject.SetActive(true);
        EndDemoText.gameObject.SetActive(true);
        PlayerCamera.gameObject.SetActive(false);
    }
    private IEnumerator HandleVictorySequence(bool isVictory)
    {
        if (isVictory == true) 
        {
            VictorySceneClientRpc(); 
        }
        else
        {
            LoseSceneClientRpc();
        }       

        yield return new WaitForSeconds(20f);

        
        Debug.Log("Shutting down the server...");
        NetworkManager.Singleton.Shutdown();
        Application.Quit();

        // For Editor mode, you can add a debug log to confirm the quit command
#if UNITY_EDITOR
        Debug.Log("Game is quitting... (This only works outside of the Unity Editor)");
#endif
    }
}
