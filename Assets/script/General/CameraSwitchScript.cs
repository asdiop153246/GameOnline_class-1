using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class CameraSwitchScript : NetworkBehaviour
{
    public static CameraSwitchScript Instance;
    public GameObject CameraSwitch;
    public GameObject CameraWin;
    public GameObject CameraLose;
    public GameObject Helicopter;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
