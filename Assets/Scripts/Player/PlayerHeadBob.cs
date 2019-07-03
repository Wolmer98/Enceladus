using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeadBob : MonoBehaviour
{
    [SerializeField] AnimationCurve YCurve;
    [SerializeField] AnimationCurve XCurve;
    [SerializeField] float BobSpeed = 1;
    [SerializeField] float BobMultiplier = 1;
    [SerializeField] float StaminaMultiplier = 0.5f;

    FPSController fpsController;
    Camera camera;
    Vector3 cameraStartPos;

    float bobValue = 0;
    bool init = false;

    void Start()
    {
        FindObjectOfType<WorldGenerator>().OnWorldStart.AddListener(delegate { Init(); });
    }

    void Init()
    {
        fpsController = FindObjectOfType<FPSController>();
        camera = Camera.main;
        cameraStartPos = camera.transform.localPosition;

        init = true;
    }

    void LateUpdate()
    {
        if (fpsController == null)
            fpsController = FindObjectOfType<FPSController>();

        if (init)
        {
            if (fpsController.Velocity.magnitude > 0.1f)
            {
                bobValue += Time.deltaTime * BobSpeed * fpsController.Velocity.magnitude;
                if (bobValue >= 1)
                {
                    bobValue = 0;
                }

                camera.transform.localPosition += new Vector3(XCurve.Evaluate(bobValue), YCurve.Evaluate(bobValue), 0) * BobMultiplier * Mathf.Max((fpsController.MaxStamina/Mathf.Max(fpsController.Stamina, 1) * StaminaMultiplier), 10);
            }
            else
            {
                bobValue = 0;
                camera.transform.localPosition = Vector3.Lerp(camera.transform.localPosition, cameraStartPos, Time.deltaTime);
            }
        }
    }
}
