using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateUtility : MonoBehaviour
{
    public void InstantiateObject(GameObject gameObject)
    {
        Instantiate(gameObject, transform.position, transform.rotation);
    }

    public void InstantiateRigidbody(GameObject gameObject)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Rigidbody[] rbArray = Instantiate(gameObject, transform.position, transform.rotation).GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rigidBody in rbArray)
        {
            rigidBody.velocity = rb.velocity.normalized;
        }
    }
}
