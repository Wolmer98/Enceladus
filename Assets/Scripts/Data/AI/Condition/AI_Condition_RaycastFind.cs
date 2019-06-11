using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI_Condition_RaycastFind", menuName = "AI/Condition/RaycastFind")]
public class AI_Condition_RaycastFind : AI_Condition
{
    public override bool CheckCondition(AI ai)
    {
        return RaycastFind(ai);
    }

    private bool RaycastFind(AI ai)
    {
        /* RaycastHit hit;
         if (Physics.SphereCast(ai.Eyes.transform.position, ai.Stats.rayCastSphereSize, ai.Eyes.transform.forward, out hit, ai.Stats.sightRange, ai.Stats.targetsLayerMask))
         {
             ai.Target = hit.transform;
             ai.MeshRenderer.material = ai.ChaseMaterial;
             return true;
         }
         else
             ai.MeshRenderer.material = ai.BaseEyeMaterial;
             return false;*/
        return false;
    }
}
