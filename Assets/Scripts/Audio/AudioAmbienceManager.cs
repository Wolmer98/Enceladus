using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioAmbienceManager : MonoBehaviour
{
    public AudioAmbienceEmitter emitterScript;
    public LayerMask TriggerLayer;

    public int roomSize;

    private void Start()
    {
        emitterScript = FindObjectOfType<AudioAmbienceEmitter>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (TriggerLayer == (TriggerLayer | (1 << other.gameObject.layer)))
        {
            emitterScript.roomAmbience.setParameterByName("Room", roomSize);
            //Debug.Log("Size: " + roomSize);
        }
    }
}
