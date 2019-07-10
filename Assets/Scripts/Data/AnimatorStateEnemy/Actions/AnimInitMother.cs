using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimInitMotherAction", menuName = "AI/AnimAction/InitMother")]
public class AnimInitMother : AnimatorAction
{
    public float initConditionTime = 20f;
    public override void EnterActions(AI ai, Animator anim)
    {
        ai.ConditionTime = initConditionTime;
        ai.HasSpawnedEnemies = false;
        anim.Play("Idle");
    }

    public override void ExitAction(AI ai, Animator anim)
    {
        //throw new System.NotImplementedException();
    }

    public override void UpdateAction(AI ai, Animator anim)
    {
        //throw new System.NotImplementedException();
    }


}
