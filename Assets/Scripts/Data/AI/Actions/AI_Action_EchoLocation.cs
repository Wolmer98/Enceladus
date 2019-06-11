using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Action_EchoLocate", menuName = "AI/Action/EchoLocate")]
public class AI_Action_EchoLocation : AI_Action
{
    public override void PreformAction(AI ai)
    {
        EchoLocation(ai);
    }

    private void EchoLocation(AI ai)
    {
        if(ai.Stats == null)
        {
            return;
        }


        if(ai.IsScreaming)
        {
            if(!ai.EchoTimer(ai.Stats.farRadiusDuration))
            {
                PlayEcho(ai, 1.0f);
                ai.DetectionSphere.radius = Mathf.Lerp(ai.DetectionSphere.radius, ai.Stats.echoDetectionFarRadius, ai.Stats.farRadiusLerpSpeed * Time.deltaTime);
            }
            else
            {
                ai.DetectionSphere.radius = ai.Stats.detectionRadius;
                ai.IsScreaming = false;
            }
        }
        else if(ai.IsEchoLocating)
        {
            if(!ai.EchoTimer(ai.Stats.nearRadiusDuration))
            {
                PlayEcho(ai, 0.0f);
                ai.DetectionSphere.radius = Mathf.Lerp(ai.DetectionSphere.radius, ai.Stats.echoDetectionNearRadius, ai.Stats.nearRadiusLerpSpeed * Time.deltaTime);
            }
            else
            {
                ai.DetectionSphere.radius = ai.Stats.detectionRadius;
                ai.IsEchoLocating = false;
            }
        }
        else
        {
            ai.DetectionSphere.radius = ai.Stats.detectionRadius;
        }
    }

    private void PlayEcho(AI ai, float ecolocation)
    {
        if (ai.soundEmitter == null)
        {
            return;
        }

        FMOD.Studio.PLAYBACK_STATE playBackState;
        ai.soundEmitter.EventInstance.getPlaybackState(out playBackState);
        if (playBackState != FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            ai.soundEmitter.Event = ai.echoSound;
            ai.soundEmitter.Play();
            ai.soundEmitter.EventInstance.release();
            ai.soundEmitter.EventInstance.setParameterByName("Ecolocation", ecolocation);
            if (!ai.TestIfPlayerInSight(ai.transform.position, 20.0f))
            {
                ai.soundEmitter.EventInstance.setParameterByName("Snapshot", 1.0f);
            }
            else
            {
                ai.soundEmitter.EventInstance.setParameterByName("Snapshot", 0.0f);
            }
        }
    }
}
