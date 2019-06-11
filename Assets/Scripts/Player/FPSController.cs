using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class FPSController : MonoBehaviour
{
    [HideInInspector] public static float SPEEDMULT = 20;


    [Header("General Settings")]
    [SerializeField]
    float SpeedMultiplier = 300;
    [SerializeField]
    [Range(0, 1)]
    float ForwardSpeed = 1;
    [SerializeField]
    [Range(0, 1)]
    float BackwardSpeed = 0.5f;
    [SerializeField]
    [Range(0, 1)]
    float SidestepSpeed = 1;

    [Header("Sprint Settings")]
    [SerializeField]
    KeyCode SprintKey = KeyCode.LeftShift;
    [SerializeField]
    float SprintSpeedMultiplier = 2;
    [Space]
    [SerializeField]
    float maxStamina = 100;
    [SerializeField]
    [Tooltip("This is how much stamina is DRAINED while sprinting, per second.")]
    float StaminaDrain = 25;
    [SerializeField]
    [Tooltip("This is how much stamina is GAINED while not sprinting, per second.")]
    float StaminaGain = 50;
    [SerializeField]
    [Tooltip("This is the percentage point (of the currentStamina based on the maxStamina) at which the sprinting speed will start to decline.")]
    [Range(0, 1)]
    float staminaDeclinePercentage = 0.3f;


    [Space]
    [SerializeField]
    [Tooltip("A higher value will increase the rate at which the player accelerate.")]
    float AccelerationRate = 1;
    public float Gravity = 9.81f;

    [Header("Mouse Settings")]
    [SerializeField]
    float MouseSensitivity = 2;
    [SerializeField]
    [Range(0f, 0.5f)]
    float MouseSmoothing = 0.1f;

    [Header("Crouch Settings")]
    public KeyCode CrouchKey = KeyCode.LeftControl;  //Not a default fps controller variable
    public float CrouchSpeedDivider = 2.0f;     //Not a default fps controller variable

    [Header("Camera Settings")]
    [SerializeField]
    Camera camera;
    [SerializeField]
    float cameraViewingCapUp = -85, cameraViewingCapDown = 60;

    [Header("Jump Settings")]
    [SerializeField]
    float jumpForce = 100;
    [SerializeField]
    float jumpReach = 2;
    [SerializeField]
    LayerMask jumpLayerMask;

    [Header("Sound Detection")]
    [SerializeField]
    float loudnessSprint = 10f;
    [SerializeField]
    float loudnessWalk = 5f;
    [SerializeField]
    float loudnessCrouch = 2f;
    [SerializeField]
    LayerMask enemyDetectionMask;

    [Header("Sound Settings")]
    [FMODUnity.EventRef]
    [SerializeField] string jumpSound;
    FMOD.Studio.EventInstance jumpSoundEvent;

    [FMODUnity.EventRef]
    [SerializeField] string landSound;
    FMOD.Studio.EventInstance landSoundEvent;

    float camXRot;

    Vector3 lastDir;
    Vector3 lastMousePos;
    Vector3 tempDir;

    float currentAcceleration;
    CharacterController characterController;

    float startSpeedMultiplier;

    bool crouching;

    float capsuleStartHeight;

    float currentStamina;
    bool canSprint = true;

    Weapon weapon;

    float recoilTimer = 0;
    float recoilSpeed;
    float recoilAmplitude;

    public Vector3 Velocity
    {
        get
        {
            return characterController.velocity;
        }
    }
    public bool IsGrounded { get; private set; }
    public bool IsSprinting { get; private set; }
    public bool IsFrozen { get; set; }

    public float Stamina { get { return currentStamina; } }
    public float MaxStamina { get { return maxStamina; } }
    public float Speed { get { return (SpeedMultiplier - 3) * SPEEDMULT; } }

    public float SprintMultiplier { get { return SprintSpeedMultiplier; } }

    public CharacterController CharacterController { get { return characterController; } }

    PlayerController playerController;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        capsuleStartHeight = characterController.height;

        currentStamina = maxStamina;

        startSpeedMultiplier = SpeedMultiplier;

        weapon = FindObjectOfType<Weapon>();
        playerController = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        if (!IsFrozen)
        {
            RecoilUpdate();
            CrouchCheck();
            MoveInputCheck();
            GroundCheck();
            MouseInputCheck();
            Move(tempDir);
        }
    }

    void FixedUpdate()
    {
        //ApplyGravity();
        //Move(tempDir);
    }

    private void GroundCheck()
    {
        if (RayCastGround(transform.forward) || RayCastGround(-transform.forward) || RayCastGround(transform.right) || RayCastGround(-transform.right))
        {
            if (IsGrounded == false)
            {
                FMODUnity.RuntimeManager.PlayOneShot(landSound, transform.position);
            }

            IsGrounded = true;
        }
        else
        {
            IsGrounded = false;
        }
    }

    private bool RayCastGround(Vector3 dirShift)
    {
        return Physics.Raycast(transform.position - dirShift * characterController.skinWidth, Vector3.down, jumpReach, jumpLayerMask);
    }


    void MoveInputCheck()
    {
        tempDir = new Vector3();

        if (playerController == null || playerController.InputData == null)
        {
            return;
        }

        if (Input.GetKey(playerController.InputData.GetKeyCode("Up")))
            tempDir.z = 1 * ForwardSpeed;
        if (Input.GetKey(playerController.InputData.GetKeyCode("Down")))
            tempDir.z = -1 * BackwardSpeed;
        if (Input.GetKey(playerController.InputData.GetKeyCode("Left")))
            tempDir.x = -1 * SidestepSpeed;
        if (Input.GetKey(playerController.InputData.GetKeyCode("Right")))
            tempDir.x = 1 * SidestepSpeed;

        if (currentStamina >= maxStamina)
            canSprint = true;
        if (currentStamina <= 0)
            canSprint = false;

        if (Input.GetKey(SprintKey) && tempDir.z > 0 && canSprint)
            IsSprinting = true;
        else
            IsSprinting = false;



        if (IsGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }
    }

    void CrouchCheck()
    {
        if (Input.GetKey(CrouchKey)) //Not a default fps controller function
        {
            characterController.height = 1.0f;
            crouching = true;
        }
        else
        {
            characterController.height = capsuleStartHeight;
            crouching = false;
        }
    }

    private void Jump()
    {
        FMODUnity.RuntimeManager.PlayOneShot(jumpSound, transform.position);
        Move(transform.up * jumpForce);
        //rigidbody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void MouseInputCheck()
    {
        Vector3 mouseDelta;
        mouseDelta.x = Input.GetAxis("Mouse X");
        mouseDelta.y = Input.GetAxis("Mouse Y");

        Rotate(new Vector2(mouseDelta.x, mouseDelta.y));
    }

    void Rotate(Vector2 direction)
    {
        direction = Vector2.Lerp(lastDir, direction, Time.deltaTime * (1 / MouseSmoothing));

        lastDir = direction;
        direction *= MouseSensitivity * Time.deltaTime;
        transform.Rotate(0, direction.x, 0);

        camXRot -= direction.y;
        camXRot = Mathf.Clamp(camXRot, cameraViewingCapDown, cameraViewingCapUp);

        camera.transform.localRotation = Quaternion.Euler(camXRot - (recoilAmplitude * recoilTimer), camera.transform.localRotation.y, camera.transform.localRotation.z);
    }

    public void CameraRecoil(float amplitude, float speed, float recoilPermanentAmplitude)
    {
        camXRot -= recoilPermanentAmplitude;
        recoilAmplitude = amplitude;
        recoilTimer = 1;
        recoilSpeed = speed;
    }

    private void RecoilUpdate()
    {
        if (recoilTimer > 0)
            recoilTimer -= Time.deltaTime * recoilSpeed;
        else
        {
            recoilTimer = 0;
        }
    }

    void Move(Vector3 direction)
    {
        //direction.Normalize(); //Resolves Strafing speed issue.
        direction = Vector3.ClampMagnitude(direction, 1);

        //If argument is true we decelerate the player.
        if (direction != Vector3.zero)
            Accelerate(false);
        else
            Accelerate(true);

        if (IsSprinting)
        {
            direction.z = direction.z * Mathf.SmoothStep(1, SprintSpeedMultiplier, currentStamina / maxStamina - staminaDeclinePercentage);
            currentStamina -= StaminaDrain * Time.deltaTime;
            currentStamina = Mathf.Max(0, currentStamina);
        }
        else
        {
            currentStamina += StaminaGain * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
        }

        direction = transform.TransformDirection(direction);

        Vector3 velocity = (direction * SpeedMultiplier * currentAcceleration * Time.deltaTime);

        if (crouching)
            velocity /= CrouchSpeedDivider;

        //rigidbody.velocity = new Vector3(velocity.x, rigidbody.velocity.y, velocity.z);
        //rigidbody.MovePosition(rigidbody.position + velocity);

        GroundCheck();
        float yForce1 = IsGrounded ? 1 : 0;
        float yForce2 = Input.GetKeyDown(KeyCode.Space) ? 1 : 0;
        float yForce = (yForce1 * yForce2 * jumpForce + characterController.velocity.y - Gravity) * Time.deltaTime;

        characterController.Move(new Vector3(velocity.x, yForce, velocity.z));

        if (crouching && characterController.velocity.magnitude > 0.1 && IsGrounded)
        {
            // Crouching
            DetectableSound.PlayDetectableSound(transform.position, loudnessCrouch, enemyDetectionMask);
        }
        else if (IsSprinting && characterController.velocity.magnitude > 0.1 && IsGrounded)
        {
            // Sprinting
            DetectableSound.PlayDetectableSound(transform.position, loudnessSprint, enemyDetectionMask);
        }
        else if (characterController.velocity.magnitude > 0.1 && IsGrounded)
        {
            // Walking
            DetectableSound.PlayDetectableSound(transform.position, loudnessWalk, enemyDetectionMask);
        }
    }

    void Accelerate(bool decelerate = false)
    {
        if (!decelerate)
            currentAcceleration += AccelerationRate * Time.deltaTime;
        else
            currentAcceleration -= AccelerationRate * Time.deltaTime;


        currentAcceleration = Mathf.Clamp(currentAcceleration, 0, 1);
    }


    public void AddSpeed(float value)
    {
        //The specific value is SPEEDMULT (10/0.05). This is simply to convert the speed to another value.
        SpeedMultiplier += value / SPEEDMULT;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y, transform.position.z), new Vector3(transform.position.x, transform.position.y - jumpReach, transform.position.z));

        if (characterController != null)
        {
            if (crouching && characterController.velocity.magnitude > 0.1 && IsGrounded)
            {
                // Crouching
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, loudnessCrouch);
            }
            else if (IsSprinting && characterController.velocity.magnitude > 0.1 && IsGrounded)
            {
                // Sprinting
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, loudnessSprint);
            }
            else if (characterController.velocity.magnitude > 0.1 && IsGrounded)
            {
                // Walking
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, loudnessWalk);
            }
        }
    }
}
