using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEngine : MonoBehaviour
{
    public Transform planeTransform;
    public float rotationSpeed = 1; // = 10° / sec, adjust value in inspector

    void Update()
    {
        transform.RotateAround(planeTransform.position, transform.right, Input.GetAxis("Mouse Y") * rotationSpeed);
        transform.RotateAround(planeTransform.position, transform.up, Input.GetAxis("Mouse X") * rotationSpeed);
    }
}
