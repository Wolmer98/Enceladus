using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimInitSwarmAction", menuName = "AI/AnimAction/InitSwarm")]
public class AnimSwarmInit : AnimatorAction
{
    public override void EnterActions(AI ai, Animator anim)
    {
        ai.SwarmSize = Random.Range(ai.AmountOfEnemies.x, ai.AmountOfEnemies.y);
        ai.SwarmAIs = new List<SwarmAI>();
        ai.Agent.speed = ai.Stats.moveSpeed;

        for (int i = 0; i < ai.SwarmSize; i++)
        {
            Vector3 offset = new Vector3(Random.Range(-2.0f, 2.0f), ai.transform.position.y, Random.Range(-2.0f, 2.0f));
            GameObject swarm = Instantiate(ai.SwarmPrefab, ai.transform.position + offset, Quaternion.identity, ai.transform);
            SwarmAI swarmAI = swarm.GetComponent<SwarmAI>();
            ai.SwarmAIs.Add(swarmAI);
            swarmAI.InitSwarm(ai);
            swarmAI.Agent.SetDestination(ai.transform.position);
            swarmAI.Animator.SetFloat("WalkingOffset", Random.Range(0f, 0.7f));
            swarmAI.Animator.SetFloat("IdleOffset", Random.Range(0f, 0.7f));
            swarmAI.SkinnedMeshRenderer.materials[0].SetColor("_EmissionColor", Color.white);
        }

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
