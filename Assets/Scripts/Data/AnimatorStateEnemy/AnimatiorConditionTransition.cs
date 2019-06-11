using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimatiorConditionTransition 
{
    [SerializeField] string transitionName;
    [SerializeField] AI_Condition condition;
    [Tooltip("This state will be transitioned too if condition is true.")]
    [SerializeField] string trueState;
    [Tooltip("This state will be transitioned too if condition is false.")]
    [SerializeField] string falseState;

    public AI_Condition Condition { get { return condition; } }
    public string TrueState { get { return trueState; } }
    public string FalseState { get { return falseState; } }
}
