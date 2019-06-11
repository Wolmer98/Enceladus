using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Name_State", menuName = "AI/State")]
public class AI_State : ScriptableObject
{
    [Header("Transition")]
    [SerializeField] AI_Transition[] transitions;

    [Header("ActionInState")]
    [SerializeField] AI_Action[] aI_Actions;

    /// <summary>
    /// Called by the AI controller every update
    /// </summary>
    public void UpdateState(AI ai)
    {
        PreformAction(ai);

        CheckCondition(ai);

    }

    /// <summary>
    /// Called by the AI controller every fixed update
    /// </summary>
    public void FixedUpdateState(AI ai)
    {
        if (aI_Actions == null)
        {
            return;
        }

        foreach (AI_Action action in aI_Actions)
        {
            if (action.isFixedUpdate)
            {
               // Debug.Log("Fixed Update: " + action);
                action.PreformAction(ai);
            }
        }
    }

    private void PreformAction(AI ai)
    {
        if (aI_Actions == null)
        {
            return;
        }
        
        for (int i = 0; i < aI_Actions.Length; i++)
        {
            if (!aI_Actions[i].isFixedUpdate)
            {
              //  Debug.Log("Update: " + aI_Actions[i]);
                aI_Actions[i].PreformAction(ai);
            }
        }
    }

    private void CheckCondition(AI ai)
    {
        if(ai.EnemyHurt)
        {
            ai.EnemyHurt = false;

            for (int i = 0; i < ai.OnHurtConditions.Length; i++)
            {
                if (ai.OnHurtConditions[i].Condition == null)
                {
                    continue;
                }

                bool condition = ai.OnHurtConditions[i].Condition.CheckCondition(ai);

                if (condition && ai.OnHurtConditions[i].TrueState != null)
                {
                    ai.ChangeState(ai.OnHurtConditions[i].TrueState);
                    break;
                }
                else if (!condition && ai.OnHurtConditions[i].FalseState != null)
                {
                    ai.ChangeState(ai.OnHurtConditions[i].FalseState);
                    break;
                }
            }
        }

        if (transitions == null)
        {
            return;
        }

        for (int i = 0; i < transitions.Length; i++)
        {

            bool condition = transitions[i].Condition.CheckCondition(ai);
            if (condition && transitions[i].TrueState != null)
            {
                ai.ChangeState(transitions[i].TrueState);
            }
            else if (!condition && transitions[i].FalseState != null)
            {
                ai.ChangeState(transitions[i].FalseState);
            }
        }
    }
    
}
