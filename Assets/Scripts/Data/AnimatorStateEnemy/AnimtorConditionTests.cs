using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimtorConditionTests 
{
    [SerializeField] AnimatiorConditionTransition[] triggerTransitions;

    /// <summary>
    /// Returns false if no transition is to be made.
    /// </summary>
    public bool TestConditions(AI ai, Animator anim)
    {
        if (triggerTransitions == null)
        {
            return false;
        }

        for (int i = 0; i < triggerTransitions.Length; i++)
        {

            bool condition = triggerTransitions[i].Condition.CheckCondition(ai);
            if (condition && triggerTransitions[i].TrueState != null)
            {
                anim.SetTrigger(triggerTransitions[i].TrueState);
                return true;
            }
            else if (!condition && triggerTransitions[i].FalseState != null)
            {
                anim.SetTrigger(triggerTransitions[i].FalseState);
                return true;
            }
        }
        return false;
    }
}
