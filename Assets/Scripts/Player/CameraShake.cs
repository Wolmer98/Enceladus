using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraShake : MonoBehaviour
{
    [SerializeField] float amplitude = 0.5f;
    [SerializeField] float shakeDuration = 0.2f;
    [SerializeField] float speed = 30;

    Vector3 startPos;
    bool shakePeaked;
    bool shake;
    float shakeTimer;
    float lerpValue;

    float _duration;
    float _amplitude;
    float _speed;
    Vector3 _shakeDist;

    private void Start()
    {
        startPos = transform.localPosition;

    }

    void SetBaseValues()
    {
        _duration = shakeDuration;
        _speed = speed;
        _amplitude = amplitude;
    }

    void Update()
    {
        if (shake)
        {
            if (!shakePeaked)
            {
                lerpValue += Time.deltaTime * _speed;

                if (lerpValue >= 1)
                {
                    NewShakeDist();
                    shakePeaked = true;
                    lerpValue = 1;
                }
            }
            else
            {
                lerpValue -= Time.deltaTime * _speed;

                if (lerpValue <= 0)
                {
                    NewShakeDist();
                    shakePeaked = false;
                    lerpValue = 0;
                }
            }
            //Lerp to shakeDistance.
            transform.localPosition = Vector3.Lerp(startPos, _shakeDist, lerpValue);

            //ShakeTimer.
            shakeTimer += Time.deltaTime;

            if (shakeTimer >= _duration)
            {
                shake = false;
                shakePeaked = false;
                shakeTimer = 0;
            }
        }
        else
        {
            //Lerp to startPos.
            transform.localPosition = startPos;
            lerpValue = 0;
            SetBaseValues();
        }
    }

    /// <summary>
    /// Initiates a camera shake for a duration based on a certain amplitude and speed.
    /// </summary>
    /// <param name="duration">The duration of the whole shake.</param>
    /// <param name="amplitude">The amplitude of the distance of the shake.</param>
    /// <param name="speed">The speed, going forth and back through the shake.</param>
    public void InitCameraShake(float duration = 0, float amplitude = 0, float speed = 0)
    {
        if (shake)
        {
            return;
        }

        if (duration != 0 && amplitude != 0 && speed != 0)
        {
            _duration = duration;
            _amplitude = amplitude;
            _speed = speed;
        }
        else
        {
            SetBaseValues();
        }

        NewShakeDist();

        shakeTimer = 0;
        shakePeaked = false;
        shake = true;
    }

    void NewShakeDist()
    {
        float shakeRemainder = 1 - shakeTimer / _duration;
        _shakeDist = startPos + Random.insideUnitSphere * _amplitude * shakeRemainder;
    }
}
