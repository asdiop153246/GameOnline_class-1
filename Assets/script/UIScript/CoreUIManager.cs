using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CoreUIManager : MonoBehaviour
{
    public static CoreUIManager Instance;

    public GameObject CoreUI;
    public Image EnergyBar;
    public Image BackEnergyBar;

    private void Awake()
    {
        // Singleton pattern to ensure only one instance exists.
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
