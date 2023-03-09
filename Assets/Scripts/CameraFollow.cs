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
}
