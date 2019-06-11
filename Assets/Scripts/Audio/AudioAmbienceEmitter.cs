using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioAmbienceEmitter : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string roomAmbienceEvent;
    public FMOD.Studio.EventInstance roomAmbience;

    void Start()
    {
        FindObjectOfType<WorldGenerator>().OnWorldStart.AddListener(delegate { Init(); });
    }

    private void Init()
    {
        roomAmbience.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        roomAmbience = FMODUnity.RuntimeManager.CreateInstance(roomAmbienceEvent);
        roomAmbience.setParameterByName("Room", 0f);
        roomAmbience.start();
        roomAmbience.release();
    }
    /*
public void PlayFootstep(int material)
{

   switch (material)
   {
       case 1:
           roomAmbience.setParameterValue("Room", 0f);
           break;
       case 2:
           roomAmbience.setParameterValue("Room", 1f);
           break;
       case 3:
           roomAmbience.setParameterValue("Room", 2f);
           break;
       case 4:
           roomAmbience.setParameterValue("Room", 3f);
           break;
   }
}
*/
}
