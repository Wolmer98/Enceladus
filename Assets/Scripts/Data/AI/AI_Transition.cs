using UnityEngine;

[System.Serializable]
public class AI_Transition
{
    [SerializeField] string transitionName;
    [SerializeField] AI_Condition condition;
    [Tooltip("This state will be transitioned too if condition is true.")]
    [SerializeField] AI_State trueState;
    [Tooltip("This state will be transitioned too if condition is false, leave null to remain in state.")]
    [SerializeField] AI_State falseState;

    public AI_Condition Condition { get { return condition; } }
    public AI_State TrueState { get { return trueState; } }
    public AI_State FalseState { get { return falseState; } }
}
