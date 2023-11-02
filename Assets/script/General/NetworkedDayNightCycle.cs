using System.Collections;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
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

    public override void OnNetworkSpawn()
    {    
            StartCoroutine(UpdateTime());
        networkDayTime.OnValueChanged += OnDayTimeChanged;

        
    }

    private void Update()
    {
        if (IsServer)
        {
            //networkDayTime.OnValueChanged += OnDayTimeChanged;
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
        //UpdateHDRI(timeRatio);
    }
    //private void UpdateHDRI(float timeRatio)
    //{
    //    if (hdriSky == null) return;

    //    // Assuming 0-0.25 is night, 0.25-0.75 is day, and 0.75-1 is night again.
    //    if (timeRatio > 0.25f && timeRatio < 0.75f)
    //    {
    //        // It's day time
    //        hdriSky.hdriSky.overrideState = true;
    //        hdriSky.hdriSky.value = NightSkybox;
    //    }
    //    else
    //    {
    //        // It's night time
    //        hdriSky.hdriSky.overrideState = true;
    //        hdriSky.hdriSky.value = DaySkybox;
    //    }

    //    // Force an update of the visual environment
    //    VisualEnvironment visualEnvironment = GlobalVolume.GetComponent<VisualEnvironment>();
    //    if (visualEnvironment != null)
    //    {
    //        visualEnvironment.skyType.value = (int)SkyType.HDRI;
    //    }
    //}
}
