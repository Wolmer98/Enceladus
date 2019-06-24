using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimCallBackupAction", menuName = "AI/AnimAction/CallBackup")]
public class AnimCallBackupAction : AnimatorAction
{
    public float callDuration = 1.5f;
    public float soundRange = 30f;
    public LayerMask enemyDetectionMask;

    public override void EnterActions(AI ai, Animator anim)
    {
        ai.Agent.isStopped = true;
        DetectableSound.PlayDetectableSound(ai.transform.position,soundRange, enemyDetectionMask);
        ai.Animator.Play("CallBackup");
        FMODUnity.RuntimeManager.PlayOneShot(ai.aggroSound, ai.transform.position);
        ai.ActionTime = 0;
    }

    public override void ExitAction(AI ai, Animator anim)
    {
        ai.ActionTime = 0f;
    }

    public override void UpdateAction(AI ai, Animator anim)
    {
        if(ai.ActionTimeCheck(callDuration))
        {
            ai.CalledForFriends = true;
        }
    }

}
