using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastTestForSound : MonoBehaviour
{
    [SerializeField] FMODUnity.StudioEventEmitter eventEmitter;
    [SerializeField] float raycastRange = 20f;
    [SerializeField] float parameterOnFalse;
    [SerializeField] float paramaterOnTrue;

    private RaycastManager raycastManager;

    // Start is called before the first frame update
    void Start()
    {
        raycastManager = FindObjectOfType<RaycastManager>();

        if(eventEmitter == null)
        {
            eventEmitter = GetComponent<FMODUnity.StudioEventEmitter>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (raycastManager)
        {
            float snapshot;
            eventEmitter.EventInstance.getParameterByName("Snapshot", out snapshot);

            if (raycastManager.IsPlayerInRaycastRange(eventEmitter.transform.position, raycastRange))
            {

                if (snapshot != parameterOnFalse)
                {
                    eventEmitter.EventInstance.setParameterByName("Snapshot", parameterOnFalse);
                }
            
            }
            else
            {
                {
                    if (snapshot != paramaterOnTrue)
                    {
                        eventEmitter.EventInstance.setParameterByName("Snapshot", paramaterOnTrue);
                    }
                }
            }
            //Debug.Log("Snapshot is currently: " + snapshot);
        }
    }
}
