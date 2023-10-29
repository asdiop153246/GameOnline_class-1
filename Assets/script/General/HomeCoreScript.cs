using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class HomeCoreScript : NetworkBehaviour
{
    public GameObject HomeCoreUI;
    public PlayerControllerScript playerMovement;
    public MoveCamera cameraControl;
    public void OpenHomeCoreUI()
    {
        HomeCoreUI.SetActive(true);
        playerMovement.canMove = false;
        cameraControl.canRotate = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseHomeCoreUI()
    {
        HomeCoreUI.SetActive(false);
        playerMovement.canMove = true;
        cameraControl.canRotate = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
