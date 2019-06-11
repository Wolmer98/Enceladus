using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamera : MonoBehaviour
{
    public Transform[] menuLocations;
    private Transform currentLocation;

    
    private float targetZoom;
    private float targetRotate;


    public float moveSpeed = 1f;
    public float zoomSpeed = 1f;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        currentLocation = menuLocations[0];
        targetZoom = cam.orthographicSize;
    }

    void Update()
    {
        var offset = new Vector3(0f, 0, -50f);
        //transform.position = Vector3.MoveTowards(transform.position, currentLocation.position + offset, speed * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, currentLocation.position + offset, moveSpeed * Time.deltaTime);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, zoomSpeed * Time.deltaTime);
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
