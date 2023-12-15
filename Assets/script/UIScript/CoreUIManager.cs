using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System;

public class CoreUIManager : NetworkBehaviour
{
    public static CoreUIManager Instance;
    public OtherCoreScript otherCore;
    public GameObject CoreUI;
    public Image EnergyBar;
    public Image BackEnergyBar;
    public PlayerControllerScript PlayerMovement { get; private set; }
    public MoveCamera CameraMovement { get; private set; }
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
    private void Update()
    {
        //if (!IsOwner) return;

        if (otherCore == null)
        {
            FindOtherCoreInstance();
        }
    }

    private void FindOtherCoreInstance()
    {
        OtherCoreScript foundInstance = FindObjectOfType<OtherCoreScript>();
        if (foundInstance != null)
        {
            otherCore = foundInstance;
        }
    }
    
}
