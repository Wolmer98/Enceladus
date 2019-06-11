using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FModTestScript : MonoBehaviour
{
    [FMODUnity.EventRef]
    [SerializeField] string testSound;
    public FMOD.Studio.EventInstance testSoundEvent;

    public FMODUnity.StudioEventEmitter fireEmitter;

    public bool isPlaying;
    // Start is called before the first frame update
    void Start()
    {
        testSoundEvent = FMODUnity.RuntimeManager.CreateInstance(testSound);
        fireEmitter.Event = testSound;
        fireEmitter.EventInstance.setParameterByName("Mjau", 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.T))
        {
            testSoundEvent.setParameterByName("Trigger", 1f);
            if(!isPlaying)
            {
                testSoundEvent.start();
                testSoundEvent.release();
            }

            isPlaying = true;
        }
        else
        {
            testSoundEvent.setParameterByName("Trigger", 0f);
            isPlaying = false;
        }

        if(Input.GetKey(KeyCode.U))
        {
            testSoundEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
}
