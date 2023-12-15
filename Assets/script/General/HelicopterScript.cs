using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class HelicopterScript : NetworkBehaviour
{
    [SerializeField] private float speed = 12.0f;
    [SerializeField] private Transform target;
    public Transform currentTarget;
    private Vector3 targetPosition;
    // Start is called before the first frame update
    void Start()
    {
        if (target != null)
        {
            targetPosition = target.position;  
            currentTarget = target;
        }
    }
    private void Update()
    {       
            MoveToTarget();       
    }
    private void MoveToTarget()
    {
        if (currentTarget != null)
        {            
            var step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
        }
        else
        {
            Debug.LogError("Current target is not assigned.");
        }
    }


}
