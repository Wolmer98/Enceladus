using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public PlayerController playerController;

    [FMODUnity.EventRef]
    public string musicManagerEvent;
    FMOD.Studio.EventInstance musicManager;

    [FMODUnity.EventRef]
    public string battleMusicEvent;
    FMOD.Studio.EventInstance battleMusic;

    [FMODUnity.EventRef]
    public string mildStingerEvent;
    FMOD.Studio.EventInstance mildStinger;

    private int battleTrack = 1;
    public int numberOfTracksForBattle;
    private int waitForMusic = 0;

    //Use these two bools to link to health points or scared trigger.... or whatever........ :D
    public bool criticalState = false;
    public bool battleState = false;
    public int eventState = 0;
    bool deathMusic = false;


    [Header("Threat Settings")]
    public int minimumEnemiesForBattleMusic = 1;
    bool inCombatMusic;
    bool inWaterMusic;
    float threatTimer = 0;
    public float waterThreat;
    float enemyThreat;
    [SerializeField] LayerMask enemyLayer;
    bool inCombat;

    float timer;
    float musicProgressionTimer;
    [SerializeField] float progressionInSeconds = 120;
    float timeout;

    float coolDownTimer;
    float battleTimer;

    WaterManager wm;

    private int ambientTrack { get { return Random.Range(1, 4); } }

    private void Awake()
    {
        //Link between two scripts!
        playerController = FindObjectOfType<PlayerController>();
        wm = FindObjectOfType<WaterManager>();

        FindObjectOfType<WorldGenerator>().OnWorldStart.AddListener(ResetMusicProgression);
        FindObjectOfType<WorldGenerator>().OnWorldStart.AddListener(InitMusic);
    }

    private void InitMusic()
    {
        StopMusic();

        musicManager = FMODUnity.RuntimeManager.CreateInstance(musicManagerEvent);


        battleTrack = Random.Range(1, numberOfTracksForBattle + 1);
        Debug.Log("Music " + ambientTrack + " is now playing.");
        musicManager.setParameterByName("Music", ambientTrack);
        musicManager.start();
    }

    private void Update()
    {
        threatTimer += Time.deltaTime;
        if (threatTimer >= 1)
        {
            threatTimer = 0;
            ReadWaterLevelThreat();
            ReadEnemyThreat();
        }

        musicManager.setParameterByName("Event", musicProgressionTimer);
        musicProgressionTimer += Time.deltaTime / progressionInSeconds / 2;

        if (coolDownTimer > 0)
        {
            coolDownTimer -= Time.deltaTime;
        }

        if (coolDownTimer < 0)
        {
            coolDownTimer = 0;
        }

        if (battleTimer > 0)
        {
            battleTimer -= Time.deltaTime;
        }

        if (battleTimer < 0)
        {
            battleTimer = 0;
        }

        //Link between two scripts!
        if (playerController.audioDeath == true)
        {
            musicManager.setParameterByName("Music", 5f);
            BattleStateOff();
        }

        else
        {
            //musicManager.setParameterByName("Music", ambientTrack);
        }
    }

    private void BattleStateOn()
    {
        if (battleTimer == 0f)
        {
            if (inCombatMusic == true || waterThreat >= 1.6f)
            {
                return;
            }

            inCombatMusic = true;
            battleTimer = 10f;

            Stinger();

            Debug.Log("BATTLE MUSIC ON");
            musicManager.setParameterByName("Music", 4f);
        }
    }

    private void BattleStateOff()
    {
        if (inCombatMusic == false)
        {
            return;
        }

        if (battleTimer == 0)
        {
            StartCoroutine(StartGeneralMusic());
        }
    }

    IEnumerator StartGeneralMusic()
    {
        Debug.Log("BATTLE MUSIC OFF");
        inCombatMusic = false;

        waitForMusic = Random.Range(1, 14);
        Debug.Log("Waiting " + waitForMusic + " seconds for music.");
        yield return new WaitForSeconds(waitForMusic);

        Debug.Log("GENERAL MUSIC APPLIED");
        musicManager.setParameterByName("Music", ambientTrack);

        Debug.Log("Ambient Music is back. Playing track: " + ambientTrack);
    }

    public void StopMusic()
    {
        musicManager.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        musicManager.release();
    }

    private void ResetMusicProgression()
    {
        musicProgressionTimer = 0;
    }

    public void Stinger()
    {
        if (coolDownTimer == 0)
        {
            CameraShake cameraShake = playerController.MainCamera.GetComponent<CameraShake>();
            if (cameraShake)
            {
                cameraShake.InitCameraShake();
            }
            FMODUnity.RuntimeManager.PlayOneShot(mildStingerEvent);
            coolDownTimer = Random.Range(20, 40);
        }

        else
        {
            return;
        }
    }

    public void WaterMusicOn()
    {
        inWaterMusic = true;

        musicManager.setParameterByName("Music", 6f);
        battleMusic.setParameterByName("Music", 0f);
        battleMusic.release();
    }

    public void WaterMusicOff()
    {
        if (inWaterMusic)
        {
            StartCoroutine(StartGeneralMusic());
            inWaterMusic = false;
        }
    }

    private void ReadEnemyThreat()
    {
        int enemyCounter = 0;
        inCombat = false;
        Collider[] colliders = Physics.OverlapSphere(playerController.transform.position, 3, enemyLayer);
        foreach (Collider c in colliders)
        {
            if (c.CompareTag("Enemy") && c.GetComponentInChildren<Destructible>().IsAlive)
            {
                inCombat = true;
                enemyCounter++;
            }
        }

        enemyThreat = enemyCounter / minimumEnemiesForBattleMusic;

        if (enemyThreat >= 1 && inCombat)
        {
            BattleStateOn();
        }
        else
        {
            BattleStateOff();
        }
    }

    private void ReadWaterLevelThreat()
    {
        if (playerController == null || playerController.FPSController == null || playerController.FPSController.CharacterController == null)
        {
            return;
        }

        float waterThreat = 0;

        float playerHeadPos = playerController.MainCamera.transform.position.y;
        float playerFeetPos = playerController.transform.position.y - playerController.FPSController.CharacterController.height / 2 - playerController.FPSController.CharacterController.skinWidth - 0.5f;
        float waterPos = wm.transform.position.y;

        float height = (waterPos - playerFeetPos) / playerHeadPos;
        float heightPrct = Mathf.SmoothStep(0, 2, height);

        waterThreat = heightPrct;
        musicManager.setParameterByName("Scared", waterThreat);
    }
}