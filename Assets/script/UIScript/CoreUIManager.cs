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

    private void Start()
    {
        // Initialize playerMovement and cameraMovement
        PlayerMovement = FindObjectOfType<PlayerControllerScript>();
        CameraMovement = Camera.main.GetComponent<MoveCamera>();
    }

    private void Update()
    {
        if (!IsOwner) return;

        // Periodically check for OtherCoreScript instance
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

    public void OnTransferEnergyButtonPressed()
    {
        if (otherCore != null)
        {
            otherCore.TransferEnergy(200); // Transfer 200 energy as an example
        }
    }

    public void OpenCoreUI()
    {
        if (!IsOwner) return;

        CoreUI.SetActive(true);
        if (PlayerMovement != null)
        {
            PlayerMovement.canMove = false;
        }
        if (CameraMovement != null)
        {
            CameraMovement.canRotate = false;
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseCoreUI()
    {
        if (!IsOwner) return;

        CoreUI.SetActive(false);
        if (PlayerMovement != null)
        {
            PlayerMovement.canMove = true;
        }
        if (CameraMovement != null)
        {
            CameraMovement.canRotate = true;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
