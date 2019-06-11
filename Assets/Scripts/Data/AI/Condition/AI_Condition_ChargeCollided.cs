using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Condition_ChargeCollided", menuName = "AI/Condition/ChargeCollided")]
public class AI_Condition_ChargeCollided : AI_Condition
{
    public float raycastRange = 1.5f;
    public float stunDuration = 2f;
    public LayerMask collisionMask;
    public LayerMask damageMask;

    [FMODUnity.EventRef]
    public string collisionObstacleSound;

    public override bool CheckCondition(AI ai)
    {
        return CollidedWithWall(ai);
    }

    private bool CollidedWithWall(AI ai)
    {
        if (!ai.IsStunned)
        {
            RaycastHit hit;
            if (Physics.SphereCast(ai.transform.position + Vector3.up * 0.8f, 0.2f, ai.transform.forward, out hit, raycastRange, collisionMask))
            {
                if (hit.transform.tag != "Player")
                {
                    ai.IsStunned = true;
                    ai.Animator.SetTrigger("Stunned");
                    if (!ai.SetCTimer)
                    {
                        ai.SetCTimer = true;
                        ai.ConditionTime = 0.0f;
                        FMODUnity.RuntimeManager.PlayOneShot(collisionObstacleSound, ai.transform.position);
                    }
                }
                if (hit.transform.tag == "Player")
                {
                    DealDamage(ai,hit);
                    FMODUnity.RuntimeManager.PlayOneShot(ai.attackSound, ai.transform.position);
                    ai.Animator.SetTrigger("HitPlayer");
                    ai.AttackCooldown = 0;
                    PlayerController pc = hit.transform.gameObject.GetComponent<PlayerController>();
                    if(pc)
                    {
                        CameraShake cameraShake =  pc.MainCamera.GetComponent<CameraShake>();
                        if(cameraShake)
                        {
                            cameraShake.InitCameraShake();
                        }
                        else
                        {
                            Debug.Log("Camera shake was null after charge hit");
                        }
                    }
                    else
                    {
                        Debug.Log("PC was null after charge hit");
                    }
                    return true;
                }
            }
            if (ai.HasCharged)
            {
                ai.Agent.nextPosition = ai.Rigidbody.position;
                ai.Animator.ResetTrigger("Stunned");
                return true;
            }
        }
        return false;

    }

    private void DealDamage(AI ai, RaycastHit hit)
    {
        Destructible destructible = hit.transform.gameObject.GetComponent<Destructible>();
        if (destructible != null)
        {
            if (ai.Enrage)
            {
                destructible.Hurt(ai.Stats.damage * ai.Stats.damageMultiplier * ai.DifficultyMod);
            }
            else
            {
                destructible.Hurt(ai.Stats.damage * ai.DifficultyMod);
            }
        }
    }

}
