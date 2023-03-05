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
        transform.position = Vector3.Lerp(transform.position, desiredPosition, cameraMoveSpeed);
    }

    public void SetNewOffset(int newOffsetNumber)
    {
        float correctOffset = Mathf.Min(newOffsetNumber, 10);
        currentCameraOffset = cameraOffset + new Vector3(0, correctOffset, -correctOffset);
    }
}
