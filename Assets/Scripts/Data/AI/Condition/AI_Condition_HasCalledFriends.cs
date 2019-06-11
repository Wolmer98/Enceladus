using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Condition_HasCalledForFriends", menuName = "AI/Condition/HasCalledForFriends")]
public class AI_Condition_HasCalledFriends : AI_Condition
{
    public float timeUntilInformFriends = 0.5f;
    public LayerMask enemyDetection;
    public float aiCallForFriendsRadius = 10f;

    public override bool CheckCondition(AI ai)
    {
        return HasCalledForFriends(ai);
    }

    private bool HasCalledForFriends(AI ai)
    {
        if (ai.CalledForFriends)
        {
            if(!ai.SetCTimer)
            {
                ai.ConditionTime = 0.0f;
                ai.SetCTimer = true;
            }

            if (ai.ConditionTimeCheck(timeUntilInformFriends))
            {
                Collider[] colliders = new Collider[10];
                Physics.OverlapSphereNonAlloc(ai.transform.position, aiCallForFriendsRadius, colliders, enemyDetection);

                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i] != null)
                    {
                        AI aiToInform = colliders[i].gameObject.GetComponentInParent<AI>();
                        if (aiToInform != null)
                        {
                            aiToInform.DetectedSound(ai.SoundLastPosition, true);
                            aiToInform.CalledForFriends = true;
                        }
                    }
                }
                return true;
            }
            return false;
        }
        else
            return false;
    }
}
