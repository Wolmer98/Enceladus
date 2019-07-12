using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SwarmAI : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Vector3 damagePosition;
    [SerializeField] float damageRadius;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] float jumpCheckRange;
    [SerializeField] LayerMask jumpLayer;
    [SerializeField] SkinnedMeshRenderer skinnedMeshRenderer;

    private AI swarmController;
    private bool isAttacking;
    private Rigidbody rb;

    public Animator Animator { get { return animator;} }
    public NavMeshAgent Agent { get; private set; }

    public float AttackCooldown { get; set; }

    public bool Jumping { get; private set; }
    public SkinnedMeshRenderer SkinnedMeshRenderer { get { return skinnedMeshRenderer; } }
    public Vector3 DamagePosition { get { return transform.position + (transform.forward * damagePosition.z) + (transform.up * damagePosition.y); } }

    public void InitSwarm(AI ai)
    {
        Agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        swarmController = ai;
        AttackCooldown = 0;
        Agent.speed = swarmController.Stats.moveSpeed;
    }

    public void UpdateChaseSwarm()
    {
        if (AttackCooldown <= 0)
        {
            if (Vector3.Distance(transform.position, swarmController.Player.transform.position) <= swarmController.Stats.attackRange
                && swarmController.TestIfPlayerInSight(transform.position, swarmController.Stats.attackRange))
            {
                JumpAttack();
            }
        }
        else if (Jumping)
        {
            if (isAttacking)
            {
                CheckOverlapSphere();
            }
            
            if(GroundCheck() && rb.velocity.y <= 0)
            {
                JumpAttackDone();
            }
        }
        if(AttackCooldown >= 0)
        {
            AttackCooldown -= Time.deltaTime;
        }
        if(!Agent.isStopped)
        {
            Agent.SetDestination(swarmController.Player.transform.position);
        }


        if (transform.position.y <= -100f)
        {
            GetComponent<Destructible>().Hurt(10000f);
        }

        if (Agent.isOnOffMeshLink)
        {
            Agent.speed = swarmController.Stats.chaseSpeed / 3;
        }
        else
        {
            Agent.speed = swarmController.Stats.chaseSpeed;
        }

        LookAtPlayer();
    }

    public void OnHurt()
    {

    }

    public void OnDeath()
    {
        swarmController.SwarmDied(this);
    }

    private void JumpAttack()
    {
        animator.Play("JumpAttack");
        Agent.isStopped = true;
        Agent.updatePosition = false;
        Agent.updateRotation = false;
        Jumping = true;
        isAttacking = true;
        AttackCooldown = swarmController.Stats.attackSpeed;

        Transform target;

        target = swarmController.Player.transform;
        //target = swarmController.Player.MainCamera.transform;
        //if (target == null)
        //{
        //    target = swarmController.Player.transform;
        //}

        Vector3 jumpdir = ((target.position + new Vector3(0, 1.5f, 0)) - transform.position).normalized;
        rb.AddForce(jumpdir * 9, ForceMode.VelocityChange);
    }

    private void JumpAttackDone()
    {
        Agent.isStopped = false;
        Agent.updatePosition = true;
        Agent.updateRotation = true;
        Jumping = false;
        isAttacking = false;
        animator.Play("JumpAttackLand");
    }

    private void CheckOverlapSphere()
    {
        Collider[] colliders = new Collider[1];
        Physics.OverlapSphereNonAlloc(DamagePosition, damageRadius, colliders, playerLayer);

        if (colliders[0] != null)
        {
            Destructible destructible = colliders[0].gameObject.GetComponent<Destructible>();
            if (destructible != null)
            {
                destructible.Hurt(swarmController.Stats.damage * swarmController.DifficultyMod);
                isAttacking = false;
            }
        }
    }

    private bool GroundCheck()
    {
        if (Physics.Raycast(transform.position, Vector3.down, jumpCheckRange, jumpLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void LookAtPlayer()
    {
        float dist = Vector3.Distance(swarmController.Player.transform.position, transform.position);
        if(Agent.isStopped || Agent.stoppingDistance <= dist)
        {
            transform.LookAt(new Vector3(swarmController.Player.transform.position.x, transform.position.y, swarmController.Player.transform.position.z));
        }
        if (dist > Agent.stoppingDistance)
        {
            animator.Play("Walking");
        }
        else if (!Jumping)
        {
            animator.Play("Idle");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + (transform.forward * damagePosition.z) + (transform.up * damagePosition.y), damageRadius);

        Gizmos.DrawLine(transform.position, Vector3.down * jumpCheckRange);
    }

}
