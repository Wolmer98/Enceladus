using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    public Transform[] menuLocations;
    private Transform currentLocation;

    private float targetZoom;
    private float targetRotate;

    public float moveSpeed = 1f;
    public float rotationSpeed = 1f;
    public float zoomSpeed = 1f;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();

        currentLocation = menuLocations[0];
        targetZoom = cam.transform.position.z;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, currentLocation.position, Time.deltaTime * moveSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, currentLocation.rotation, Time.deltaTime * rotationSpeed);
    }

    public void MoveToLocation(int index)
    {
        currentLocation = menuLocations[index];
    }

    public void SetZoom(int TargetZoom)
    {
        targetZoom = TargetZoom;
    }
}
