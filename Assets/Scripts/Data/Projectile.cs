using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] GameObject impactEffect;
    [SerializeField] float impactRadius = 0.25f;

    public float Damage { get; set; }
    public float FireRange { get; set; }
    public float AOERadius { get; set; }
    public LayerMask HitLayerMask { get; set; }

    Vector3 lastPos;
    Vector3 originalPos;

    private void Start()
    {
        originalPos = transform.position;
    }

    private void Update()
    {
        //if (lastPos != Vector3.zero)
        //{
        //    RaycastHit rayHitInfo;
        //    Vector3 dir = Vector3.Normalize((transform.position - lastPos));
        //    if (Physics.Raycast(lastPos, dir, out rayHitInfo, Vector3.Distance(lastPos, transform.position), HitLayerMask) || Vector3.Distance(originalPos, transform.position) >= 30)
        //    {
        //        Impact();
        //    }
        //}
        //lastPos = transform.position;
    }

    private void Impact()
    {

        float radius = Mathf.Max(impactRadius, AOERadius);

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, HitLayerMask);

        foreach (Collider c in colliders)
        {
            Destructible destro = c.GetComponentInParent<Destructible>();
            if (destro != null)
            {
                destro.Hurt(Damage);
            }
        }

        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (HitLayerMask == (HitLayerMask | (1 << other.gameObject.layer)))
        {
            Impact();
        }
    }
}
