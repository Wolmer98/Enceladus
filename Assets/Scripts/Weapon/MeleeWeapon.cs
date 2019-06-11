using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum MeleeWeaponType { Light, Heavy}

public class MeleeWeapon : MonoBehaviour
{
    [SerializeField] MeleeWeaponType weaponType;

    [SerializeField] string weaponName;

    [SerializeField] bool isDamageActive;
    [SerializeField] bool isReturned = true;
    Collider hitTrigger;

    [Header("Stats")]
    [SerializeField] float damage = 5;
    [SerializeField] float range = 2;
    [SerializeField] float throwForce = 5;
    [SerializeField] float attackSpeed = 1; // Convert to cooldown
    [SerializeField] float stunChance = 0.3f;
    [SerializeField] int durability = 20;

    [Space]
    [SerializeField] GameObject weaponVisuals;

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] hitAudioClips;

    public UnityEvent OnHit;
    public UnityEvent OnDestroy;

    public MeleeWeaponType WeaponType { get { return weaponType; } }

    public string WeaponName { get { return weaponName; } }
    public GameObject WeaponVisuals { get { return weaponVisuals; } }
    public bool IsDamageActive { get { return isDamageActive; } }
    public bool IsReturned { get { return isReturned; } }

    public float Damage { get { return damage; } }
    public float ThrowForce { get { return throwForce; } }
    public float AttackSpeed { get { return attackSpeed; } }
    public float StunChance { get { return stunChance; } }
    public int Durability { get { return durability; } set { durability = value; } }

    bool isThrown;
    Animator animator;
    Camera cam;

    private void Awake()
    {
        cam = Camera.main;
        animator = GetComponentInParent<Animator>();
        hitTrigger = GetComponent<Collider>();
    }

    private void Start()
    {
        if (!isThrown)
        {           
            isDamageActive = false;
        }

        isReturned = true;
    }

    private void AttackRay()
    {
        RaycastHit hit;
        Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range);

        if (isDamageActive)
        {
            OnHit.Invoke();

            Destructible destructible = hit.transform.GetComponentInParent<Destructible>();
            PlayerController pc = hit.transform.GetComponentInParent<PlayerController>();
            if (destructible != null && pc == null)
            {
                destructible.Hurt(damage);

                durability -= 1;
                durability = Mathf.Max(0, durability);
                if (durability == 0)
                {
                    // The weapon breaks!
                    OnDestroy.Invoke();
                    Destroy(gameObject);
                }
            }
        }

        if (isThrown)
        {
            SetAttackMode(false);
        }

        if (hitAudioClips.Length > 0)
        {
            AudioClip randomClip = hitAudioClips[Random.Range(0, hitAudioClips.Length)];
            audioSource.PlayOneShot(randomClip);
        }
    }

    public void SetAttackMode(bool value)
    {
        if (hitTrigger == null)
        {
            hitTrigger = GetComponent<Collider>();
        }

        isDamageActive = value;
        hitTrigger.enabled = value;

        EndSwing();

    }

    /// <summary>
    /// Plays the attack animation, enables hit trigger.
    /// </summary>
    public void Attack()
    {
        if (isReturned)
        {
            isReturned = false;
            SetAttackMode(true);
            animator.SetTrigger("MeleeAttack");
            AttackRay();
        }
    }

    public void Throw()
    {
        SetAttackMode(true);
        isThrown = true;
    }

    /// <summary>
    /// Creates a random weapon from the prefab.
    /// </summary>
    public void InitRandomWeapon()
    {

    }

    /// <summary>
    /// End Swing evemt. Is called when weapon is almost returned to idle position. Sets IsReturned to true.
    /// </summary>
    public void EndSwing()
    {
        isReturned = true;
    }

    /// <summary>
    /// Sets the weapon layers to the given integer value.
    /// </summary>
    public void SetWeaponLayers(int value)
    {
        weaponVisuals.layer = value;
        for (int i = 0; i < weaponVisuals.transform.childCount; i++)
        {
            weaponVisuals.transform.GetChild(i).gameObject.layer = value;
        }
    }
}
