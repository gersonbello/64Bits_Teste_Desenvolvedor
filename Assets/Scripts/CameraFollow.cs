using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    [SerializeField]
    private float cameraMoveSpeed;

    [SerializeField]
    private Vector3 cameraOffset;
    private Vector3 desiredPosition;

    private void FixedUpdate()
    {
        desiredPosition = target.transform.position + cameraOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, cameraMoveSpeed);
    }
}
