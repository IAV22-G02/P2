using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public float smoothSpeed = 0.125f;
    public Vector3 cameraOffset;
    public Vector3 angularOffset;

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 desiredPosition = target.position + cameraOffset;
        Vector3 smoothedPosition = Vector3.Slerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        transform.LookAt(target);
        transform.rotation *= Quaternion.Euler(angularOffset);
    }
}