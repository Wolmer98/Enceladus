using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationHandler : MonoBehaviour
{
    [SerializeField] Vector3 axis = new Vector3(0, 0, 1);
    [SerializeField] float speed = 1f;

    private void Update()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + axis * speed * Time.deltaTime);
    }
}
