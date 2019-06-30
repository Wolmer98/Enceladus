using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateUtility : MonoBehaviour
{
    private void Start()
    {
        FindObjectOfType<WorldGenerator>().OnWorldStart.AddListener(delegate { Init(); });
    }

    PlayerController player;

    void Init()
    {
        player = FindObjectOfType<PlayerController>();
    }

    public void InstantiateObject(GameObject gameObject)
    {
        Instantiate(gameObject, transform.position, transform.rotation);
    }

    public void InstantiateRigidbody(GameObject gameObject)
    {
        if (player == null)
            player = FindObjectOfType<PlayerController>();

        float damage = player.MainWeapon.WeaponStats.damage;

        Rigidbody rb = GetComponent<Rigidbody>();
        GameObject instantiatedObject = Instantiate(gameObject, transform.position, transform.rotation);
        Rigidbody[] rbArray = instantiatedObject.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rigidBody in rbArray)
        {
            rigidBody.velocity = rb.velocity.normalized;
            rigidBody.AddForce(damage * 0.01f * (player.transform.position - instantiatedObject.transform.position), ForceMode.Impulse);
        }
    }
}
