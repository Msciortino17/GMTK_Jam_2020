using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticles : MonoBehaviour
{
    public ParticleSystem ParticleSystem;

    /// <summary>
    /// Standard start
    /// </summary>
    void Start()
    {
        ParticleSystem = GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// Destroy the game object after the particle system does it's thing
    /// </summary>
    void Update()
    {
        if (!ParticleSystem.IsAlive())
        {
            Destroy(gameObject);
        }
    }
}
