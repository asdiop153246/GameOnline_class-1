using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class NavigationBaker : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private NavMeshSurface surface;

    [SerializeField]
    private Transform objectToRotation;

    // Update is called once per frame
    void Update()
    {
        objectToRotation.localRotation = Quaternion.Euler(new Vector3(0, 15 * Time.deltaTime, 0) + objectToRotation.localRotation.eulerAngles);
        surface.BuildNavMesh();
    }
}
