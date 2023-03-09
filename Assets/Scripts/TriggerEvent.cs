using UnityEngine.Events;
using UnityEngine;

public class TriggerEvent : MonoBehaviour
{
    public UnityEvent enterTriggerEvents, exitTriggerEvents;
    public void EnterTriggerEvents()
    {
        enterTriggerEvents.Invoke();
    }

    public void ExitTriggerEvents()
    {
        exitTriggerEvents.Invoke();
    }
}
