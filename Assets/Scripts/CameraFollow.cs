using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    [SerializeField]
    private float cameraMoveSpeed;

    [SerializeField]
    private Vector3 cameraOffset;
    [SerializeField]
    private Vector3 currentCameraOffset;
    private Vector3 desiredPosition;

    private void Start()
    {
        currentCameraOffset = cameraOffset;
    }

    private void FixedUpdate()
    {
        desiredPosition = target.transform.position + currentCameraOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, cameraMoveSpeed * Time.deltaTime * Mathf.Max(1, Vector3.Distance(desiredPosition, transform.position)));
    }

    public void SetNewOffset(int newOffsetNumber)
    {
        float correctOffset = Mathf.Min(newOffsetNumber, 10);
        currentCameraOffset = cameraOffset - correctOffset * transform.forward;
    }

    public IEnumerator CameraShake(float shakeTime, float shakeAmount)
    {
        float currentTime = 0;
        while(currentTime < shakeTime)
        {
            transform.position += new Vector3(
                UnityEngine.Random.Range(-shakeAmount, shakeAmount) ,
                UnityEngine.Random.Range(-shakeAmount, shakeAmount), 0);
            currentTime += Time.deltaTime;
            yield return null;
        }
    }
}
