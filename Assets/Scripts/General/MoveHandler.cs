using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveHandler : MonoBehaviour
{
    public enum MoveType { Axis, Forward, Back, Right, Left }

    [SerializeField] MoveType moveType;
    [SerializeField] Vector3 axis = new Vector3(1, 0, 0);
    [SerializeField] float speed = 1f;

    private void Update()
    {
        switch (moveType)
        {
            case MoveType.Axis:
                transform.position += axis * speed * Time.deltaTime;
                break;
            case MoveType.Forward:
                transform.position += transform.forward * speed * Time.deltaTime;
                break;
            case MoveType.Back:
                transform.position -= transform.forward * speed * Time.deltaTime;
                break;
            case MoveType.Right:
                transform.position += transform.right * speed * Time.deltaTime;
                break;
            case MoveType.Left:
                transform.position -= transform.right * speed * Time.deltaTime;
                break;
        }     
    }
}
