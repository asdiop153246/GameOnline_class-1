using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private Volume globalVolume1;
    [SerializeField] private Volume globalVolume2;

    [Header("HDRI Sky")]
    [SerializeField] private Cubemap dayHDRI;
    [SerializeField] private Cubemap nightHDRI;
    private HDRISky hdriSky1;
    private HDRISky hdriSky2;

    [SerializeField] private IslandSpawnScript islandSpawnScript;
    public bool islandSpawnedThisCycle = false;
    public float timeRatio;
    public bool isDayTime = false;
    [SerializeField] public GameObject monsterPrefab;
    [SerializeField] public Transform[] monsterSpawnPoints;
    private List<GameObject> spawnedMonsters = new List<GameObject>();
    public bool monstersSpawned = false;
    private bool canSpawnNewMonsters = true;
    public bool daytimeTrigger = false;
    public BedInteraction DayCount;
    public float currentDayTime;
    public NetworkVariable<float> networkDayTime = new NetworkVariable<float>();

    public override void OnNetworkSpawn()
    {
        StartCoroutine(UpdateTime());
        networkDayTime.OnValueChanged += OnDayTimeChanged;

        
        if (!globalVolume1.profile.TryGet<HDRISky>(out hdriSky1))
        {
            Debug.LogWarning("No HDRISky found in Global Volume1.");
        }
        if (!globalVolume2.profile.TryGet<HDRISky>(out hdriSky2))
        {
            Debug.LogWarning("No HDRISky found in Global Volume2.");
        }
        if (DayCount == null)
        {
            DayCount = GameObject.FindWithTag("Bed").GetComponent<BedInteraction>();
        }
    }

    private void Update()
    {
        if (IsServer)
        {
            UpdateSunPosition();
            CheckAllMonstersDefeated();

            if (currentDayTime >= fullDayLength * 0.76f && !islandSpawnedThisCycle)
            {                
                islandSpawnScript.SpawnIsland();
                islandSpawnedThisCycle = true;
            }
            
            if (currentDayTime < fullDayLength * 0.76f)
            {
                islandSpawnedThisCycle = false;
            }
            if (monstersSpawned && monsterSpawnPoints.All(monster => !monster.gameObject.activeSelf))
            {            
                monstersSpawned = false;
            }
            if (currentDayTime >= 225f && currentDayTime <= 227 &&daytimeTrigger == false)
            {
                DayCount.dayCount.Value += 1;
                daytimeTrigger = true;
                StartCoroutine(DelaybeforeTrigger());
                
            }
        }
    }
    IEnumerator DelaybeforeTrigger()
    {
        yield return new WaitForSeconds(5f);
        daytimeTrigger = false;
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
        timeRatio = currentDayTime / fullDayLength;
        float sunRotationAngle = timeRatio * 360f;
        Vector3 sunDirection = Quaternion.Euler(sunRotationAngle, 0, 0) * noonSunDirection.normalized;

        sunLight.transform.forward = -sunDirection;
        UpdateLightColor(timeRatio); 

        Debug.Log($"Time Ratio: {timeRatio}, Sun Rotation Angle: {sunRotationAngle}, Sun Direction: {sunDirection}");
        //0.76 Day
    }

    private void OnDayTimeChanged(float oldTime, float newTime)
    {
        currentDayTime = newTime;
        UpdateSunPosition();
        //Debug.Log($"Day Time Changed - Old Time: {oldTime}, New Time: {newTime}");
    }

    private void UpdateLightColor(float timeRatio)
    {         
       
        if (timeRatio > 0.25f && timeRatio < 0.75f)
        {
            // Night time        
            float nightRatio = timeRatio < 0.25f ? timeRatio / 0.25f : (timeRatio - 0.75f) / 0.25f;
            sunLight.color = Color.Lerp(dayColor, nightColor, nightRatio);

        }
        else
        {
            // Day time
            float dayRatio = (timeRatio - 0.25f) / 0.5f;
            sunLight.color = Color.Lerp(nightColor, dayColor, dayRatio);

        }
        UpdateHDRISky(timeRatio);
    }
    private void UpdateHDRISky(float timeRatio)
    {
        if (hdriSky1 == null && hdriSky2 == null) return;

        
        float rotation = Mathf.Lerp(0f, 360f, timeRatio);
        hdriSky1.rotation.value = rotation;
        hdriSky2.rotation.value = rotation;


        if (timeRatio > 0.25f && timeRatio < 0.75f)
        {
            //Debug.Log("Night time");
            // Night time
            hdriSky1.hdriSky.value = nightHDRI;
            hdriSky2.hdriSky.value = nightHDRI;
            if (!monstersSpawned)
            {
                SpawnMonsters();
            }
        }
        else
        {
            hdriSky1.hdriSky.value = dayHDRI;
            hdriSky2.hdriSky.value = dayHDRI;
            if (monstersSpawned)
            {
                monstersSpawned = false;
            }
        }
    }
    private void SpawnMonsters()
    {
        if (canSpawnNewMonsters && Random.value <= 0.50f)
        {
            canSpawnNewMonsters = false;
            if (monsterSpawnPoints.Length > 0 && monsterPrefab != null)
            {
                foreach (var spawnPoint in monsterSpawnPoints)
                {
                    var monster = Instantiate(monsterPrefab, spawnPoint.position, spawnPoint.rotation);
                    monster.GetComponent<NetworkObject>().Spawn();
                    monstersSpawned = true;

                    // Add the monster to the list
                    spawnedMonsters.Add(monster);

                    Transform parentTransform = transform.parent;
                    if (parentTransform != null)
                    {
                        monster.transform.SetParent(parentTransform);
                        Debug.Log($"Monster spawned at {spawnPoint.name} under parent {parentTransform.name}");
                    }
                }
            }
            else
            {
                Debug.LogError("Monster is not spawning");
            }
        }
    }
    private void CheckAllMonstersDefeated()
    {
        if (monstersSpawned && spawnedMonsters.All(monster => monster == null))
        {
            StartCoroutine(ResetMonsterSpawn());
        }
    }
    public bool IsNightTime()
    {   
        
        timeRatio = currentDayTime / fullDayLength;        
        return timeRatio > 0.25f && timeRatio < 0.75f;
    }

    private IEnumerator ResetMonsterSpawn()
    {
        monstersSpawned = false;
        spawnedMonsters.Clear();
        Debug.Log("All monsters have been defeated. Waiting 30 seconds before new spawn...");

        yield return new WaitForSeconds(30); 

        canSpawnNewMonsters = true; 
        Debug.Log("Monsters can now spawn again.");
    }
    public void SetTimeToMorning()
    {
        if (IsServer)  
        {       
            currentDayTime = fullDayLength * 0.76f;
            networkDayTime.Value = currentDayTime;              
            UpdateSunPosition();
            UpdateHDRISky(currentDayTime / fullDayLength);
        }
    }
}
