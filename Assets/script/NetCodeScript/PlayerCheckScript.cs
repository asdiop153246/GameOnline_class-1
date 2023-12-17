using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerCheckScript : NetworkBehaviour
{
    [Header("Material")]
    public Material materialP1;
    public Material materialP2;
    public Material materialP3;
    public Material materialP4;

    [Header("PlayerObject")]
    public GameObject PlayerMesh;

    private int PlayerCount;
    private void Update()
    {
        CheckMaterial();
    }

    private void CheckMaterial()
    {
        if (IsOwnedByServer)
        {
            //PlayerMesh = 
        }
    }
}
