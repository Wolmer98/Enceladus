using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FB_Projectile", menuName = "Weapon/Fire Behavior/FB_Projectile")]
public class FireBehavior_Projectile : FireBehavior
{
    [Header("Projectile fire settings")]

    [SerializeField] Projectile projectile;
    [SerializeField] float projectileSpeed = 30;

    public override void Fire(Weapon weapon)
    {
        OnShoot.Invoke();

        int projCount = Mathf.Max(0, weapon.WeaponStats.projectileAmount);
        for (int i = 0; i < projCount; i++)
        {
            Projectile spawnedProjectile = Instantiate(projectile, weapon.FirePoint.position, weapon.FirePoint.rotation);
            spawnedProjectile.Damage = weapon.WeaponStats.damage * weapon.ChargeValue;
            spawnedProjectile.FireRange = weapon.WeaponStats.fireRange;
            spawnedProjectile.AOERadius = weapon.WeaponStats.areaOfEffect;
            spawnedProjectile.HitLayerMask = weapon.HitMask;

            Vector3 dir = weapon.FirePoint.forward;

            RaycastHit cameraHit;
            if (Physics.Raycast(weapon.WeaponCamera.transform.position, weapon.WeaponCamera.transform.forward, out cameraHit, Mathf.Infinity, weapon.HitMask))
            {
                //Accuracy
                Vector3 cameraHitPoint = cameraHit.point + (Random.insideUnitSphere * spreadRadius * (1 - Mathf.Clamp01(weapon.WeaponStats.accuracy)));
                dir = Vector3.Normalize(cameraHitPoint - weapon.FirePoint.position);
            }

            Vector3 force = (dir * projectileSpeed); // + playerRigidbody.velocity;
            spawnedProjectile.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
        }
    }

    public override void DrawGizmos()
    {
        
    }

}
