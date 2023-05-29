using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class NetworkedDayNightCycle : NetworkBehaviour
{
    public float dayDuration = 10f;
    public Light sun;
    public Color dayColor;
    public Color nightColor;

    private NetworkVariable<float> timeOfDay = new NetworkVariable<float>(0f);

    private bool isDay = true;

    private void Start()
    {
        if (!IsServer)
        {
            // Disable the script on non-server clients
            enabled = false;
        }
    }

    private void Update()
    {
        // Update the time of day based on the server's synchronized value
        float normalizedTime = timeOfDay.Value / dayDuration;

        // Check if it's time to transition between day and night
        if (normalizedTime >= 1f)
        {
            // Invert the day/night state
            isDay = !isDay;

            // Reset the time of day
            timeOfDay.Value = 0f;

            // Transition to the appropriate state
            if (isDay)
            {
                SetDay();
            }
            else
            {
                SetNight();
            }
        }
    }

    private void SetDay()
    {
        // Set the sun color to represent daytime
        sun.color = dayColor;
    }

    private void SetNight()
    {
        // Set the sun color to represent nighttime
        sun.color = nightColor;
    }
}
