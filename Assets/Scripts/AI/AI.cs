using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum enemyType { swarm, medium, big, hiveMother, carrier, reaper }

[RequireComponent(typeof(NavMeshAgent))]
public class AI : MonoBehaviour
{
    public enemyType typeOfEnemy;
    [Header("AI Components")]
    [SerializeField] Animator AIStateMachine;
    [SerializeField] AI_State baseState;
    [SerializeField] AI_State detectedSoundState;
    [SerializeField] AI_State currentState;
    [SerializeField] AI_Transition[] onHurtConditions;

    [Header("GameObject Components")]
    [SerializeField] SphereCollider detectionSphere;
    [SerializeField] AI_Stats stats;
    [SerializeField] Animator animator;
    [SerializeField] GameObject underwaterEffects;
    [SerializeField] Transform topOfCrabForWaterLevel;
    [SerializeField] SkinnedMeshRenderer skinnedMeshRenderer;

    [Header("Jumping")]
    [SerializeField] LayerMask jumpLayerMask;
    [SerializeField] float rayCastRange = 1;

    [Header("DamageArea")]
    [SerializeField] Vector3 damagePosition;
    [SerializeField] float radiusOfDamageSphere = 0.6f;

    [Header("Sound Settings")]
    public FMODUnity.StudioEventEmitter soundEmitter;
    public FMODUnity.StudioEventEmitter walkingSoundEmitter;

    [Space]
    [FMODUnity.EventRef]
    public string spawnSound;

    [FMODUnity.EventRef]
    public string aggroSound;

    [FMODUnity.EventRef]
    public string echoSound;

    [FMODUnity.EventRef]
    public string attackSound;

    [FMODUnity.EventRef]
    public string hitMarkSound;

    //Water
    private WaterManager waterManager;

    private RaycastManager raycastManager;
    private Rigidbody rigidbody;
    private Room room;
    private bool playerBehindWallForSound;
    private bool playingSoundBehindWall;

    // State variables
    private float actionTimer;
    private float conditionTimer;
    private float echoTime;
    private Vector3 startPosition;
    private Vector3 startRotation;
    //private Color skinnedmeshRenderEmissionOriginalColor; Not used anymore.

    // Visuals
    public Animator Animator { get { return animator; } }

    // Agent
    private NavMeshAgent agent;
    private float baseStoppingDist;

    //Locations
    private PatrolWaypoint[] patrolWaypoints;

    // Tracking
    private Vector3 targetLastPosition;
    private Vector3 soundLastPosition;


    public bool Paused { get; set; }

    // AI Components
    public AI_Stats Stats { get { return stats; } }                             //Used by Animator
    public Rigidbody Rigidbody { get { return rigidbody; } }
    public enemyType EnemyType { get { return typeOfEnemy; } }
    public bool IsMoving { get; set; }
    public EnemySpawnManager EnemySpawnManager { get; private set; }

    // State getters
    public AI_State CurrentState { get { return currentState; } }
    public AI_Transition[] OnHurtConditions { get { return onHurtConditions; } }
    public Vector3 StartPosition { get { return startPosition; } }
    public Vector3 StartRotation { get { return startRotation; } }
    public Animator StateMachine { get { return AIStateMachine; } }

    // Visuals getters

    // Agent Getters
    public NavMeshAgent Agent { get
        {
            if (agent == null )
            {
                agent = GetComponent<NavMeshAgent>();
                Debug.Log("agent was set again in AI");
            }
            return agent;
        }
    } //Used by Animator
    public float BaseStoppingDist { get { return baseStoppingDist; } }

    // Patrol Getters
    public PatrolWaypoint[] PatrolWaypoints { get { return patrolWaypoints; } }
    public int PatrolWaypointIndex { get; set; }
    public Room Room { get { return room; } }

    // Tracking getters
    public Vector3 TargetLastPosition { get { return targetLastPosition; } }
    public Vector3 SoundLastPosition { get { return soundLastPosition; } }              //Used by animator
    public Transform Target { get; set; }

    // Detection
    public SphereCollider DetectionSphere { get { return detectionSphere; } }
    // Detection/Timer
    private bool isScreaming;
    public bool IsScreaming { get { return isScreaming; } set { echoTime = 0; isScreaming = value; } }
    private bool isEcholocating;
    public bool IsEchoLocating { get { return isEcholocating; } set { echoTime = 0; isEcholocating = value; } }
    public float AggroSoundCountdown { get; set; }

    public bool SetATimer { get; set; }                                                    //Used by Animator
    public float ActionTime { get { return actionTimer; } set { actionTimer = value; } } //Used by Animator

    public bool SetCTimer { get; set; }
    public float ConditionTime { get { return conditionTimer; } set { conditionTimer = value; } }

    // Search
    public int SearchPointIndex { get; set; }
    public Vector3[] SearchPoints { get; set; }
    public bool FoundSearchPoints { get; set; }

    // Combat
    public Destructible Destructible { get; private set; }
    public float AttackCooldown { get; set; }
    public bool IsAttacking { get; set; }
    public bool ChasingPlayer { get; set; }
    public bool FleeingFromPlayer { get; set; }
    public float DifficultyMod { get; private set; }
    public bool EnemyHurt { get; set; }
    public bool Enrage { get; set; }
    public float ChargeCooldown { get; set; }
    public Vector3 ChargeDirection { get; set; }
    public bool IsStunned { get; set; }
    public bool HasCharged { get; set; }
    public bool HealthThresholdReaction { get; set; }
    public bool BeenHealthStunned { get; set; }


    public Vector3 DamagePosition { get { return transform.position + (transform.forward * damagePosition.z) + (transform.up * damagePosition.y); } } //Used by anim
    public float DamageRadius { get { return radiusOfDamageSphere; } }

    //Backup
    public EnemySpawnPoint SpawnPointForBackup { get; set; }
    public bool CalledForFriends { get; set; }                          // Used by Anim

    // Awareness
    public List<AI> AlliesAround { get; set; }
    public int ID { get; private set; }
    public List<int> IDsRegisteredAt { get; set; }
    public bool FindNearbyAllies { get; set; }

    #region AnimatorBehaviourGetters
    private PlayerController pc;
    public PlayerController Player { get
        {
            if (pc == null)
            {
                pc = FindObjectOfType<PlayerController>();
            }
            return pc;
        }
        private set
        {
            pc = value;
        }
    }
    #endregion

    /// <summary>
    /// Called to initiate the AI with information relevant for it.
    /// </summary>
    public void InitAi(Room room, float difficultyMod, int Id, EnemySpawnManager enemySpawnManager)
    {
        if (room != null)
        {
            this.room = room;
            patrolWaypoints = room.PatrolWaypoints;
        }
        DifficultyMod = difficultyMod;

        ID = Id;
        EnemySpawnManager = enemySpawnManager;

        FindNearbyAllies = true;

        Destructible = GetComponent<Destructible>();
        Destructible.AddMaxHealth(Destructible.MaxHealth * difficultyMod - Destructible.MaxHealth);
        Destructible.Heal(Destructible.MaxHealth);

        Destructible.OnHurt.AddListener(delegate { FMODUnity.RuntimeManager.PlayOneShot(hitMarkSound); });

        FMODUnity.RuntimeManager.PlayOneShot(spawnSound, transform.position);
        if (!HasRunStart)
            Start();
    }

    private bool HasRunStart;
    private void Start()
    {
        //Testing
        if(room == null)
            patrolWaypoints = FindObjectsOfType<PatrolWaypoint>();

        // Get and find
        rigidbody = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        raycastManager = FindObjectOfType<RaycastManager>();
        waterManager = FindObjectOfType<WaterManager>();

        //Init
        startPosition = new Vector3(transform.position.x, 0, transform.position.z);
        IDsRegisteredAt = new List<int>();
        AlliesAround = new List<AI>();

        //Start values
        detectionSphere.radius = stats.detectionRadius;
        SearchPoints = new Vector3[stats.nbrSearchPos];
        AttackCooldown = stats.attackSpeed;
        ChargeCooldown = stats.chargeCooldown / 2;
        //skinnedmeshRenderEmissionOriginalColor = skinnedMeshRenderer.materials[0].GetColor("_EmissionColor"); Not used for now.
        skinnedMeshRenderer.materials[0].SetColor("_EmissionColor", Color.white);

        HasRunStart = true;
    }

    public void AnimUpdate()                //Used by animator
    {
        if (Paused)
        {
            return;
        }

        //if (currentState != null)
        //{
        //    currentState.UpdateState(this);
        //}

        AIMoving();

        if (waterManager != null)
        {
            //CheckIfUnderWater();
        }
    }

    private void AIMoving()                 //Used by animator
    {
        if (IsMoving)
        {
            float snapshot;
            playerBehindWallForSound = TestIfPlayerInSight(transform.position, 20.0f);
            walkingSoundEmitter.EventInstance.getParameterByName("Snapshot", out snapshot);

            if (!walkingSoundEmitter.IsPlaying())
            {
                walkingSoundEmitter.Play();
            }

            if (!playerBehindWallForSound)
            {
                if (snapshot != 1.0f)
                {
                    walkingSoundEmitter.EventInstance.setParameterByName("Snapshot", 1.0f);
                }
            }
            else
            {
                if (snapshot != 0.0f)
                {
                    walkingSoundEmitter.EventInstance.setParameterByName("Snapshot", 0.0f);
                }
            }
        }
        else
        {
            if (walkingSoundEmitter.IsPlaying())
            {
                walkingSoundEmitter.Stop();
            }
        }
    }

    private void CheckIfUnderWater()                                //Used by animator   
    {
        if (topOfCrabForWaterLevel.position.y < waterManager.transform.position.y)
        {
            if (!underwaterEffects.activeSelf)
            {
                skinnedMeshRenderer.materials[0].SetColor("_EmissionColor", Color.white);
                underwaterEffects.SetActive(true);
            }
        }
        else
        {
            if(underwaterEffects.activeSelf)
            {
                skinnedMeshRenderer.materials[0].SetColor("_EmissionColor", Color.black);
                underwaterEffects.SetActive(false);
            }
        }
    }

    private void PlayAggroSound()
    {
        if (AggroSoundCountdown <= 0)
        {
            FMODUnity.RuntimeManager.PlayOneShot(aggroSound, transform.position);
            AggroSoundCountdown = Stats.timeBetweenAggroSound;
        }
    }

    /// <summary>
    /// Used to inform the AI that a friend has died and need to find allies.
    /// </summary>
    public void FriendDied()
    {
        //Debug.Log(ID + " had to find allies in range");
        FindNearbyAllies = true;
    }

    /// <summary>
    /// Used by actions to check if a certain time has passed.
    /// </summary>
    public bool ActionTimeCheck(float duration)                         //Used by Animator
    {
        actionTimer += Time.deltaTime;
        return (actionTimer >= duration);
    }

    /// <summary>
    /// Used by states to see if a certain amount of time has passed.
    /// </summary>
    public bool ConditionTimeCheck(float duration)                    //Used by Animator
    {
        conditionTimer += Time.deltaTime;
        return(conditionTimer >= duration);
    }
    
    /// <summary>
    /// Check if scream has been going on for duration.
    /// </summary>
    public bool EchoTimer(float duration)
    {
        echoTime += Time.deltaTime;
        return(echoTime >= duration);
    }

    /// <summary>
    /// Check if AI can attack yet.
    /// </summary>
    public bool AttackTimer(float duration)                         //Used by animator
    {
        AttackCooldown += Time.deltaTime;
        return (AttackCooldown >= duration);
    }

    public bool ChargeTimer(float duration)
    {
        ChargeCooldown += Time.deltaTime;
        return (ChargeCooldown >= duration);
    }

    /// <summary>
    /// Used by sounds to tell the AI it has heard something.
    /// </summary>
    public void DetectedSound(Vector3 soundPosition, bool knowExactLocation)            //Used by animator
    {
        if(!gameObject.activeSelf)
        {
            return;
        }
        if (knowExactLocation)
        {
            soundLastPosition = soundPosition;
            agent.destination = soundLastPosition;
        }
        else
        {
            SetSoundLastPostionOnPath(soundPosition);
            if(gameObject.activeSelf && agent.isActiveAndEnabled)
            {
                agent.destination = soundLastPosition;
            }
        }

        //Debug.Log("AI heard sound");
        if(!StateMachine.GetCurrentAnimatorStateInfo(0).IsName("Chase") 
            || !StateMachine.GetCurrentAnimatorStateInfo(0).IsName("Charge") 
            || !StateMachine.GetCurrentAnimatorStateInfo(0).IsName("Stunned"))
        {
            if(!ChasingPlayer && !FleeingFromPlayer && !IsStunned)
            {
                StateMachine.Play("Chase");
                PlayAggroSound();
            }
        }
        else if (!StateMachine.GetCurrentAnimatorStateInfo(0).IsName("Search"))
        {
            conditionTimer = 0f;
        }
    }

    /// <summary>
    /// Subs to onDestroy, stops sounds on death.
    /// </summary>
    public void StopSounds()
    {
        walkingSoundEmitter.EventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        soundEmitter.EventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        walkingSoundEmitter.Stop();
        soundEmitter.Stop();
    }

    /// <summary>
    /// Used by some actions to check if grounded.
    /// </summary>
    public bool GroundCheck()
    {
        if (Physics.Raycast(transform.position, Vector3.down, rayCastRange, jumpLayerMask))
        {
           return true;
        }
        else
        {
            return false;
        }
    }


    /// <summary>
    /// Used by some conditions and actions.
    /// </summary>
    public bool TestIfPlayerInSight(Vector3 position, float range)
    {
        bool inSight = false;
        if(raycastManager != null)
        {
            inSight = raycastManager.IsPlayerInRaycastRange(position, range);
            return inSight;
        }
        return false;
    }

    /// <summary>
    /// Notify SpawnManager OfDeath. 
    /// </summary>
    public void NotifyEnemyManagerDeath()
    {
        EnemySpawnManager enemySpawnManager = FindObjectOfType<EnemySpawnManager>();
        enemySpawnManager.RemoveEnemyFromList(this);
    }


    private void SetSoundLastPostionOnPath(Vector3 soundPosition)
    {

        //Debug.Log("Distance to position: " + Vector3.Distance(soundPosition, transform.position));

        NavMeshPath path = new NavMeshPath();
        //soundPosition.y = transform.position.y;

        if(NavMesh.CalculatePath(transform.position, soundPosition, NavMesh.GetAreaFromName("walkable"), path))
        {
            float lng = 0.0f;

            Vector3 corner;

            if ((path.status != NavMeshPathStatus.PathInvalid) && (path.corners.Length > 1))
            {
                //Debug.Log(path.corners.Length);
                if (path.corners.Length <= 2)
                {
                    soundLastPosition = soundPosition;
                    return;
                }

                for (int i = 1; i < path.corners.Length; ++i)
                {
                    lng += Vector3.Distance(path.corners[i - 1], path.corners[i]);

                    if (lng > stats.distanceToMoveTowardsSound)
                    {
                        corner = path.corners[i-1];
                        soundLastPosition = corner;
                        //Debug.Log("Distance to travel: " + Vector3.Distance(corner, transform.position));
                        return;
                    }
                }
            }
            soundLastPosition = soundPosition;
        }
    }


    public void onHurt()
    {
        EnemyHurt = true;
        //soundLastPosition = Player.transform.position;
        if (Destructible.Health <= Stats.healthThresholdForReaction * Destructible.MaxHealth && !HealthThresholdReaction)
        {
            if (typeOfEnemy == enemyType.big)
            {
                Enrage = true;
            }
            if (typeOfEnemy == enemyType.medium)
            {
                StateMachine.Play("CallBackup");
            }
            HealthThresholdReaction = true;
        }
        if (Destructible.Health <= Destructible.MaxHealth / 2 && !BeenHealthStunned)
        {
            StateMachine.Play("Stunned");
            BeenHealthStunned = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, Vector3.down * rayCastRange);

        // Draw damage sphere
        if (typeOfEnemy != enemyType.swarm)
        {
            Gizmos.DrawWireSphere(transform.position + (transform.forward * damagePosition.z) + (transform.up * damagePosition.y), radiusOfDamageSphere);
        }
    }

    private void OnDisable()
    {
        StopSounds();
    }

    private void OnDestroy()
    {
        StopSounds();
    }

    public void PlayEcho(float ecolocationType)                             // Used by Animator
    {
        if (soundEmitter == null)
        {
            Debug.Log("SoundEmitter was null on AI");
            return;
        }
        soundEmitter.EventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        soundEmitter.Event = echoSound;
        soundEmitter.Play();
        soundEmitter.EventInstance.release();
        soundEmitter.EventInstance.setParameterByName("Ecolocation", ecolocationType);
        if (TestIfPlayerInSight(transform.position, 20.0f))
        {
            soundEmitter.EventInstance.setParameterByName("Snapshot", 1.0f);
        }
        else
        {
            soundEmitter.EventInstance.setParameterByName("Snapshot", 0.0f);
        }
    }

}
