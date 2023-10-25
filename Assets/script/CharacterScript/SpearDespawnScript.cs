using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class SpearDespawnScript : NetworkBehaviour
{
    public bool hasPlayer = false;
    private void OnTriggerEnter(Collider other)
    {
        PlayerControllerScript player = other.GetComponent<PlayerControllerScript>();
        if (player != null)
        {
            Debug.Log("Player entered the area!");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        PlayerControllerScript player = other.GetComponent<PlayerControllerScript>();
        if (player != null && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Player pressed E!");
            StartCoroutine(Waittime());
        }
    }
    IEnumerator Waittime ()
    {
        yield return new WaitForSeconds(1f);
        GetComponent<NetworkDestroy>().RequestDestroy();
    }
}
