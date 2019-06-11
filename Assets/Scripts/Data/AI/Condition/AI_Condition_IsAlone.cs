using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI_Condition_IsAlone", menuName = "AI/Condition/IsAlone")]
public class AI_Condition_IsAlone : AI_Condition
{
    public LayerMask enemy;

    public override bool CheckCondition(AI ai)
    {
        return IsAlone(ai);
    }

    public bool IsAlone(AI ai)
    {
        if (ai.FindNearbyAllies)
        {
            //Debug.Log(ai.ID + " found new friends");
            FindAlliesInRange(ai);
        }

        if (ai.AlliesAround.Count <= 1)
        {
            //Debug.Log(ai.ID + " is alone");
            return true;
        }
        //Debug.Log(ai.ID + " is not alone: " + ai.AlliesAround.Count);
        return false;
    }

    private void FindAlliesInRange(AI ai)
    {
        Collider[] colliders = new Collider[5];
        Physics.OverlapSphereNonAlloc(ai.transform.position, ai.Stats.awareOfFriendRadius, colliders, enemy);
        //Debug.Log("FindAlliesInRange");

        ai.AlliesAround.Clear();
        ai.AlliesAround.TrimExcess();

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] != null)
            {
                AI aiFound = colliders[i].GetComponent<AI>();
                if (aiFound)
                {
                    //Debug.Log("AI found");
                    ai.AlliesAround.Add(aiFound);

                    if(!ai.IDsRegisteredAt.Contains(aiFound.ID) || aiFound.ID != ai.ID)
                    {
                        //Debug.Log(ai.ID + " has registered: " + aiFound.ID);
                        ai.GetComponentInChildren<Destructible>().OnDeath.AddListener(delegate { ai.FriendDied(); });
                        ai.IDsRegisteredAt.Add(aiFound.ID);
                    }
                }
            }
        }

        ai.FindNearbyAllies = false;
    }
}
