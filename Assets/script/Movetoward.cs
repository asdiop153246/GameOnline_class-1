using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movetoward : MonoBehaviour
{
    public float speed = 1.0f;
    public Transform target;
    public Transform target2; // Second destination
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
        yield return new WaitForSeconds(seconds);
        target = target2; // Change the target to the second destination
        isFirstTarget = false;
        isWaiting = false;
    }
}
