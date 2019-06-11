using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioWaterRise : MonoBehaviour
{
    public MusicManager musicManager;
    public FMODUnity.StudioEventEmitter waterRiseEvent;
    float waterLevel;

    private void Start()
    {
        musicManager = FindObjectOfType<MusicManager>();
        waterLevel = musicManager.waterThreat * 2;
    }
}
