using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Pickup : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string pickupSound;

    [SerializeField] string pickupName;
    [SerializeField] string pickupText;

    Rigidbody rb;

    bool initialized;

    public string PickupName { get { return pickupName; } }
    public string PickupText { get { return pickupText; } }

    public Rigidbody RB { get { return rb; } }
    public Transform ItemParent { get; private set; }

    protected void Start()
    {
        if (!initialized)
        {
            InitPickup();
        }
    }

    protected virtual void InitPickup()
    {
        rb = GetComponent<Rigidbody>();
        ItemParent = transform.GetChild(0);
        initialized = true;
    }

    /// <summary>
    /// Adds the picked up values to the player.
    /// </summary>
    public virtual bool PickUp(PlayerController pc, bool pickUpIsPrefab = false)
    {
        if (pickUpIsPrefab == false)
        {
            FMODUnity.RuntimeManager.PlayOneShot(pickupSound, transform.position);
            Destroy(gameObject);
        }
        return true;
    }
}
