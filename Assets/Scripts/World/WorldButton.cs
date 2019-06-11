using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WorldButton : MonoBehaviour
{
    [SerializeField] bool malfunctionOnPress;
    [SerializeField] float functionDelay;

    [FMODUnity.EventRef]
    public string buttonPressSound;

    public DelayedEvent[] onPressEvents;

    Animator animator;

    public bool IsBroken { get; private set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Presses the button, triggering its events and animations.
    /// </summary>
    public void Press()
    {
        if (IsBroken)
        {
            return;
        }

        StartCoroutine(PressAfterTime(functionDelay));
    }

    IEnumerator PressAfterTime(float time)
    {
        if (buttonPressSound != null)
        {
            FMODUnity.RuntimeManager.PlayOneShot(buttonPressSound, transform.position);
        }

        if (animator)
        {
            animator.SetTrigger("Press");
        }

        if (malfunctionOnPress)
        {
            Malfunction();
        }

        // Delayed Events
        if (onPressEvents != null && onPressEvents.Length > 0)
        {        
            for (int i = 0; i < onPressEvents.Length; i++)
            {
                yield return new WaitForSeconds(onPressEvents[i].delay);
                onPressEvents[i].uEvent.Invoke();

                if (onPressEvents == null)
                {
                    break;
                }
            }
        }
    }

    public void RemoveAllEvents()
    {
        onPressEvents = null;
    }

    /// <summary>
    /// Malfunctions the button. It can not be pressed and there is no highlight on hover.
    /// </summary>
    public void Malfunction()
    {
        GetComponentInChildren<GlowObject>().GlowColor = Color.black;
        IsBroken = true;
    }
}
