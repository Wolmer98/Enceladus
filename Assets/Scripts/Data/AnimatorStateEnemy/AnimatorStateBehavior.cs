﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorStateBehavior : StateMachineBehaviour
{
    [SerializeField] AnimatiorConditionTransition[] conditionTransitions;
    [SerializeField] AnimatorAction[] actions;

    private AI ai;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (ai == null)
        {
            ai = animator.gameObject.GetComponent<AI>();
        }

        if(ai.animatorAction != null)
        {
            ai.animatorAction.ExitAction(ai, animator);
        }

        foreach (AnimatorAction animatorAction in actions)
        {
            animatorAction.EnterActions(ai, animator);

            ai.animatorAction = animatorAction;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(!ai.MoveAcrossNavMeshesStarted)
        {
            // Preforms the actions for this state.
            foreach (AnimatorAction animatorAction in actions)
            {
                if (ai.animatorAction != animatorAction)
                {
                    return;
                }
                animatorAction.UpdateAction(ai, animator);
            }

            // Test for conditions and if AI should transition to another state.
            foreach (AnimatiorConditionTransition condition in conditionTransitions)
            {
                if (condition.Condition != null)
                {
                    if (condition.Condition.CheckCondition(ai))
                    {
                        if (condition.TrueState != null && condition.TrueState != "")
                        {
                            animator.SetTrigger(condition.TrueState);
                            return;
                        }
                    }
                    else
                    {
                        if (condition.FalseState != null && condition.FalseState != "")
                        {
                            animator.SetTrigger(condition.FalseState);
                            return;
                        }
                    }
                }
            }
        }
        ai.AnimUpdate();
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    foreach (AnimatorAction animatorAction in actions)
    //    {
    //        animatorAction.ExitAction(ai, animator);
    //    }
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
