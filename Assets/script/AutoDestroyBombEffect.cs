using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AutoDestroyBombEffect : NetworkBehaviour
{
    public float delayTime = 1.0f;
    private ParticleSystem ps;
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }
    void Update()
    {
        if (!IsOwner) return;
        if (ps && !ps.IsAlive())
        {
            DestroyEffect();
        }
    }

    void DestroyEffect()
    {
        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject, delayTime);
    }
}
