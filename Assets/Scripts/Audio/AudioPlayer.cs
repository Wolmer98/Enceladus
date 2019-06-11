using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public FPSController fPSController;

    [FMODUnity.EventRef]
    public string playerBreathinEvent;
    FMOD.Studio.EventInstance playerBreathin;

    float timer;
    public float defaultValue = 0.6f;
    float walkInterval;
    int stepStroke = 3;
    bool playerSprint = false;
    bool playerRecovering;
    float stam;
    float lastStam;
    bool breathBool = false;

    void Start()
    {
        playerBreathin = FMODUnity.RuntimeManager.CreateInstance(playerBreathinEvent);
        playerBreathin.start();

        //StartCoroutine(BreathDelay());
    }

    IEnumerator BreathDelay()
    {
        breathBool = true;
        yield return new WaitForSeconds(1);
        breathBool = false;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            PlayerWalk();
        }
    }

    void PlayerWalk()
    {
        timer += Time.deltaTime;

        if (timer >= walkInterval)
        {

            timer = 0;
            stepStroke++;
        }
    }
}