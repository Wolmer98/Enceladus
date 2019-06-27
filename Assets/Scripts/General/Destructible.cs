using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Destructible : MonoBehaviour
{
    [SerializeField]
    protected bool destructible = true;

    [SerializeField]
    protected bool destroyOnDeath = true;

    [SerializeField]
    protected bool canDrown = false;

    [SerializeField]
    protected float maxHealth = 100f;
    public float MaxHealth { get { return maxHealth; } }

    [SerializeField]
    protected float health = 100f;
    public float Health { get { return health; } }

    [SerializeField] protected float resistance;
    public float Resistance { get { return resistance; } }

    [SerializeField] protected float regeneration;
    public float Regeneration { get { return regeneration; } }

    [SerializeField] protected float regenerationInterval = 5;
    float regenerationTimer = 0;

    [Header("Drown Settings")]
    [SerializeField] protected float drownDamageInterval = 5;
    [SerializeField] protected float intervalIncreaseRate = 1.2f;
    [SerializeField] protected float drownDamage = 10;
    float drownInterval = 0;
    float drownTimer = 0;
    WaterManager wm;

    [Space]
    [SerializeField] GameObject hitEffect;

    // NOT USED
    private int factionID;

    public UnityEvent OnHurt;

    public UnityEvent OnDeath;

    [Header("Sound Settings")]
    [FMODUnity.EventRef]
    public string onHurtSound;
    FMOD.Studio.EventInstance onHurtEvent;

    [FMODUnity.EventRef]
    public string onDeathSound;
    FMOD.Studio.EventInstance onDeathEvent;

    protected bool isAlive = true;

    CameraShake camShake;

    private void OnValidate()
    {
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        else if (health < 0)
        {
            health = 0;
        }

        if (maxHealth < 0)
        {
            maxHealth = 0;
        }
    }

    public void Hurt(float damage, float penetration = 0, Vector3 hitPosition = new Vector3())
    {
        if (isAlive == false)
        {
            return;
        }

        FMODUnity.RuntimeManager.PlayOneShot(onHurtSound, transform.position);

        if (camShake == null)
        {
            camShake = FindObjectOfType<CameraShake>();
        }
        else
        {
            camShake.InitCameraShake(0.2f, 0.1f, 30);
        }

        float calcResistance = (resistance - penetration) / 100; //10 resistance = 10% damage reduction.
        if (calcResistance < 0)
        {
            calcResistance = 0;
        }

        damage *= (1 - calcResistance);
        damage = Mathf.Max(1, damage);

        if (destructible)
        {
            if (hitEffect != null)
            {
                hitEffect.transform.position = hitPosition;
            }
            OnHurt.Invoke();

            if (health - damage > 0)
            {
                health -= damage;
            }
            else
            {
                health = 0;
                Die();
            }
        }
    }

    public void Heal(float value)
    {
        if (health + value < maxHealth)
        {
            health += value;
        }
        else
        {
            health = maxHealth;
        }
    }

    public void AddMaxHealth(float value)
    {
        maxHealth = Mathf.Max(1, maxHealth + value);
    }

    public void AddRegeneration(float value)
    {
        regeneration = Mathf.Max(0, regeneration + value);
    }

    public void AddResistance(float value)
    {
        resistance = Mathf.Max(0, resistance + (value * Mathf.Max(0, (1 - (resistance / 100)))));
    }

    private void Update()
    {
        UpdateRegeneration();

        if (canDrown)
        {
            UpdateDrowning();
        }
    }

    private void UpdateRegeneration()
    {
        regenerationTimer += Time.deltaTime;

        if (regenerationTimer >= regenerationInterval)
        {
            Heal(regeneration);
            regenerationTimer = 0;
        }
    }

    private void UpdateDrowning()
    {
        if (wm == null)
        {
            wm = FindObjectOfType<WaterManager>();
            drownInterval = drownDamageInterval;
            return;
        }

        if (wm.PlayerBeneathWater)
        {
            drownTimer += Time.deltaTime;
            if (drownTimer >= drownInterval)
            {
                drownInterval /= intervalIncreaseRate;
                drownInterval = Mathf.Max(drownInterval, 0.5f);
                drownTimer = 0;

                Hurt(drownDamage + resistance);
            }
        }
        else
        {
            drownTimer = 0;
            drownInterval = drownDamageInterval;
        }
    }

    protected virtual void Die()
    {
        OnDeath.Invoke();

        FMODUnity.RuntimeManager.PlayOneShot(onDeathSound, transform.position);

        isAlive = false;
        //gameObject.SetActive(false);
        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
    }

    public bool IsAlive
    {
        get
        {
            return isAlive;
        }
    }

    public int FactionID
    {
        get
        {
            return factionID;
        }
    }

    public bool IsEnemy(int factionID)
    {
        return (factionID != this.factionID)
            && (factionID != 0)
            && (this.factionID != 0);
    }
}
