using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeyserManager : MonoBehaviour
{
    [SerializeField] Vector2 intervalRandomRange;
    [SerializeField] float duration = 1;

    [Header("Camera Shake Settings")]
    [SerializeField] float amplitude = 0.1f;
    [SerializeField] float speed = 30;

    float intervalTimer;
    float interval;

    float durationTimer;

    CameraShake camShake;

    FMODUnity.StudioEventEmitter soundEmitter;

    private void Start()
    {
        FindObjectOfType<WorldGenerator>().OnWorldStart.AddListener(delegate { Init(); });
    }

    void Init()
    {
        camShake = FindObjectOfType<CameraShake>();
        soundEmitter = GetComponent<FMODUnity.StudioEventEmitter>();
        soundEmitter.EventInstance.release();

        interval = Random.Range(intervalRandomRange.x, intervalRandomRange.y) + duration;
    }

    void Update()
    {
        if (durationTimer >= 0)
        {
            durationTimer -= Time.deltaTime;
            if (soundEmitter != null && soundEmitter.Event != null)
            {
                soundEmitter.EventInstance.setParameterByName("Geyser", durationTimer / duration);
            }
        }

        if (interval > 0)
        {
            if (intervalTimer <= interval)
                intervalTimer += Time.deltaTime;
            else
            {
                StartGeyser();
            }
        }
    }

    public void StartGeyser()
    {
        durationTimer = duration;
        intervalTimer = 0;
        interval = Random.Range(intervalRandomRange.x, intervalRandomRange.y) + duration;

        camShake.InitCameraShake(duration, amplitude, speed);
        soundEmitter.Play();
    }
}
