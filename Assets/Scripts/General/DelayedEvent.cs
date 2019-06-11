using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DelayedEvent
{
    public UnityEvent uEvent;
    public float delay = 0f;
}

[System.Serializable]
public class DelayedEventGroup
{
    public DelayedEvent[] delayedEvents;

    public void RunEvents(MonoBehaviour instance)
    {
        instance.StartCoroutine(DelayedEventsCoroutine());
    }

    public IEnumerator DelayedEventsCoroutine()
    {
        if (delayedEvents != null && delayedEvents.Length > 0)
        {
            for (int i = 0; i < delayedEvents.Length; i++)
            {
                yield return new WaitForSeconds(delayedEvents[i].delay);
                delayedEvents[i].uEvent.Invoke();
            }
        }

        yield return null;
    }
}
