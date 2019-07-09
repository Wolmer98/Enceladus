using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
using NaughtyAttributes;
using UnityEngine.Rendering.PostProcessing;

public enum InteractionType { Pickup, Mod, MajorMod, Log }

[RequireComponent(typeof(Destructible))]
public class PlayerController : MonoBehaviour
{
    [Header("Character")]
    [SerializeField] CharacterData characterData;

    [Header("General Settings")]
    [SerializeField] bl_Input inputData;
    [SerializeField] Transform mainWeaponParent;
    [SerializeField] Transform meleeWeaponParent;
    [SerializeField] Pickup_Weapon weaponPickupPrefab;

    [Header("Interaction Settings")]
    [SerializeField] float reach = 5;
    [SerializeField] float baseThrowForce = 1;
    [SerializeField] LayerMask interactionMask;
    [SerializeField] LayerMask defaultLayer;

    [Space]
    [SerializeField] GameObject flashLight;

    [Header("Rig Settings")]
    [SerializeField] bool weaponSway;
    [ShowIf("weaponSway")] [SerializeField] Transform rig;
    [ShowIf("weaponSway")] [SerializeField] Transform rigRotatePivot;
    [ShowIf("weaponSway")] [SerializeField] Transform rigTarget;
    [ShowIf("weaponSway")] [SerializeField] Vector3 rigTargetOffset;
    [ShowIf("weaponSway")] [SerializeField] float rigLerpSpeed = 1000f;
    [ShowIf("weaponSway")] [SerializeField] float rigRotationLerpSpeed = 1000f;
    [ShowIf("weaponSway")] [SerializeField] Vector3 wiggleAmount = new Vector3(4, 0, 12f);
    [ShowIf("weaponSway")] [SerializeField] Vector3 rigWiggleLerpSpeed = new Vector3(4f, 0f, 6f);
    [ShowIf("weaponSway")] [SerializeField] Vector3 rigAngleLimit = new Vector3(5f, 0, 5f);

    Vector3 currentForwardVector;
    Vector3 lastForwardVector;

    [Header("Other")]
    CharacterController cc;

    bool initialized;

    public CharacterData CharacterData { get { return characterData; } }
    public bl_Input InputData { get { return inputData; } }
    public FPSController FPSController { get; private set; }

    public Camera MainCamera { get; private set; }
    public Weapon MainWeapon { get; private set; }
    bool mainWeaponActive = true;

    public Destructible Destructible { get; private set; }

    public MeleeWeapon MeleeWeaponPrefab { get; private set; }
    public MeleeWeapon MeleeWeapon { get; private set; }

    public LayerMask InteractionMask { get { return interactionMask; } }
    public float Reach { get { return reach; } }

    public bool IsFrozen { get; set; }
    [HideInInspector] public bool audioDeath = false;

    public List<CharacterScreenOption> SuitMods { get; private set; }
    public UnityEvent OnInteract { get; private set; }
    public string LastPickedUpName { get; private set; }
    public string LastPickedUpText { get; private set; }
    public bool LastInteractionWasMod { get; private set; }
    public InteractionType LastInteractionType { get; private set; }
    public int LastPickedUpLog { get; private set; }

    private void Awake()
    {
        OnInteract = new UnityEvent();
    }

    private void Start()
    {

    }

    public void InitPlayer()
    {
        if (initialized)
            return;

        cc = GetComponent<CharacterController>();
        Destructible = GetComponent<Destructible>();
        FPSController = GetComponent<FPSController>();


        SuitMods = new List<CharacterScreenOption>();
        MainWeapon = mainWeaponParent.GetComponentInChildren<Weapon>();

        if (GameStateHandler.characterData != null)
        {
            characterData = GameStateHandler.characterData;
        }

        ApplyCharacter();
        /**/


        MainCamera = Camera.main;
        Debug.Log("Player Camera: " + MainCamera.name);

        //Set motion blur.
        MotionBlur mbSettings;
        MainCamera.GetComponent<PostProcessVolume>().profile.TryGetSettings(out mbSettings);
        mbSettings.enabled.value = (PlayerPrefs.GetInt(AllOptionsKeyPro.MotionBlur) == 1) ? true : false;

        MainWeapon.InitWeapon(MainCamera);

        UpdateActiveWeapon();

        currentForwardVector = MainCamera.transform.forward;
        lastForwardVector = currentForwardVector;

        initialized = true;
    }

    private void ApplyCharacter()
    {
        FindObjectOfType<CharacterScreen>().SetCharacterNameText(characterData.CharacterName);
        FindObjectOfType<CharacterScreen>().SetCharacterPortraitImage(characterData.Portrait);

        MainWeapon.ApplyMod(characterData.StartWeaponMod, true);
        MainWeapon.AddMaxStorageAmmo(characterData.MaxStorageAmmo);
        MainWeapon.AddStorageAmmo(characterData.Ammunition);

        foreach (Pickup_Enhancer suitMod in characterData.SuitMods)
        {
            suitMod.PickUp(this, true);
        }

        Destructible.AddMaxHealth(characterData.MaxHealth);
        Destructible.Heal(characterData.Health);
        Destructible.AddRegeneration(characterData.Regeneration);
        Destructible.AddResistance(characterData.Resistance);

        FPSController.AddSpeed(characterData.Speed);

        FindObjectOfType<AudioPlayerMovement>().playerFootstepEvent = characterData.walkSound;
        FindObjectOfType<AudioPlayerMovement>().playerSprintEvent = characterData.sprintSound;
        Destructible.onHurtSound = characterData.hurtSound;
        Destructible.onDeathSound = characterData.deathSound;

        if (characterData.StartMeleeWeapon != null)
        {
            EquipWeapon(characterData.StartMeleeWeapon, characterData.StartMeleeWeapon.Durability);
        }
    }

    private void Update()
    {
        if (IsFrozen || !initialized)
        {
            return;
        }

        currentForwardVector = MainCamera.transform.forward;

        //// -- Lerp Rig -- //
        if (weaponSway)
        {
            if (rig != null && rigTarget != null)
            {
                rig.position = Vector3.Lerp(rig.position, rigTarget.position + rigTargetOffset, Time.deltaTime * rigLerpSpeed);
                //rig.position = rigTarget.position;

                float angleUp = Vector3.SignedAngle(currentForwardVector, lastForwardVector, Vector3.right);
                angleUp = Mathf.Clamp(angleUp, -rigAngleLimit.x, rigAngleLimit.x);
                float signUp = angleUp > 0 ? 1 : -1;
                float xRot = Mathf.Lerp(0, wiggleAmount.x * signUp, Mathf.Clamp01(Mathf.Abs(angleUp)));
                //Debug.Log("a:" + angleUp + " signUp: " + signUp + " xRot: " + xRot);

                float angle = Vector3.SignedAngle(rig.forward, rigTarget.forward, Vector3.up);
                angle = Mathf.Clamp(angle, -rigAngleLimit.z, rigAngleLimit.z);
                float sign = angle > 0 ? -1 : 1;
                //Debug.Log(angle + " " + sign);
                float zRot = Mathf.Lerp(0, wiggleAmount.z * sign, Mathf.Clamp01(Mathf.Abs(angle / 10)));

                rig.rotation = Quaternion.Lerp(Quaternion.Euler(rigTarget.rotation.eulerAngles.x, rig.rotation.eulerAngles.y, rig.rotation.eulerAngles.z), rigTarget.rotation, Time.deltaTime * rigRotationLerpSpeed);
                //rig.rotation = rigTarget.rotation;

                rigRotatePivot.localRotation = Quaternion.Lerp(rigRotatePivot.localRotation, Quaternion.Euler(xRot, rigRotatePivot.localRotation.eulerAngles.y, rigRotatePivot.localRotation.eulerAngles.z), Time.deltaTime * rigWiggleLerpSpeed.x);
                rigRotatePivot.rotation = Quaternion.Lerp(rigRotatePivot.rotation, Quaternion.Euler(rigRotatePivot.rotation.eulerAngles.x, rigRotatePivot.rotation.eulerAngles.y, zRot), Time.deltaTime * rigWiggleLerpSpeed.z);
            }

            //rig.position = rigTarget.position;
            //rig.rotation = rigTarget.rotation;
        }

        if (inputData == null)
        {
            return;
        }

        // -- Switch Weapon -- // 
        if (Input.GetKeyDown(inputData.GetKeyCode("Primary Select")) && mainWeaponActive == false && MeleeWeapon != null && MeleeWeapon.IsReturned)
        {
            mainWeaponActive = true;
            UpdateActiveWeapon();
        }
        else if (Input.GetKeyDown(inputData.GetKeyCode("Secondary Select")) && mainWeaponActive == true && MeleeWeapon != null)
        {
            mainWeaponActive = false;
            UpdateActiveWeapon();
        }

        // -- Weapon -- //
        if (MainWeapon != null && mainWeaponActive)
        {
            if ((MainWeapon.FireBehavior.FireMode == FireMode.Automatic && Input.GetKey(inputData.GetKeyCode("Fire"))) ||
                (MainWeapon.FireBehavior.FireMode == FireMode.SemiAutomatic && Input.GetKeyDown(inputData.GetKeyCode("Fire"))) ||
                 MainWeapon.WeaponStats.chargeUp && Input.GetKey(inputData.GetKeyCode("Fire")) ||
                 MainWeapon.WeaponStats.chargeUp && Input.GetKeyUp(inputData.GetKeyCode("Fire")))
            {
                MainWeapon.Fire();
            }
        }
        else if (MeleeWeapon != null && !mainWeaponActive)
        {
            if (Input.GetKeyDown(inputData.GetKeyCode("Fire")))
            {
                MeleeWeapon.Attack();
            }
        }

        if (MainWeapon.FireBehavior.WeaponType == WeaponType.Laser)
        {
            if (Input.GetKeyDown(inputData.GetKeyCode("Fire")) && mainWeaponActive)
            {
                GetComponent<PlayerAnimatorController>().SetFire(true);
            }
            else if (Input.GetKeyUp(inputData.GetKeyCode("Fire")) && mainWeaponActive)
            {
                GetComponent<PlayerAnimatorController>().SetFire(false);
            }
        }

        if (Input.GetKeyDown(inputData.GetKeyCode("Reload")) && mainWeaponActive)
        {
            MainWeapon.Reload();
        }

        // -- Interaction -- //
        if (Input.GetKeyDown(inputData.GetKeyCode("Use")))
        {
            InteractionCheck();
        }

        // -- Throw/Drop -- //
        if (Input.GetKeyDown(inputData.GetKeyCode("Throw")) && !mainWeaponActive)
        {
            ThrowWeapon(baseThrowForce);
        }

        lastForwardVector = currentForwardVector;
    }

    /// <summary>
    /// Instantiates the given weapon and sets it to the player's current weapon.
    /// </summary>
    public void EquipWeapon(MeleeWeapon meleeWeapon, int durability)
    {
        if (MeleeWeapon != null)
        {
            // TODO: Add drop functionality.
            Destroy(MeleeWeapon.gameObject);
        }

        if (meleeWeapon)
        {
            MeleeWeaponPrefab = meleeWeapon;
            MeleeWeapon = Instantiate(MeleeWeaponPrefab, meleeWeaponParent);
            MeleeWeapon.Durability = durability;
            MeleeWeapon.OnDestroy.AddListener(delegate { FocusMainWeapon(); });
            MeleeWeapon.SetWeaponLayers(LayerMask.NameToLayer("WeaponColliders"));
        }
        else
        {
            MeleeWeaponPrefab = null;
            MeleeWeapon = null;
        }
    }

    private void InteractionCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(MainCamera.transform.position, MainCamera.transform.forward, out hit, reach, interactionMask))
        {
            //Debug.Log("RaycastHit");
            LastInteractionWasMod = false;
            InstallWeaponMod iwm = hit.transform.GetComponent<InstallWeaponMod>();
            bool interactNotification = false;

            if (hit.transform.GetComponent<Pickup>() || hit.transform.GetComponent<Pickup_Enhancer>())
            {
                Pickup_Log log = hit.transform.GetComponent<Pickup_Log>();
                if (log != null)
                {
                    LastInteractionType = InteractionType.Log;
                    LastPickedUpLog = log.LogID;
                }
                else
                {
                    LastInteractionType = InteractionType.Pickup;
                }

                bool pickedUpItem = PickupItem();
                if (pickedUpItem == false)
                {
                    if (hit.transform.GetComponent<Pickup_Enhancer>()?.ammo > 0)
                        FindObjectOfType<UIController>()?.ShowNotification("Ammo Storage Is Full", 1);
                    else
                        FindObjectOfType<UIController>()?.ShowNotification("Suit Mod Already Equipped", 1);

                    return;
                }

                interactNotification = true;
            }

            if (iwm != null)
            {
                LastInteractionWasMod = true;
                WeaponMod installedMod = MainWeapon.ApplyMod(iwm.GetWeaponModAndInstall());
                if (installedMod.FireBehavior != null)
                {
                    LastInteractionType = InteractionType.MajorMod;
                }
                else
                {
                    LastInteractionType = InteractionType.Mod;
                }

                LastPickedUpName = installedMod.WeaponModName;
                LastPickedUpText = installedMod.WeaponModName;

                // Destroy component to prevent multiple clicks.
                Destroy(iwm);
                interactNotification = true;
            }
            else if (hit.transform.tag == "Button")
            {
                Debug.Log("Button hit");
                hit.transform.GetComponentInParent<WorldButton>().Press();
                interactNotification = false;
            }
            else if (hit.transform.tag == "Door")
            {
                hit.transform.GetComponentInParent<Door>().Toggle();
                interactNotification = false;
            }
            else if (hit.transform.tag == "Vent")
            {
                if (!hit.transform.GetComponentInParent<Vent>().Used)
                {
                    hit.transform.GetComponentInParent<Vent>().Use();
                }
                interactNotification = false;
            }

            if (interactNotification)
            {
                GetComponent<PlayerAnimatorController>().PlayGrabAnimation();
                OnInteract.Invoke();
            }
        }
    }

    private bool PickupItem()
    {
        RaycastHit hit;
        if (Physics.Raycast(MainCamera.transform.position, MainCamera.transform.forward, out hit, reach, interactionMask))
        {
            Pickup pickup = hit.transform.GetComponentInParent<Pickup>();
            if (pickup != null)
            {
                bool result = pickup.PickUp(this);
                LastPickedUpName = pickup.PickupName;
                LastPickedUpText = pickup.PickupText;
                return result;
            }
        }
        return false;
    }

    public void AddSuitMod(CharacterScreenOption so)
    {
        SuitMods.Add(so);
    }

    public void EnableFlashLight()
    {
        flashLight.gameObject.SetActive(true);
    }

    private void ThrowWeapon(float baseThrowForce)
    {
        if (MeleeWeapon == null)
        {
            return;
        }

        GetComponent<PlayerAnimatorController>().ToggleMeleeWeapon(0, MeleeWeapon.WeaponType);

        int durability = MeleeWeapon.Durability;
        MeleeWeapon weaponToThrow = MeleeWeaponPrefab;
        EquipWeapon(null, 0);

        Pickup_Weapon spawnedPickup = Instantiate(weaponPickupPrefab, meleeWeaponParent);
        spawnedPickup.transform.SetParent(null);
        spawnedPickup.SetInitWeapon(weaponToThrow, durability);
        spawnedPickup.RB.AddForce((MainCamera.transform.forward * baseThrowForce * weaponToThrow.ThrowForce) + cc.velocity, ForceMode.Impulse);

        spawnedPickup.RB.AddTorque(transform.right * Random.Range(300, 500) + transform.up * Random.Range(-500, 500), ForceMode.Impulse);

        spawnedPickup.SetWeaponLayer(LayerMask.NameToLayer("Default"));
        spawnedPickup.SpawnedMeleeWeapon.Throw();

        // Switch to main weapon
        FocusMainWeapon();
    }

    /// <summary>
    /// End Attack event. Is called when attack is almost finished. Sets IsDamageActive to false.
    /// </summary>
    public void EndAttack()
    {
        MeleeWeapon mw = GetComponentInChildren<MeleeWeapon>();
        if (mw != null)
        {
            mw.SetAttackMode(false);
        }
    }

    public void FocusMainWeapon()
    {
        mainWeaponActive = true;
        UpdateActiveWeapon();
    }

    private void UpdateActiveWeapon()
    {
        if (mainWeaponActive)
        {
            mainWeaponParent.gameObject.SetActive(true);
            meleeWeaponParent.gameObject.SetActive(false);

            if (MeleeWeapon != null)
                GetComponent<PlayerAnimatorController>().ToggleMeleeWeapon(0, MeleeWeapon.WeaponType);
        }
        else
        {
            mainWeaponParent.gameObject.SetActive(false);
            meleeWeaponParent.gameObject.SetActive(true);

            if (MeleeWeapon != null)
                GetComponent<PlayerAnimatorController>().ToggleMeleeWeapon(1, MeleeWeapon.WeaponType);
        }
    }

    public void Die()
    {
        FPSController.IsFrozen = true;
        audioDeath = true;

        MainWeapon.AddStorageAmmo(-Mathf.Infinity);
        MainWeapon.AddAmmo(-MainWeapon.WeaponStats.magSize);
        FindObjectOfType<PlayerAnimatorController>().PlayDeathAnimation();
    }

    public void EnableDeathScreen()
    {
        FindObjectOfType<UIController>().EnableDeathScreen();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Respawns the player. Restarts the game.
    /// </summary>
    public void Respawn()
    {
        audioDeath = false;
        FindObjectOfType<WaterManager>().UnderWaterSource.EventInstance.setParameterByName("Underwater", 0);
        FindObjectOfType<WaterManager>().UnderWaterSource.Stop();
        MainWeapon.SuppressionEmitter.EventInstance.setParameterByName("Suppression", 0);
        MainWeapon.SuppressionEmitter.Stop();
        FindObjectOfType<MusicManager>().StopMusic();

        FindObjectOfType<WorldGenerator>().StopAllFMODEmitters();

        // Full health.
        // Score screen...?
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        GameStateHandler.LocalToGlobalLogsTransfer();
        audioDeath = false;
        FindObjectOfType<WaterManager>().UnderWaterSource.EventInstance.setParameterByName("Underwater", 0);
        FindObjectOfType<WaterManager>().UnderWaterSource.Stop();
        MainWeapon.SuppressionEmitter.EventInstance.setParameterByName("Suppression", 0);
        MainWeapon.SuppressionEmitter.Stop();
        FindObjectOfType<MusicManager>().StopMusic();

        FindObjectOfType<WorldGenerator>().StopAllFMODEmitters();

        SceneManager.LoadScene(0);
    }
}
