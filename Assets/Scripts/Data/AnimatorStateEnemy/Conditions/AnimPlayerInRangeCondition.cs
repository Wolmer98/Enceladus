using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "AnimConditionPlayerInRange", menuName = "AI/AnimCondition/PlayerInRange")]
public class AnimPlayerInRangeCondition : AI_Condition
{
    public override bool CheckCondition(AI ai)
    {
        if(Vector3.Distance(ai.transform.position, ai.Player.transform.position) < ai.DetectionSphere.radius)
        {
            return PathValid(ai);
        }
        else
        {
            return false;
        }
    }

    private bool PathValid(AI ai)
    {
        NavMeshPath path = new NavMeshPath();
        ai.Agent.CalculatePath(ai.Player.transform.position, path);
        if(path.status != NavMeshPathStatus.PathPartial)
        {
            //Debug.Log("Path valid");
            return true;
        }
        //Debug.Log("Path Invalid");
        return false;
    }
}
