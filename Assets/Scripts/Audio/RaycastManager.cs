using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastManager : MonoBehaviour
{
    [SerializeField] GameObject playerCharacter;
    [SerializeField] LayerMask obstructionMask;

    private void Start()
    {
        if (!playerCharacter)
        {
            PlayerController pc = FindObjectOfType<PlayerController>();
            if (pc)
            {
                playerCharacter = pc.gameObject;
            }      
        }
            
    }
    public bool IsPlayerInRaycastRange(Vector3 emitterPosition, float raycastRange)
    {
        if (!playerCharacter)
        {
            return false;
        }

        if(Vector3.Distance(playerCharacter.transform.position, emitterPosition) < raycastRange)
        {
            Vector3 dir = (playerCharacter.transform.position - emitterPosition).normalized;
            RaycastHit hit;
            if (Physics.SphereCast(emitterPosition, 0.1f, dir, out hit, raycastRange, obstructionMask))
            {
                if (hit.transform.gameObject != this.gameObject)
                {
                    if (hit.transform.gameObject.tag == playerCharacter.tag)
                    {
                        //Debug.Log("PlayerInSight");
                        return true;
                    }
                }
            }
        }
        //Debug.Log("PlayerNotSight");
        return false;
    }
}
