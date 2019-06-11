using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FB_Raycast", menuName = "Weapon/Fire Behavior/FB_Raycast")]
public class FireBehavior_Raycast : FireBehavior
{
    [Header("Raycast Settings")]

    [Tooltip("A prefab of effects that will be spawned at the hit point. Needs to kill itself.")]
    [SerializeField]
    GameObject impactEffect;
    [SerializeField]
    GameObject bulletHole;

    Vector3 hitPoint;
    Vector3 startPoint;

    [Header("Debug")]
    [SerializeField] Color gizmosColor = Color.red;

    public override void Fire(Weapon weapon)
    {
        OnShoot.Invoke();

        float fireRange = Mathf.Max(0, weapon.WeaponStats.fireRange);

        bool isContinousLaser = IsContinuosLaser;

        RaycastHit cameraHit;
        if (Physics.Raycast(weapon.WeaponCamera.transform.position, weapon.WeaponCamera.transform.forward, out cameraHit, fireRange, weapon.HitMask))
        {
            int projectileCount = Mathf.Max(0, weapon.WeaponStats.projectileAmount);
            for (int i = 0; i < projectileCount; i++)
            {
                Vector3 hitPoint = new Vector3();
                // Accuracy.
                if (fireMode == FireMode.Automatic && specialFireType == SpecialFireType.Laser)
                {
                    hitPoint = cameraHit.point;
                }
                else
                {
                    hitPoint = cameraHit.point + (Random.insideUnitSphere * spreadRadius * (1 - Mathf.Clamp01(weapon.WeaponStats.accuracy)));
                }

                // Direction Calculation.
                Vector3 dir = (hitPoint - weapon.FirePoint.position).normalized;
                startPoint = weapon.FirePoint.position;

                RaycastHit hit;
                if (Physics.Raycast(startPoint, dir, out hit, fireRange, weapon.HitMask))
                {
                    hitPoint = hit.point;

                    HitDestructible(weapon, hit.transform.gameObject, hit.point);

                    if (impactEffect != null)
                    {
                        Instantiate(impactEffect, hit.point, Quaternion.identity);
                        if (bulletHole != null && hit.transform.GetComponent<Destructible>() == null)
                        {
                            Destroy(Instantiate(bulletHole, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal), hit.transform), 10);
                        }
                    }

                    // Area damage. Goes through all objects around hit point and hurts them (if destructible).
                    float areaOfEffect = Mathf.Max(0, weapon.WeaponStats.areaOfEffect);
                    if (areaOfEffect > 0)
                    {
                        Collider[] colliders = Physics.OverlapSphere(hitPoint, areaOfEffect);
                        foreach (Collider col in colliders)
                        {
                            HitDestructible(weapon, col.gameObject, hit.point);
                        }
                    }

                    LastHitPoint = hitPoint;
                }
            }
        }
        else
        {
            LastHitPoint = weapon.FirePoint.position + weapon.FirePoint.forward * weapon.WeaponStats.fireRange;
        }
    }

    private void HitDestructible(Weapon weapon, GameObject go, Vector3 hitPoint)
    {
        Destructible destructible = go.GetComponentInParent<Destructible>();
        if (destructible != null)
        {
            float damage = IsContinuosLaser ? ((weapon.WeaponStats.damage * weapon.WeaponStats.roundsPerMinute ) / 60) * Time.deltaTime : weapon.WeaponStats.damage * weapon.ChargeValue;
            destructible.Hurt(damage, weapon.WeaponStats.penetration, hitPoint);
        }
    }

    public override void DrawGizmos()
    {
        //Gizmos.color = gizmosColor;
        //Gizmos.DrawLine(startPoint, hitPoint);
    }
}
