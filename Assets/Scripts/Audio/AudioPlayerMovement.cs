using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayerMovement : MonoBehaviour
{
    public FPSController fPSController;

    [FMODUnity.EventRef]
    public string playerFootstepEvent;
    //FMOD.Studio.EventInstance playerFootstep;

    [FMODUnity.EventRef]
    public string playerSprintEvent;
    //FMOD.Studio.EventInstance playerBreathin;

    [SerializeField] FMODUnity.StudioEventEmitter walkSource;
    [SerializeField] FMODUnity.StudioEventEmitter sprintSource;

    WaterManager wm;

    float timer;
    public float defaultValue = 0.6f;
    float walkInterval;
    int stepStroke = 3;
    bool playerSprint = false;
    bool playerRecovering;
    float stam;
    float lastStam;
    bool breathBool = false;
    float breathinTimeout;

    void Start()
    {
        walkInterval = defaultValue;



        wm = FindObjectOfType<WaterManager>();
    }

    void Update()
    {
        if (walkSource.Event == "" || sprintSource.Event == "")
        {
            walkSource.Event = playerFootstepEvent;
            sprintSource.Event = playerSprintEvent;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            PlayerWalk();
        }

        if (fPSController.IsSprinting)
        {
            if (playerSprint == false)
            {
                stepStroke = 1;
            }

            //walkInterval = defaultValue / 2;
            playerSprint = true;
        }

        else
        {
            //walkInterval = defaultValue;
            playerSprint = false;
        }
        //Divide the deafultValue with the current speed divided by the maximum speed.
        walkInterval = defaultValue / (fPSController.Velocity.magnitude * ((fPSController.Speed + 3) / fPSController.SprintMultiplier));
        if (stepStroke == 5)
        {
            stepStroke = 1;
        }

        if (breathinTimeout > 0)
        {
            breathinTimeout -= Time.deltaTime;
        }

        if (breathinTimeout < 0)
        {
            breathinTimeout = 0;
        }
    }

    private void PlayerWalk()
    {
        timer += Time.deltaTime;

        if (walkSource == null || sprintSource == null || wm == null)
        {
            return;
        }

        if (timer >= walkInterval)
        {
            walkSource.Play();
            walkSource.EventInstance.release();
            timer = 0;
            stepStroke++;


            //stam = fPSController.Stamina / fPSController.MaxStamina;
            //sprintSource.EventInstance.setParameterByName("Stamina", stam);
        }
        walkSource.EventInstance.setParameterByName("Weight", 0.5f);

        if (wm.PlayerInWater)
        {
            walkSource.EventInstance.setParameterByName("TouchingWater", 1f);
        }
        else
        {
            walkSource.EventInstance.setParameterByName("TouchingWater", 0f);
        }



        if (sprintSource.IsPlaying() == false)
        {
            sprintSource.EventInstance.setParameterByName("Stamina", stam);
            sprintSource.Play();
            sprintSource.EventInstance.release();
        }

        if (breathinTimeout == 0)
        {
            sprintSource.EventInstance.setParameterByName("Step", stepStroke);
        }

        stam = fPSController.Stamina / fPSController.MaxStamina;
        sprintSource.EventInstance.setParameterByName("Stamina", stam);

        float deltaStam = lastStam - stam;
        lastStam = stam;

        if (stam < 0.1f || deltaStam < 0)
        {
            stam = 0;
            breathinTimeout = 4f;
        }


        //playerFootstep = FMODUnity.RuntimeManager.CreateInstance(playerFootstepEvent);
        //playerFootstep.setParameterByName("Weight", 0.5f);
        //playerFootstep.start();
        //playerFootstep.release();

        //Debug.Log("SPRINTINNININING");
        //playerBreathin = FMODUnity.RuntimeManager.CreateInstance(playerBreathinEvent);

        //stam = 1;
        //if (!playerRecovering)
        //{
        //    stam = fPSController.Stamina / fPSController.MaxStamina;
        //}

        //if (stam <= 0.1f)
        //{
        //    stam = 0;
        //}

        //if (stam <= 0)
        //{
        //    playerRecovering = true;
        //}
        //else if (stam >= 1)
        //{
        //    playerRecovering = false;
        //}


        //playerBreathin.setParameterByName("Stamina", stam);

        //if (playerSprint)
        //{
        //    playerBreathin.setParameterByName("Step", stepStroke);
        //}

        //playerBreathin.start();
        //playerFootstep.release();


        /*
        if (fPSController.Stamina/100 <= 0)
        {
            playerBreathin = FMODUnity.RuntimeManager.CreateInstance(playerBreathinEvent);
            playerBreathin.setParameterByName("Step", 0f);
            playerBreathin.start();
        }
        */
        //fPSController.Stamina/100
    }
}
