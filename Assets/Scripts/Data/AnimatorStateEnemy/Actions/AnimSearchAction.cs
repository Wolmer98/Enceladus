using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "AnimSearchAction", menuName = "AI/AnimAction/Search")]
public class AnimSearchAction : AnimatorAction
{
    public override void EnterActions(AI ai, Animator anim)
    {
        ai.FoundSearchPoints = false;
        FindSearchPoints(ai);
        ai.Agent.SetDestination(ai.SearchPoints[0]);
        ai.SearchPointIndex = 0;
        ai.Agent.isStopped = false;
        ai.ConditionTime = 0f;
        ai.Agent.speed = ai.Stats.chaseSpeed;
        ai.Animator.SetBool("Walking", true);
    }

    public override void ExitAction(AI ai, Animator anim)
    {
        ai.FoundSearchPoints = false;
        ai.ConditionTime = 0f;
        anim.ResetTrigger("Search");
        ai.Animator.SetBool("Walking", false);
    }

    public override void UpdateAction(AI ai, Animator anim)
    {
        if (!ai.FoundSearchPoints)
        {
            return;
        }

        if(ai.Agent.remainingDistance <= ai.Agent.stoppingDistance && !ai.Agent.pathPending)
        {
            ai.SearchPointIndex = (ai.SearchPointIndex + 1) % ai.SearchPoints.Length;
            ai.Agent.SetDestination(ai.SearchPoints[ai.SearchPointIndex]);
            if (Random.Range(0f, 1f) < ai.Stats.chanceOfNearRadius)
            {
                ai.ActionTime = 0.0f;
                ai.SetATimer = true;

                ai.Animator.Play("EchoLocation");

                ai.Animator.SetBool("Walking", false);
                AIStoppedMoving(ai);
            }
        }
        else if (ai.SetATimer)
        {
            if (ai.ActionTimeCheck(ai.Stats.nearRadiusDuration))
            {
                ai.SetATimer = false;
                ai.Animator.SetTrigger("EchoLocationOver");
                AIStoppedMoving(ai);
            }
        }
        else
        {
            AIStartedMoving(ai);
            ai.Animator.SetBool("Walking", true);
        }
    }


    private void FindSearchPoints(AI ai)
    {
        // ai.SearchPoints[0] = ai.SoundLastPosition;
        ai.SearchPoints[0] = ai.SoundLastPosition;
        int missedPoints = 0;

        NavMeshPath path = new NavMeshPath();

        for (int i = 1; i < ai.Stats.nbrSearchPos; i++)
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
                            ai.SearchPoints[i] = hit.position;
                            ai.FoundSearchPoints = true;
                        }
                        else
                        {
                            missedPoints++;
                            if(missedPoints >= 20)
                            {
                                Debug.LogWarning("AI was unable to find enough points on a navmesh");
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