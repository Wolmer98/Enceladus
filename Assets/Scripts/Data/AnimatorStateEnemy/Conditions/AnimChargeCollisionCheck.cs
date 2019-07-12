using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimConditionChargeCollisionCheck", menuName = "AI/AnimCondition/ChargeCollisionCheck")]
public class AnimChargeCollisionCheck : AI_Condition
{
    public float raycastRange = 2f;
    public float raycastRadius = 0.5f;
    public LayerMask collisionMask;

    [FMODUnity.EventRef]
    public string collisionObstacleSound;

    public override bool CheckCondition(AI ai)
    {

        RaycastHit hit;
        if (Physics.SphereCast(new Vector3(ai.transform.position.x, ai.transform.position.y + ai.DamagePosition.y, ai.transform.position.z), raycastRadius, ai.transform.forward, out hit, raycastRange, collisionMask))
        {
            if (hit.transform.tag != "Player")
            {
                FMODUnity.RuntimeManager.PlayOneShot(collisionObstacleSound, ai.transform.position);
                return true;
            }
            if (hit.transform.tag == "Player")
            {
                if (ai.IsAttacking)
                {
                    DealDamage(ai, hit);
                    FMODUnity.RuntimeManager.PlayOneShot(ai.attackSound, ai.transform.position);
                    //ai.Animator.SetTrigger("HitPlayer");

                    CameraShake cameraShake = ai.Player.MainCamera.gameObject.GetComponent<CameraShake>();
                    if (cameraShake)
                    {
                        cameraShake.InitCameraShake();
                    }
                    ai.StateMachine.SetTrigger("Chase");
                }
                ai.IsAttacking = false;
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
