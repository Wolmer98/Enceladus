using UnityEngine;
using UnityEngine.Events;

public class EventCaller : MonoBehaviour
{
    public UnityEvent unityEvent;

    /// <summary>
    /// Calls the UnityEvent on the EventCaller.
    /// </summary>
    public void EventCaller_CallEvent()
    {
        unityEvent.Invoke();
    }
}
