using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public float scale = 0.5f;
    public float smoothSpeed = 0.125f;
    public Vector3 cameraOffset;
    public Vector3 angularOffset;

    void Update()
    {
        cameraOffset.y -= Input.mouseScrollDelta.y * scale;
        cameraOffset.x += Input.mouseScrollDelta.y * scale * 0.5f;
    }

    // Update is called once per frame
    void LateUpdate() {
        Vector3 desiredPosition = target.position + cameraOffset;
        Vector3 smoothedPosition = Vector3.Slerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}