using System;
using UnityEngine;
using UnityStandardAssets.Utility;
using UnityStandardAssets.Characters.FirstPerson;

public class HeadBob : MonoBehaviour
{
    public Camera Camera;
    public CurveControlledBob motionBob = new CurveControlledBob();
    public LerpControlledBob jumpAndLandingBob = new LerpControlledBob();
    public FPSController fpsController;
    public float StrideInterval;
    [Range(0f, 1f)] public float RunningStrideLengthen;

    [Header("Sound Settings")]
    [FMODUnity.EventRef]
    [SerializeField] string sprintSound;
    FMOD.Studio.EventInstance sprintSoundEvent;

    [FMODUnity.EventRef]
    [SerializeField] string walkSound;
    FMOD.Studio.EventInstance walkSoundEvent;

    [FMODUnity.EventRef]
    [SerializeField] string crouchSound;
    FMOD.Studio.EventInstance crouchSoundEvent;

    // private CameraRefocus m_CameraRefocus;
    private bool m_PreviouslyGrounded;
    private Vector3 m_OriginalCameraPosition;


    private void Start()
    {
        motionBob.Setup(Camera, StrideInterval);
        m_OriginalCameraPosition = Camera.transform.localPosition;
    //     m_CameraRefocus = new CameraRefocus(Camera, transform.root.transform, Camera.transform.localPosition);
    }


    private void Update()
    {
        //  m_CameraRefocus.GetFocusPoint();
        Vector3 newCameraPosition;
        if (fpsController.Velocity.magnitude > 0 && fpsController.IsGrounded)
        {
            Camera.transform.localPosition = motionBob.DoHeadBob(fpsController.Velocity.magnitude*(fpsController.IsSprinting ? RunningStrideLengthen : 1f));
            newCameraPosition = Camera.transform.localPosition;
            newCameraPosition.y = Camera.transform.localPosition.y - jumpAndLandingBob.Offset();
        }
        else
        {
            newCameraPosition = Camera.transform.localPosition;
            newCameraPosition.y = m_OriginalCameraPosition.y - jumpAndLandingBob.Offset();
        }
        Camera.transform.localPosition = newCameraPosition;

        if (!m_PreviouslyGrounded && fpsController.IsGrounded)
        {
            StartCoroutine(jumpAndLandingBob.DoBobCycle());
        }

        m_PreviouslyGrounded = fpsController.IsGrounded;
        //  m_CameraRefocus.SetFocusPoint();
    }
}
