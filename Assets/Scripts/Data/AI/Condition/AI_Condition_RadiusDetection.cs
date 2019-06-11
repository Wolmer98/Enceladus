using UnityEngine;

[CreateAssetMenu(fileName = "Condition_RadiusDetection", menuName = "AI/Condition/RadiusDetection")]
public class AI_Condition_RadiusDetection : AI_Condition
{
    public override bool CheckCondition(AI ai)
    {
        return InRadius(ai);
    }

    private bool InRadius(AI ai)
    {
        Collider[] colliders = new Collider[5];
        Physics.OverlapSphereNonAlloc(ai.transform.position, ai.Stats.detectionRadius, colliders, ai.Stats.targetsLayerMask);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] != null)
            {
                ai.Target = colliders[i].transform;
                ai.ChasingPlayer = true;
                if (!ai.IsStunned)
                {
                    return true;
                }
            }
        }

        ai.ChasingPlayer = false;
        return false;
    }
}
