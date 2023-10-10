using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class NetworkedDayNightCycle : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] private float fullDayLength = 120f;
    [SerializeField] private Vector3 noonSunDirection = new Vector3(50f, 78.37f, 0f);

    [Header("Day Night Colors")]
    [SerializeField] private Color dayColor;
    [SerializeField] private Color nightColor;

    [Header("Components")]
    [SerializeField] private Light sunLight;

    private float currentDayTime;
    private NetworkVariable<float> networkDayTime = new NetworkVariable<float>();

    private void Start()
    {
        
        
            StartCoroutine(UpdateTime());
        

        networkDayTime.OnValueChanged += OnDayTimeChanged;
    }

    private void Update()
    {
        if (IsServer)
        {
            UpdateSunPosition();
        }
    }

    IEnumerator UpdateTime()
    {
        while (true)
        {
            currentDayTime += Time.deltaTime;
            if (currentDayTime > fullDayLength)
            {
                currentDayTime -= fullDayLength;
            }

            networkDayTime.Value = currentDayTime;
            yield return null;
        }
    }

    private void UpdateSunPosition()
    {
        float timeRatio = currentDayTime / fullDayLength;
        float sunRotationAngle = timeRatio * 360f;
        Vector3 sunDirection = Quaternion.Euler(sunRotationAngle, 0, 0) * noonSunDirection.normalized;

        sunLight.transform.forward = -sunDirection;
        UpdateLightColor(timeRatio); // Added this line to update the light color based on time of day

        //Debug.Log($"Time Ratio: {timeRatio}, Sun Rotation Angle: {sunRotationAngle}, Sun Direction: {sunDirection}");
    }

    private void OnDayTimeChanged(float oldTime, float newTime)
    {
        currentDayTime = newTime;
        UpdateSunPosition();
        //Debug.Log($"Day Time Changed - Old Time: {oldTime}, New Time: {newTime}");
    }

    private void UpdateLightColor(float timeRatio)
    {
        // Assuming 0-0.25 is night, 0.25-0.75 is day, and 0.75-1 is night again.
        if (timeRatio > 0.25f && timeRatio < 0.75f)
        {
            // Day time
            float dayRatio = (timeRatio - 0.25f) / 0.5f;
            sunLight.color = Color.Lerp(nightColor, dayColor, dayRatio);
        }
        else
        {
            // Night time
            float nightRatio = timeRatio < 0.25f ? timeRatio / 0.25f : (timeRatio - 0.75f) / 0.25f;
            sunLight.color = Color.Lerp(dayColor, nightColor, nightRatio);
        }
    }
    //public float dayDuration = 10f;
    //public Light sun;
    //public Color dayColor;
    //public Color nightColor;

    //private NetworkVariable<float> timeOfDay = new NetworkVariable<float>(0f);

    //private bool isDay = true;

    //private void Start()
    //{
    //    if (!IsServer)
    //    {
    //        // Disable the script on non-server clients
    //        enabled = false;
    //    }
    //}

    //private void Update()
    //{
    //    // Update the time of day based on the server's synchronized value
    //    float normalizedTime = timeOfDay.Value / dayDuration;

    //    // Check if it's time to transition between day and night
    //    if (normalizedTime >= 1f)
    //    {
    //        // Invert the day/night state
    //        isDay = !isDay;

    //        // Reset the time of day
    //        timeOfDay.Value = 0f;

    //        // Transition to the appropriate state
    //        if (isDay)
    //        {
    //            SetDay();
    //        }
    //        else
    //        {
    //            SetNight();
    //        }
    //    }
    //}

    //private void SetDay()
    //{
    //    // Set the sun color to represent daytime
    //    sun.color = dayColor;
    //}

    //private void SetNight()
    //{
    //    // Set the sun color to represent nighttime
    //    sun.color = nightColor;
    //}
}
