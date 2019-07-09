using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum WeaponType { Bullet, Laser, Plasma }

[System.Serializable]
public class WeaponStats
{
    [Header("Damage")]
    public float damage;
    [Tooltip("Accuracy is a float value between 0 and 1.")]
    [Range(0, 1)]
    public float accuracy;
    public float penetration;

    [Header("Projectile")]
    public int projectileAmount;
    public float projectileSpeed;

    [Header("Fire Settings")]
    public float fireRange;
    public float areaOfEffect;
    public float roundsPerMinute;
    public float reloadSpeed;
    public float fireDelay;

    [Header("Failure")]
    [Tooltip("Failure Rate is a float value between 0 and 1.")]
    [Range(0, 1)]
    public float failureRate;
    public float restartTime;

    [Header("Ammunition")]
    public float magSize;

    public float ShotCooldown
    {
        get
        {
            return 1f / (Mathf.Max(0, roundsPerMinute) / 60f);
        }
    }


    [Header("Charge-Up")]
    public bool chargeUp = false;
    public float chargeUpTime;
    [Range(0, 1)]
    [Tooltip("If the chargeup is (in percentage) below this value, the weapon will shutdown.")]
    public float minimumChargeUp;

    [Header("Hard Caps")]
    [SerializeField] WeaponHardCaps hardCaps;

    private int inverseMultiplier = 1;
    private bool capOverride = false;

    /// <summary>
    /// Applies the given weaponStats to this weaponStats (Increments all variables).
    /// </summary>
    public void ApplyWeaponStats(WeaponStats weaponStats, bool decreaseStats = false, bool capOverride = false, bool isMajorMod = false)
    {

        inverseMultiplier = decreaseStats ? -1 : 1;
        this.capOverride = capOverride;

        damage = AssignStat(damage, weaponStats.damage, hardCaps.MinCaps.damage, hardCaps.MaxCaps.damage);
        accuracy = AssignStat(accuracy, weaponStats.accuracy, hardCaps.MinCaps.accuracy, hardCaps.MaxCaps.accuracy);
        penetration = AssignStat(penetration, weaponStats.penetration, hardCaps.MinCaps.penetration, hardCaps.MaxCaps.penetration);

        projectileAmount = (int)AssignStat(projectileAmount, weaponStats.projectileAmount, hardCaps.MinCaps.projectileAmount, hardCaps.MaxCaps.projectileAmount);
        projectileSpeed = AssignStat(projectileSpeed, weaponStats.projectileSpeed, hardCaps.MinCaps.projectileSpeed, hardCaps.MaxCaps.projectileSpeed);

        fireRange = AssignStat(fireRange, weaponStats.fireRange, hardCaps.MinCaps.fireRange, hardCaps.MaxCaps.fireRange);
        areaOfEffect = AssignStat(areaOfEffect, weaponStats.areaOfEffect, hardCaps.MinCaps.areaOfEffect, hardCaps.MaxCaps.areaOfEffect);
        roundsPerMinute = AssignStat(roundsPerMinute, weaponStats.roundsPerMinute, hardCaps.MinCaps.roundsPerMinute, hardCaps.MaxCaps.roundsPerMinute);
        reloadSpeed = AssignStat(reloadSpeed, weaponStats.reloadSpeed, hardCaps.MinCaps.reloadSpeed, hardCaps.MaxCaps.reloadSpeed);
        fireDelay = AssignStat(fireDelay, weaponStats.fireDelay, hardCaps.MinCaps.fireDelay, hardCaps.MaxCaps.fireDelay);

        failureRate = AssignStat(failureRate, weaponStats.failureRate, hardCaps.MinCaps.failureRate, hardCaps.MaxCaps.failureRate);
        restartTime = AssignStat(restartTime, weaponStats.restartTime, hardCaps.MinCaps.restartTime, hardCaps.MaxCaps.restartTime);

        magSize = AssignStat(magSize, weaponStats.magSize, hardCaps.MinCaps.magSize, hardCaps.MaxCaps.magSize);

        if (isMajorMod)
        {
            chargeUp = weaponStats.chargeUp;
            chargeUpTime = AssignStat(chargeUpTime, weaponStats.chargeUpTime, hardCaps.MinCaps.chargeUpTime, hardCaps.MaxCaps.chargeUpTime);
            minimumChargeUp = AssignStat(minimumChargeUp, weaponStats.minimumChargeUp, hardCaps.MinCaps.minimumChargeUp, hardCaps.MaxCaps.minimumChargeUp);
        }
    }

    private float AssignStat(float baseValue = 0, float value = 0, float minCap = 0, float maxCap = Mathf.Infinity)
    {
        if (capOverride == false)
        {
            return Mathf.Clamp(baseValue + (value * inverseMultiplier), minCap, maxCap);
        }
        else
        {
            return baseValue + (value * inverseMultiplier);
        }
    }
}

public class Weapon : MonoBehaviour
{
    [SerializeField] Transform firePoint;
    [SerializeField] Transform[] firePoints;
    [SerializeField] LayerMask hitMask;

    [Header("Shutdown Settings")]
    [SerializeField] GameObject shutdownEffect;
    [FMODUnity.EventRef]
    [SerializeField] string shutdownSound;

    [Header("Mods")]
    [SerializeField] WeaponMod mainWeaponMod;
    [SerializeField] List<WeaponMod> currentMods;

    [Space]
    [SerializeField] FireBehavior fireBehavior;
    [SerializeField] WeaponStats weaponStats;

    float shotTimer;
    float fireDelayTimer;
    bool fireDelay;

    float currentRecoil;
    Vector3 camStartRot;

    bool canShoot = true;
    bool online = true;
    bool didFireThisFrame;

    bool reloading = false;
    float reloadTimer;

    float chargeTimer;
    float chargeValue;

    float shutdownTimer;
    bool shutdown;

    [Header("Ammo")]
    [SerializeField] float currentMagAmmo;
    [SerializeField] float storageAmmo;
    [SerializeField] float maxStorageAmmo;

    [Header("Audio")]
    [SerializeField] FMODUnity.StudioEventEmitter firePointAudio;
    [SerializeField] FMODUnity.StudioEventEmitter weaponAmbienceAudio;

    [Header("Animations")]
    [SerializeField] Animator mainAnimator;
    [SerializeField] AnimationClip reloadClip;

    [Header("Weapon Emmision Assets")]
    [SerializeField] Renderer weaponRenderer;
    [SerializeField] Text[] ammoTexts;
    [SerializeField] Image[] chargeUpImages;
    [SerializeField] Light[] emissionLights;

    [Header("Charge Failed Audio")]
    [FMODUnity.EventRef]
    public string chargeUpFailedSound;

    [Header("Suppression Audio")]
    [FMODUnity.EventRef]
    public string suppressionSound;
    FMOD.Studio.EventInstance suppressionInstance;
    [SerializeField] FMODUnity.StudioEventEmitter suppressionEmitter;
    bool canDecreaseSuppressionCoroutine = false;
    float currentSuppression = 0;
    Coroutine currentSuppressionRoutine;

    [Header("Charge Projectile Effect")]
    [SerializeField] GameObject chargeUpProjectile;
    [SerializeField] GameObject chargeUpEffect;
    Vector3 projectileStartScale;

    [Header("Rigs")]
    [SerializeField] GameObject[] weaponRigs;

    // Line renderer.
    LineRenderer lineRenderer;
    float lineRendererTimer;
    const float lrLifeTime = 0.05f;

    GameObject muzzleFlashParent;
    ParticleSystem[] currentMuzzleFlashes;

    GameObject chargeUpParent;
    ParticleSystem[] currentChargeUps;

    CameraShake camShake;
    FPSController fpsController;

    UIController uiController;

    float laserFailureTick;

    public UnityEvent OnShoot;

    public Camera WeaponCamera { get; private set; }

    public Transform FirePoint { get { return firePoint; } }
    public LayerMask HitMask { get { return hitMask; } }

    public WeaponMod MainWeaponMod { get { return mainWeaponMod; } set { mainWeaponMod = value; } }
    public WeaponMod[] WeaponMods { get { return currentMods.ToArray(); } }

    public FireBehavior FireBehavior { get { return fireBehavior; } }
    public WeaponStats WeaponStats { get { return weaponStats; } }

    public bool Online { get { return online; } }
    public bool CanShoot { get { return canShoot; } }

    public float CurrentMagAmmo { get { return currentMagAmmo; } }
    public float StorageAmmo { get { return storageAmmo; } }
    public float MaxStorageAmmo { get { return maxStorageAmmo;  } }

    public float ChargeValue
    {
        get
        {
            if (weaponStats.chargeUp)
                return Mathf.Max(0.1f, chargeValue);
            else
                return 1;
        }
    }

    public LineRenderer LineRenderer { get { return lineRenderer; } }

    public FMODUnity.StudioEventEmitter SuppressionEmitter { get { return suppressionEmitter; } }

    PlayerController player;

    private void Start()
    {
        if (suppressionEmitter != null)
        {
            suppressionEmitter.Event = suppressionSound;
            suppressionEmitter.Play();
            suppressionEmitter.EventInstance.setParameterByName("Suppression", 0f);
        }

        player = FindObjectOfType<PlayerController>();
        uiController = FindObjectOfType<UIController>();

        if (chargeUpProjectile != null)
        {
            projectileStartScale = chargeUpProjectile.transform.localScale;
            chargeUpProjectile.transform.localScale = Vector3.zero;
        }
    }

    private void Update()
    {
        ShotTimerUpdate();
        ShutdownUpdate();
        // Update line renderer life time.
        UpdateLineRenderer();
        UpdateLineTimer();

        UpdateRecoil();
        UpdateFireDelay();

        if (Input.GetKeyUp(KeyCode.Mouse0) && fireBehavior.SpecialFireType == SpecialFireType.Laser)
        {
            firePointAudio.Stop();
            StopMuzzleFlashes();
        }
    }

    private void LateUpdate()
    {
        didFireThisFrame = false;
    }

    /// <summary>
    /// Initiates the weapon.
    /// </summary>
    public void InitWeapon(Camera camera)
    {
        WeaponCamera = camera;
        fpsController = FindObjectOfType<FPSController>();
    }

    /// <summary>
    /// Fires the weapon.
    /// </summary>
    public void Fire()
    {
        if (reloading || shutdown || fpsController.IsSprinting)
        {
            return;
        }

        if (camShake == null)
        {
            camShake = FindObjectOfType<CameraShake>();
        }

        if (!FireBehavior.IsContinuosLaser && canShoot && currentMagAmmo >= 1)
        {
            if (weaponStats.chargeUp)
            {
                if (chargeTimer >= weaponStats.chargeUpTime && Input.GetKeyUp(KeyCode.Mouse0) || Input.GetKeyUp(KeyCode.Mouse0))
                {
                    chargeTimer = 0;

                    if (chargeValue < weaponStats.minimumChargeUp)
                    {
                        firePointAudio.Stop();
                        chargeValue = 0;
                        //TODO: Sound When Failed to fire.

                        FMODUnity.RuntimeManager.PlayOneShot(chargeUpFailedSound, transform.position);
                        chargeUpProjectile.transform.localScale = Vector3.zero;
                        ToggleChargeParticles(false);

                        Debug.Log("FAILURE ON CHARGEUP");
                        return;
                    }

                    //Proceed to Fire the charged bullet.
                    Debug.Log("FIRING!");
                    if (chargeUpProjectile != null)
                    {
                        chargeUpProjectile.transform.localScale = Vector3.zero;
                    }
                    ToggleChargeParticles(false);


                    firePointAudio.Stop();

                    firePointAudio.Event = fireBehavior.FireSound;
                    FMODUnity.RuntimeManager.PlayOneShot(fireBehavior.FireSound, firePoint.transform.position);

                }
                else if (Input.GetKey(KeyCode.Mouse0))
                {
                    //TODO: DO CHARGE STUFF HERE.
                    if (fireBehavior.ChargeSound != null && chargeTimer <= 0.1f)
                    {
                        if (firePointAudio.IsPlaying() == false)
                        {
                            Debug.Log("CHARCHING!");
                            firePointAudio.Event = fireBehavior.ChargeSound;

                            firePointAudio.Play();

                            firePointAudio.EventInstance.setParameterByName("Trigger", 1);
                            firePointAudio.EventInstance.release();

                            ToggleChargeParticles(true);
                        }
                    }

                    chargeTimer += Time.deltaTime;
                    chargeValue = Mathf.Min(chargeTimer / weaponStats.chargeUpTime, 1);

                    firePointAudio.EventInstance.setParameterByName("ChargeValue", ChargeValue);

                    if (FireBehavior as FireBehavior_Projectile != null)
                        chargeUpProjectile.transform.localScale = projectileStartScale * ChargeValue;

                    return;
                }
            }

            //Fire delay.
            if (fireDelayTimer < weaponStats.fireDelay)
            {
                fireDelay = true;
                return;
            }
            fireDelayTimer = 0;

            //Fire.
            didFireThisFrame = true;
            fireBehavior.Fire(this);
            ShotTimerReset();
            AddAmmo(-weaponStats.projectileAmount);
            chargeValue = 0;

            // Function call to FMod to play fireBehavior.FireAudio.
            if (!weaponStats.chargeUp)
            {
                FMODUnity.RuntimeManager.PlayOneShot(fireBehavior.FireSound, firePoint.transform.position);
            }

            // Effects.
            PlayMuzzleFlashes();
            ResetLineTimer();
            Recoil(true);
            uiController.ScreenBlur();
            IncreaseSuppression();

            //Experimental Risk. NEEDS TO EXECUTE LAST IN THIS FUNCTION.
            if (Random.Range(0f, 1f) <= WeaponStats.failureRate)
            {
                InitShutdown();
                return;
            }
        }
        else if (FireBehavior.IsContinuosLaser && currentMagAmmo > 0)
        {
            //Experimental Risk.
            if (Random.Range(0f, 1f) <= WeaponStats.failureRate && laserFailureTick >= 1)
            {
                laserFailureTick = 0;
                shutdown = true;
                InitShutdown();
            }
            else if (shutdown == false)
            {
                laserFailureTick += (weaponStats.roundsPerMinute / 60) * Time.deltaTime;
            }

            didFireThisFrame = true;
            fireBehavior.Fire(this);
            AddAmmo(-(weaponStats.roundsPerMinute / 60) * Time.deltaTime);

            // Effects.
            PlayMuzzleFlashes();
            ResetLineTimer();
            Recoil(false);


            if (!firePointAudio.IsPlaying())
            {
                firePointAudio.Play();
            }
            if (currentMagAmmo <= 0)
            {
                firePointAudio.Stop();
            }

        }

        //Test for fire to make detectable sound.
        if (player != null && fireBehavior != null)
        {
            DetectableSound.PlayDetectableSound(player.transform.position, fireBehavior.fireSoundRadius, fireBehavior.EnemyDetectionMask);
        }
        else
        {
            if (player == null)
            {
                Debug.Log("Player was null in weapon");
            }
            if (FireBehavior != null)
            {
                Debug.Log("Firebehaviour was null in weapon");
            }
        }

        if (currentMagAmmo <= 0)
        {
            StopMuzzleFlashes();

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                FMODUnity.RuntimeManager.PlayOneShot(fireBehavior.TriggerSound, transform.position);
            }
        }
    }

    /// <summary>
    /// Reloads the weapon.
    /// </summary>
    public void Reload()
    {
        if (reloading || storageAmmo <= 0 || currentMagAmmo >= weaponStats.magSize || shutdown)
        {
            return;
        }

        mainAnimator.SetFloat("ReloadSpeed", weaponStats.reloadSpeed);
        mainAnimator.SetTrigger("Reload");
        reloading = true;

        // Function call to FMod to play fireBehavior.ReloadSound.
        FMODUnity.RuntimeManager.PlayOneShot(fireBehavior.ReloadSound);

        float ammoToRefill = weaponStats.magSize - currentMagAmmo;
        if (storageAmmo >= ammoToRefill)
        {
            currentMagAmmo += ammoToRefill;
            storageAmmo -= ammoToRefill;
        }
        else
        {
            currentMagAmmo += storageAmmo;
            storageAmmo = 0;
        }

    }

    public void EndReload()
    {
        reloading = false;
    }

    private void InitShutdown()
    {
        if (shutdown)
        {
            return;
        }

        //Shutdown Effects!
        shutdown = true;
        shutdownTimer = 0;

        SetEmissionColor(Color.black);

        Debug.Log("SHUTDOWN");
        if (shutdownEffect != null)
        {
            FMODUnity.StudioEventEmitter shutdownEmitter = shutdownEffect.GetComponent<FMODUnity.StudioEventEmitter>();
            shutdownEmitter.Play();
            shutdownEmitter.EventInstance.setParameterByName("Shutdown", 1);

            ParticleSystem[] particleSystems = shutdownEffect.transform.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem p in particleSystems)
            {
                p.Play();
            }
        }
    }

    private void ShutdownUpdate()
    {
        if (shutdown)
        {
            shutdownTimer += Time.deltaTime;
            if (shutdownTimer >= WeaponStats.restartTime)
            {
                shutdownTimer = 0;
                shutdown = false;

                //End Shutdown Effects!
                SetEmissionColor(MainWeaponMod.EmissionColor);

                if (shutdownEffect != null)
                {
                    FMODUnity.StudioEventEmitter shutdownEmitter = shutdownEffect.GetComponent<FMODUnity.StudioEventEmitter>();
                    shutdownEmitter.EventInstance.setParameterByName("Shutdown", 0);

                    ParticleSystem[] particleSystems = shutdownEffect.transform.GetComponentsInChildren<ParticleSystem>();
                    foreach (ParticleSystem p in particleSystems)
                    {
                        p.Stop();
                    }
                }
            }
        }
    }

    private void ToggleChargeParticles(bool play)
    {
        //ParticleSystem[] systems = chargeUpEffect.GetComponentsInChildren<ParticleSystem>();
        if (currentChargeUps == null || currentChargeUps.Length == 0)
        {
            return;
        }

        foreach (ParticleSystem p in currentChargeUps)
        {
            if (play)
                p.Play();
            else
                p.Stop();
        }
    }

    private void UpdateFireDelay()
    {
        if (fireDelay)
        {
            if (fireDelayTimer < weaponStats.fireDelay)
            {
                fireDelayTimer += Time.deltaTime;
                return;
            }
            fireDelay = false;
            Fire();
        }
    }

    private void UpdateLineRenderer()
    {
        if (lineRenderer == null)
        {
            return;
        }

        lineRenderer.positionCount = 2;
        Vector3[] linePoints = new Vector3[] { firePoint.position, fireBehavior.LastHitPoint };
        lineRenderer.SetPositions(linePoints);
    }

    private void UpdateLineTimer()
    {
        if (lineRenderer == null)
        {
            return;
        }

        lineRendererTimer -= Time.deltaTime;
        if (lineRendererTimer < 0)
        {
            lineRenderer.enabled = false;
        }
    }

    private void ResetLineTimer()
    {
        if (lineRenderer == null)
        {
            return;
        }

        UpdateLineRenderer();

        if (FireBehavior.IsContinuosLaser)
            lineRendererTimer = lrLifeTime * 2;
        else
            lineRendererTimer = lrLifeTime;

        lineRenderer.enabled = true;
    }

    private void IncreaseSuppression()
    {
        currentSuppression += MainWeaponMod.SuppressionIncrement;
        suppressionEmitter.EventInstance.setParameterByName("Suppression", MainWeaponMod.Suppression.Evaluate(currentSuppression));
        if (currentSuppressionRoutine != null)
        {
            StopCoroutine(currentSuppressionRoutine);
        }
        currentSuppressionRoutine = StartCoroutine(DelaySuppressionDecrease());
    }

    private IEnumerator DelaySuppressionDecrease()
    {
        yield return new WaitForSeconds(MainWeaponMod.SuppressionRecoveryDelay);

        currentSuppression = 0;
        suppressionEmitter.EventInstance.setParameterByName("Suppression", 0);
    }

    private void Recoil(bool rotate = true)
    {
        currentRecoil += 1;
        currentRecoil = Mathf.Min(10, currentRecoil);

        if (rotate)
        {
            fpsController.CameraRecoil(fireBehavior.RecoilAmplitude, fireBehavior.RecoilSpeed, fireBehavior.RecoilPermanentAmplitude);
            //mainAnimator.transform.localRotation = Quaternion.Euler(new Vector3(-currentRecoil, 0, 0));
        }

        float XValue = mainAnimator.transform.localPosition.x;
        if (mainAnimator.transform.localPosition.x < FireBehavior.RecoilXReturnThreshold && mainAnimator.transform.localPosition.x > -FireBehavior.RecoilXReturnThreshold)
        {
            XValue = Random.Range(-FireBehavior.RecoilXAmplitude, FireBehavior.RecoilXAmplitude);
        }

        mainAnimator.transform.localPosition = new Vector3(XValue, 0, -0.05f);

        camShake.InitCameraShake(0.2f, fireBehavior.CameraShakeAmplitude, 30);

    }

    private void UpdateRecoil()
    {

        if (!didFireThisFrame)
        {
            currentRecoil -= Time.deltaTime * 15;
        }
        currentRecoil = Mathf.Max(0, currentRecoil);

        // Lerp.
        //mainAnimator.transform.localRotation = Quaternion.Lerp(mainAnimator.transform.localRotation, Quaternion.identity, Time.deltaTime * 7);
        mainAnimator.transform.localPosition = Vector3.Lerp(mainAnimator.transform.localPosition, Vector3.zero, Time.deltaTime * 5);
    }

    private void UpdateReloadTimer()
    {

    }

    /// <summary>
    /// Resets the shotTimer. Sets CanShoot to false.
    /// </summary>
    private void ShotTimerReset()
    {
        canShoot = false;
        shotTimer = 0;
    }

    private void ShotTimerUpdate()
    {
        shotTimer += Time.deltaTime;
        if (shotTimer > weaponStats.ShotCooldown)
        {
            canShoot = true;
        }
    }

    private void OnDrawGizmos()
    {
        if (fireBehavior)
        {
            fireBehavior.DrawGizmos();
        }
    }

    /// <summary>
    /// Add ammo to the weapon mag. Is capped between 0 and magSize.
    /// </summary>
    public void AddAmmo(float value)
    {
        currentMagAmmo += value;
        currentMagAmmo = Mathf.Clamp(currentMagAmmo, 0, weaponStats.magSize);
    }

    /// <summary>
    /// Add Storage ammo. Is capped between 0 and maxStorageAmmo.
    /// </summary>
    public void AddStorageAmmo(float value)
    {
        storageAmmo += value;
        storageAmmo = Mathf.Clamp(storageAmmo, 0, maxStorageAmmo);
    }

    public void AddMaxStorageAmmo(float value)
    {
        maxStorageAmmo = Mathf.Max(maxStorageAmmo + value, 1);
    }

    private void PlayMuzzleFlashes()
    {
        if (currentMuzzleFlashes == null)
        {
            return;
        }

        ParticleSystemRandomizer particleSystemRandomizer = muzzleFlashParent.GetComponent<ParticleSystemRandomizer>();
        if (particleSystemRandomizer != null)
        {
            particleSystemRandomizer.PlayParticleSystems();
            return;
        }

        foreach (ParticleSystem ps in currentMuzzleFlashes)
        {
            if (FireBehavior.IsContinuosLaser == false)
                ps.Play();
            else if (ps.isPlaying == false)
                ps.Play();
        }
    }

    private void StopMuzzleFlashes()
    {
        if (currentMuzzleFlashes == null)
        {
            return;
        }

        ParticleSystemRandomizer particleSystemRandomizer = muzzleFlashParent.GetComponent<ParticleSystemRandomizer>();
        if (particleSystemRandomizer != null)
        {
            particleSystemRandomizer.StopParticleSystems();
            return;
        }

        foreach (ParticleSystem ps in currentMuzzleFlashes)
        {
            ps.Stop();
        }
    }

    private void SetEmissionColor(Color color)
    {
        weaponRenderer.material.SetColor("_EmissionColor", color);
        foreach (Light l in emissionLights)
        {
            l.color = color;
        }

        foreach (Text t in ammoTexts)
        {
            t.color = color;
        }
        foreach (Image i in chargeUpImages)
        {
            i.color = color;
        }
    }

    public WeaponMod ApplyMod(WeaponMod weaponMod, bool firstMod = false)
    {
        if (weaponMod == null)
        {
            return null;
        }

        //Major ModPackage/Weapon-type.
        if (weaponMod.FireBehavior != null)
        {
            Debug.Log("MAJOR MOD APPLIED");
            //Remove old main mod stats.
            if (firstMod == false)
            {
                weaponStats.ApplyWeaponStats(mainWeaponMod.WeaponStats, true, true, true);
            }

            //Switch rig.
            SwitchWeaponRig(weaponMod.FireBehavior.WeaponType);

            //Apply new main mod stats and behavior.
            mainWeaponMod = weaponMod;
            fireBehavior = weaponMod.FireBehavior;
            weaponStats.ApplyWeaponStats(weaponMod.WeaponStats, false, false, true);

            //Replace the first UI element in the mods list with the new majormod.
            if (currentMods.Count > 0)
            {
                currentMods[0] = weaponMod;
            }
            else
            {
                currentMods.Insert(0, weaponMod);
            }

            // Switch fmod event.
            firePointAudio.Event = fireBehavior.FireSound;
            firePointAudio.Stop();

            // Spawn muzzleFlash.
            if (muzzleFlashParent != null)
            {
                Destroy(muzzleFlashParent);
            }

            if (weaponMod.FireBehavior.MuzzleFlash != null)
            {
                muzzleFlashParent = Instantiate(weaponMod.FireBehavior.MuzzleFlash, firePoint);
                muzzleFlashParent.transform.localPosition = Vector3.zero;
                currentMuzzleFlashes = muzzleFlashParent.GetComponentsInChildren<ParticleSystem>();
            }

            //Spawn ChargeUp Effect
            if (chargeUpParent != null)
            {
                Destroy(chargeUpParent);
            }

            if (weaponMod.FireBehavior.ChargeUp != null)
            {
                chargeUpParent = Instantiate(weaponMod.FireBehavior.ChargeUp, firePoint);
                currentChargeUps = chargeUpParent.GetComponentsInChildren<ParticleSystem>();
            }

            // Spawn LineRenderer.
            if (lineRenderer != null)
            {
                Destroy(lineRenderer.gameObject);
            }

            if (weaponMod.FireBehavior.LineRenderer != null)
            {
                lineRenderer = Instantiate(weaponMod.FireBehavior.LineRenderer, firePoint);
            }

            //Change emmision color of the weapon.
            SetEmissionColor(MainWeaponMod.EmissionColor);

            //Reloaded gun on game start.
            AddAmmo(MainWeaponMod.WeaponStats.magSize);
        }
        //Minor mod, stat increase etc...
        else
        {
            Debug.Log("MINOR MOD APPLIED");
            weaponStats.ApplyWeaponStats(weaponMod.WeaponStats);
            currentMods.Add(weaponMod);
        }

        return weaponMod;
    }

    public void SwitchWeaponRig(WeaponType weaponType)
    {
        for (int i = 0; i < weaponRigs.Length; i++)
        {
            if (i == (int)weaponType)
            {
                weaponRigs[i].SetActive(true);
                firePoint = firePoints[i];
                mainAnimator = weaponRigs[i].GetComponent<Animator>();
                FindObjectOfType<PlayerAnimatorController>().weaponAnimator = weaponRigs[i].GetComponent<Animator>();
                //uiController.storageAmmo = weaponRigs[i].transform.Find(uiController.storageAmmo.gameObject.name).GetComponent<Text>();
                //uiController.magInfoText = weaponRigs[i].transform.Find(uiController.magInfoText.name).GetComponent<Text>();
                //uiController.chargeUpImage = weaponRigs[i].transform.Find(uiController.chargeUpImage.name).GetComponent<Image>();
                //uiController.minChargeUpImage = weaponRigs[i].transform.Find(uiController.minChargeUpImage.name).GetComponent<Image>();
            }
            else
            {
                weaponRigs[i].SetActive(false);
            }
        }
        
        FindObjectOfType<PlayerAnimatorController>().SetWeapon(weaponType);
    }
}

