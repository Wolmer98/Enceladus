using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    [SerializeField] FPSController fpsController;
    [SerializeField] Weapon weapon;
    [SerializeField] Animator weaponAnimator;
    [SerializeField] Animator playerAnimator;


    bool Idle;
    bool Walking;
    bool Running;

    private void Start()
    {
        playerAnimator.enabled = false;
    }

    void Update()
    {
        if (fpsController.IsSprinting)
        {
            ResetBoolsRegardingExclusion(ref Running);
            weaponAnimator.SetFloat("RunningSpeed", fpsController.Stamina / fpsController.MaxStamina);
        }
        else if (fpsController.Velocity.magnitude > 0.1f)
        {
            ResetBoolsRegardingExclusion(ref Walking);
        }
        else
        {
            ResetBoolsRegardingExclusion(ref Idle);
        }

        weaponAnimator.SetBool("Idle", Idle);
        weaponAnimator.SetBool("Walking", Walking);
        weaponAnimator.SetBool("Running", Running);
    }

    private void ResetBoolsRegardingExclusion(ref bool excludedBool)
    {
        Idle = false;
        Walking = false;
        Running = false;

        excludedBool = true;
    }

    public void ToggleMeleeWeapon(int value, MeleeWeaponType weaponType)
    {
        weaponAnimator.SetFloat("Melee", value);

        if (weaponType == MeleeWeaponType.Heavy)
        {
            weaponAnimator.SetFloat("HeavyMelee", 1);
        }
        else
        {
            weaponAnimator.SetFloat("HeavyMelee", 0);
        }
    }

    public void PlayDeathAnimation()
    {
        playerAnimator.enabled = true;
        playerAnimator.SetBool("Dead", true);
    }
}
