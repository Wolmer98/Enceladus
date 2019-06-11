using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicWeaponSway : MonoBehaviour
{
    [SerializeField]
    private float amount;
    [SerializeField]
    private float smoothAmount;
    [SerializeField]
    private float maxAmount;

    private Vector3 initialPosition;
    private FPSController fpsController;

    // Start is called before the first frame update
    void Start()
    {
        fpsController = GetComponentInParent<FPSController>();
        initialPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (fpsController.IsFrozen == false)
        {
            float movementX = -Input.GetAxis("Mouse X") * amount;

            float movementY = -Input.GetAxis("Mouse Y") * amount;
            movementX = Mathf.Clamp(movementX, -maxAmount, maxAmount);
            movementY = Mathf.Clamp(movementY, -maxAmount, maxAmount);

            Vector3 finalPosition = new Vector3(movementX, movementY, 0);
            transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialPosition, Time.deltaTime * smoothAmount);
        }
    }
}
