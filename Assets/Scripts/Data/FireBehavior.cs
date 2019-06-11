using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum FireMode { SemiAutomatic, Automatic}
public enum SpecialFireType { None, Laser }

public abstract class FireBehavior : ScriptableObject
{
    protected const float spreadRadius = 1f;

    [SerializeField] protected FireMode fireMode;
    [SerializeField] protected SpecialFireType specialFireType;

    [Header("Sound Settings")]
    [FMODUnity.EventRef]
    [SerializeField] string fireSound;
    FMOD.Studio.EventInstance fireSoundEvent;

    [FMODUnity.EventRef]
    [SerializeField] string chargeSound;
    FMOD.Studio.EventInstance chargeSoundEvent;

    [FMODUnity.EventRef]
    [SerializeField] string reloadSound;
    FMOD.Studio.EventInstance reloadSoundsEvent;

    [FMODUnity.EventRef]
    [SerializeField] string triggerSound;
    FMOD.Studio.EventInstance triggerSoundEvent;

    [Header("Effects")]
    [SerializeField] protected GameObject muzzleFlash;
    [SerializeField] protected LineRenderer lineRenderer;
    [SerializeField] protected GameObject chargeUp;

    [Header("Camera")]
    [SerializeField] float recoilAmplitude = 2;
    [SerializeField] float recoilSpeed = 7;
    [SerializeField] float recoilPermanentAmplitude = 0.1f;
    [SerializeField] float cameraShakeAmplitude = 0.01f;
    [Space]
    [SerializeField] float recoilXAmplitude = 0.001f;
    [SerializeField] float recoilXReturnThreshold = 0.0001f;

    [Space]
    public UnityEvent OnShoot;

    public FireMode FireMode { get { return fireMode; } }
    public SpecialFireType SpecialFireType { get { return specialFireType; } }

    public string FireSound { get { return fireSound; } }
    public string ReloadSound { get { return reloadSound; } }
    public string ChargeSound { get { return chargeSound; } }
    public string TriggerSound { get { return triggerSound; } }
    public float fireSoundRadius = 18.0f;
    public LayerMask EnemyDetectionMask;

    public GameObject MuzzleFlash { get { return muzzleFlash; } }
    public GameObject ChargeUp { get { return chargeUp; } }
    public LineRenderer LineRenderer { get { return lineRenderer; } }

    public Vector3 LastHitPoint { get; protected set; }

    public bool IsContinuosLaser { get { return (fireMode == FireMode.Automatic && specialFireType == SpecialFireType.Laser); } }

    public float RecoilAmplitude { get { return recoilAmplitude; } }
    public float RecoilSpeed { get { return recoilSpeed; } }
    public float RecoilPermanentAmplitude { get { return recoilPermanentAmplitude; } }
    public float CameraShakeAmplitude { get { return cameraShakeAmplitude; } }
    public float RecoilXAmplitude { get { return recoilXAmplitude; } }
    public float RecoilXReturnThreshold { get { return recoilXReturnThreshold; } }


    public abstract void Fire(Weapon weapon);

    public abstract void DrawGizmos();
}
