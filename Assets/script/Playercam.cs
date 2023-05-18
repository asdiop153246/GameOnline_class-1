using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Playercam : NetworkBehaviour
{
    public GameObject Cam;
    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner) return;
        Cam.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
