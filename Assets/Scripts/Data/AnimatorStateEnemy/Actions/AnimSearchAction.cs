using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "AnimSearchAction", menuName = "AI/AnimAction/Search")]
public class AnimSearchAction : AnimatorAction
{
    public override void EnterActions(AI ai, Animator anim)
    {
        ai.SearchPoints[0] = ai.SoundLastPosition;
        ai.Agent.SetDestination(ai.SearchPoints[0]);
        ai.SearchPointIndex = 0;
        ai.Agent.SetDestination(ai.SearchPoints[ai.SearchPointIndex]);
        ai.ChasingPlayer = false;

        ai.Agent.isStopped = false;
        ai.ConditionTime = 0f;
        ai.Agent.speed = ai.Stats.chaseSpeed;
        ai.Animator.SetBool("Walking", true);
        ai.Animator.Play("Walking");
        ai.SetATimer = false;
    }

    public override void ExitAction(AI ai, Animator anim)
    {
        ai.FoundSearchPoints = false;
        ai.ConditionTime = 0f;
        anim.ResetTrigger("Search");
        ai.SetATimer = false;
        //ai.Animator.SetBool("Walking", false);
    }

    public override void UpdateAction(AI ai, Animator anim)
    {
        if(ai.Agent.remainingDistance <= ai.Agent.stoppingDistance && !ai.Agent.pathPending)
        {
            if (Random.Range(0f, 1f) < ai.Stats.chanceOfFarRadius && !ai.SetATimer)
            {
                ai.ActionTime = 0.0f;
                ai.SetATimer = true;

                ai.Animator.Play("EchoLocation");
                ai.PlayEcho(1.0f);
                AIStoppedMoving(ai);
                //Find new search point
                if(ai.SearchPointIndex < ai.SearchPoints.Length)
                {
                    FindNewSearchPoint(ai);
                }
                //ai.SearchPointIndex = (ai.SearchPointIndex + 1) % ai.SearchPoints.Length;
                //ai.Agent.SetDestination(ai.SearchPoints[ai.SearchPointIndex]);
            }
        }
        else if (ai.SetATimer)
        {
            ai.DetectionSphere.radius = Mathf.Lerp(ai.DetectionSphere.radius, ai.Stats.echoDetectionFarRadius, ai.Stats.farRadiusLerpSpeed * Time.deltaTime);
            if (ai.ActionTimeCheck(ai.Stats.farRadiusDuration))
            {
                ai.SetATimer = false;
                ai.Animator.SetBool("Walking", true);
                ai.Animator.Play("Walking");
                AIStartedMoving(ai);
                ai.DetectionSphere.radius = ai.Stats.detectionRadius;
            }
        }
    }


    private void FindNewSearchPoint(AI ai)
    {
        int missedPoints = 0;

        NavMeshPath path = new NavMeshPath();

        for (int i = 0; i < 1; i++)
        {
            Vector3 randomDir = Random.insideUnitSphere * ai.Stats.searchRadius;
            randomDir += ai.SoundLastPosition;

            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomDir, out hit, ai.Stats.searchRadius, NavMesh.AllAreas))
            {
                // Debug.Log("hitpos:" + hit.position + " transformPos: " + ai.transform.position);
                if (NavMesh.CalculatePath(ai.transform.position, hit.position, NavMesh.AllAreas, path))
                {
                    if (path.status != NavMeshPathStatus.PathPartial)
                    {
                        if (GetPathLength(path) < ai.Stats.searchRadius)
                        {
                            ai.Agent.SetDestination(hit.position);
                        }
                        else
                        {
                            missedPoints++;
                            if(missedPoints >= 20)
                            {
                                Debug.LogWarning("AI was unable to find a search point on a navmesh");
                                ai.FoundSearchPoints = false;
                                break;
                            }
                            i--;
                        }
                    }
                }
            }
        }
    }

    private float GetPathLength(NavMeshPath path)
    {
        float lng = 0.0f;

        if ((path.status != NavMeshPathStatus.PathInvalid) && (path.corners.Length > 1))
        {
            for (int i = 1; i < path.corners.Length; ++i)
            {
                lng += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }
        }

        return lng;
    }
}