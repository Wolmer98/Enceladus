using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class ParticleSystemRandomizer : MonoBehaviour
{
    [ReorderableList] [SerializeField] ParticleSystem[] particleSystems;

    ParticleSystem lastPlayedSystem;

    public void PlayParticleSystems()
    {
        int rngIndex = Random.Range(0, particleSystems.Length);
        ParticleSystem[] currentSystems = particleSystems[rngIndex].GetComponentsInChildren<ParticleSystem>();
        lastPlayedSystem = particleSystems[rngIndex];

        foreach (ParticleSystem p in currentSystems)
        {
            p.Play();
        }
    }

    public void StopParticleSystems()
    {
        if (lastPlayedSystem == null)
        {
            return;
        }

        ParticleSystem[] currentSystems = lastPlayedSystem.GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem p in currentSystems)
        {
            p.Stop();
        }
    }
}
