using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class Door : MonoBehaviour
{
    public Color lockedColor;
    public Color openColor;


    [Header("Door Settings")]
    [SerializeField] bool isOpen;
    [SerializeField] bool isLocked;

    [SerializeField] float toggleDelay;
    [SerializeField] UnityEvent onToggle;

    [SerializeField] DelayedEventGroup toggleDelayedEventGroup;

    [Header("Other Settings")]
    [FMODUnity.EventRef]
    [SerializeField] string openSound;

    [FMODUnity.EventRef]
    [SerializeField] string toggleSound;
    FMOD.Studio.EventInstance toggleEvent;

    [SerializeField] FMODUnity.StudioEventEmitter emitter;
    [SerializeField] FMODUnity.StudioEventEmitter openEmitter;
    Animator animator;
    Collider[] doorColliders;
    [SerializeField] NavMeshLink navMeshLink;

    [SerializeField] Light[] lockLights;

    [SerializeField] NavMeshObstacle doorNavmeshObstacle;

    WaterManager wm;

    [FMODUnity.EventRef]
    public string doorLockedEvent;
    FMOD.Studio.EventInstance doorLocked;

    public bool IsOpen { get { return isOpen; } }
    public bool IsActive { get; private set; }

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        doorColliders = GetComponentsInChildren<Collider>();
        if (navMeshLink == null)
        {
            navMeshLink = GetComponentInParent<NavMeshLink>();
        }
    }

    private void Start()
    {
        wm = FindObjectOfType<WaterManager>();

        animator.SetBool("IsOpen", isOpen);
        SetLights(isLocked);

        if (!IsOpen && navMeshLink != null)
        {
            navMeshLink.area = NavMesh.GetAreaFromName("Not Walkable");
            //Debug.Log(navMeshLink.area);
            navMeshLink.UpdateLink();
        }

        if (!IsOpen)
        {
            if (doorNavmeshObstacle != null)
            {
                doorNavmeshObstacle.carving = enabled;
                doorNavmeshObstacle.enabled = enabled;
            }
        }
    }

    /// <summary>
    /// Opens the door if it is closed and closes it if it is open.
    /// </summary>
    public void Toggle()
    {
        if (isLocked || IsActive)
        {
            doorLocked = FMODUnity.RuntimeManager.CreateInstance(doorLockedEvent);
            doorLocked.start();
            doorLocked.release();
            return;
        }

        IsActive = true;

        if (emitter != null && toggleDelay > 0)
        {
            emitter.Play();
            emitter.EventInstance.setParameterByName("Elevator Control", 0f);

            emitter.EventInstance.release();
        }

        onToggle.Invoke();
        StartCoroutine(ToggleDoorAfterTime(toggleDelay));
    }

    public void ToggleOverride()
    {
        IsActive = true;

        if (emitter != null && toggleDelay > 0)
        {
            emitter.Play();
            emitter.EventInstance.setParameterByName("Elevator Control", 0f);

            emitter.EventInstance.release();
        }

        onToggle.Invoke();
        StartCoroutine(ToggleDoorAfterTime(toggleDelay));
    }

    private IEnumerator ToggleDoorAfterTime(float time)
    {
        StartCoroutine(toggleDelayedEventGroup.DelayedEventsCoroutine());

        yield return new WaitForSeconds(time);

        if (emitter != null && toggleDelay > 0)
        {
            emitter.EventInstance.setParameterByName("Elevator Control", 1f);
            yield return new WaitForSeconds(3f);
        }

        SetOpen(!isOpen);
    }

    private void SetOpen(bool isOpen)
    {
        openEmitter.Stop();

        if (openEmitter.IsPlaying() == false)
        {
            openEmitter.Play();
            if (wm != null)
            {
                if (wm.PlayerInWater)
                {
                    openEmitter.EventInstance.setParameterByName("TouchingWater", 1f);
                }
                else
                {
                    openEmitter.EventInstance.setParameterByName("TouchingWater", 0f);
                }
            }
            openEmitter.EventInstance.release();
        }

        this.isOpen = isOpen;
        animator.SetBool("IsOpen", isOpen);
        if (isOpen)
            StartCoroutine(ToggleCollidersAfterTime(2f));
        else
            StartCoroutine(ToggleCollidersAfterTime(0));

        if (navMeshLink != null)
        {
            navMeshLink.area = NavMesh.GetAreaFromName("Walkable");
            navMeshLink.UpdateLink();
        }
    }

    public void SetDoorDefaultValues()
    {
        ClearOnToggle();
        toggleDelay = 0;
    }

    public void ClearOnToggle()
    {
        onToggle.RemoveAllListeners();
    }

    public void SetState(bool open)
    {
        IsActive = true;
        SetOpen(open);
    }

    private IEnumerator ToggleCollidersAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        ToggleCollider();
        //transform.GetChild(0).gameObject.SetActive(!transform.GetChild(0).gameObject.activeSelf); 
        GetComponent<StencilReferenceSetter>().enabled = !GetComponent<StencilReferenceSetter>().enabled;
        IsActive = false;
    }

    /// <summary>
    /// Toggles the door collider. 
    /// </summary>
    public void ToggleCollider()
    {
        if (doorColliders == null || doorColliders.Length == 0)
        {
            return;
        }

        foreach (Collider col in doorColliders)
        {
            col.enabled = !col.enabled;
        }


        if (doorNavmeshObstacle != null)
        {
            doorNavmeshObstacle.carving = false;
            doorNavmeshObstacle.enabled = false;
        }
    }

    public void SetLights(bool value)
    {
        if (lockLights == null || lockLights.Length == 0)
        {
            return;
        }

        foreach (Light light in lockLights)
        {
            //light.enabled = value;
            light.color = value ? lockedColor : openColor;
        }

        if (value)
        {
            GetComponent<GlowObject>().GlowColor = Color.black;
        }
    }

    /// <summary>
    /// Locks the door.
    /// </summary>
    public void Lock()
    {
        isLocked = true;
        SetLights(isLocked);
    }

    /// <summary>
    /// Unlocks the door.
    /// </summary>
    public void Unlock()
    {
        isLocked = false;
        SetLights(isLocked);
    }

    private void OnDestroy()
    {
        if (navMeshLink != null)
        {
            navMeshLink.area = NavMesh.GetAreaFromName("Walkable");
            //Debug.Log(navMeshLink.area);
            navMeshLink.UpdateLink();
        }

        if (emitter != null)
            emitter.EventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        if (openEmitter != null)
            openEmitter.EventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        if (doorNavmeshObstacle != null)
        {
            doorNavmeshObstacle.carving = false;
            doorNavmeshObstacle.enabled = false;
        }

    }
}
