using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Joystick : MonoBehaviour
{
    private Vector2 startPoint;
    private Vector2 currentPoint;

    [SerializeField]
    private float maxHandleDistance;
    [SerializeField]
    private RectTransform joystickBKG;
    [SerializeField]
    private RectTransform joystickHandle;

    private float currentDistance;

    RectTransform thisRect;
    private void Start()
    {
        thisRect = GetComponent<RectTransform>();
    }
    private void Update()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began) startPoint = Input.GetTouch(0).position;
            if (RectTransformUtility.RectangleContainsScreenPoint(thisRect, startPoint))
            {

                currentPoint = Input.GetTouch(0).position;

                currentDistance = Mathf.Min(Vector2.Distance(startPoint, currentPoint), maxHandleDistance);

                joystickBKG.position = startPoint;
                joystickHandle.position = startPoint + (GetJoystickAxis() * currentDistance);
            }
            else currentDistance = 0;

            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                currentPoint = Vector2.zero;
                currentDistance = 0;
                joystickBKG.localPosition = Vector2.zero;
                joystickHandle.localPosition = Vector2.zero;
            }
        }
    }

    public Vector2 GetJoystickAxis()
    {
        if (Input.touchCount > 0) return (currentPoint - startPoint).normalized * currentDistance/maxHandleDistance;
        else return Vector2.zero;
    }
}
