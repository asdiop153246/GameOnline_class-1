using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class EndDemoScript : NetworkBehaviour
{
    public HomeCoreScript HomeCore;
    public BedInteraction Daycount;
    public GameObject CameraSwitch;
    public GameObject Helicopter;
    public GameObject PlayerCamera;
    public GameObject CameraWin;
    public GameObject CameraLose;
    private bool isEnding = false;
    private bool isStart = false;
    public bool isCamReady = false;
    private void Start()
    {
        Daycount = GameObject.FindWithTag("Bed").GetComponent<BedInteraction>();
        HomeCore = GameObject.FindWithTag("HomeCore").GetComponent<HomeCoreScript>();
    }
    private void Update()
    {
        if (IsServer)
            StartCoroutine(FindCamElements());
        {
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
                if (HomeCore.Health.Value <= 0 || HomeCore.Energy.Value <= 0)
                {
                    CameraLose.gameObject.SetActive(true);
                    PlayerCamera.gameObject.SetActive(false);
                    isEnding = true;
                    isStart = false;
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
    private void VictorySceneServerRpc()
    {       
        isEnding = true;
        VictorySceneClientRpc();
    }
    [ClientRpc]
    private void VictorySceneClientRpc()
    {
        CameraWin.gameObject.SetActive(true);
        PlayerCamera.gameObject.SetActive(false);
        Helicopter.gameObject.SetActive(true);
    }
}
