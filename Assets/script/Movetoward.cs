using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Movetoward : NetworkBehaviour
{
    public float speed = 5.0f;
    public Transform target;
    public Transform target2; // Second destination
    public GameObject monsterPrefab; // Add your monster prefab here
    public Transform[] monsterSpawnPoints; // The positions where the monsters will be spawned
    private bool isFirstTarget = true;
    private bool isWaiting = false;

    void Update()
    {
        // if waiting don't update position
        if (isWaiting)
            return;

        var step = speed * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);

        // Check if the object has reached its destination
        if (Vector3.Distance(transform.position, target.position) < 0.001f)
        {
            if (isFirstTarget)
            {
                StartCoroutine(WaitAndMove(30)); // Wait for 30 seconds
            }
        }
    }

    IEnumerator WaitAndMove(float seconds)
    {
        isWaiting = true;
        if (isFirstTarget)
        {
            // Spawn the monsters if this is the server
            if (IsServer)
            {
                foreach (var spawnPoint in monsterSpawnPoints)
                {
                    var monster = Instantiate(monsterPrefab, spawnPoint.position, spawnPoint.rotation);
                    monster.GetComponent<NetworkObject>().Spawn();
                }
            }

            target = target2; // Change the target to the second destination
            isFirstTarget = false;
        }
        yield return new WaitForSeconds(seconds);

        

        isWaiting = false;
    }
}
