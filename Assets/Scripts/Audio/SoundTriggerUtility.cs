using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FMODSoundContainer
{
    [FMODUnity.EventRef]
    public string eventRef;
}

public class SoundTriggerUtility : MonoBehaviour
{
    [SerializeField] FMODSoundContainer[] sounds;

    public void TriggerSound(int i)
    {
        if (sounds != null && sounds.Length > 0)
        {
            FMODUnity.RuntimeManager.PlayOneShot(sounds[i].eventRef, transform.position);
        }
    }

    public void TriggerAllSounds()
    {
        if (sounds != null && sounds.Length > 0)
        {
            foreach (FMODSoundContainer fms in sounds)
            {
                FMODUnity.RuntimeManager.PlayOneShot(fms.eventRef, transform.position);
            }
        }
    }
}
