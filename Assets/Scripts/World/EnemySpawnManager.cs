using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawnManager : MonoBehaviour
{
    public bool spawnRoomEnemies = true;
    public bool spawnRandomEnemies = false;
    public enum EventPointExcluder { NotSeenByPlayer, SeenByPlayer, All }

    [Header("Enemy Types")]
    [SerializeField] GameObject[] enemyType;
    [SerializeField] EnemySpawnPack[] enemySpawnPacks;
    [Header("Enemy Counters")]
    [SerializeField] int maxEnemyCount = 20;
    [SerializeField] int enemyCount;
    [Header("Spawn Veriables")]
    [SerializeField] float minSpawnPointDistance = 3f;
    [SerializeField] float maxSpawnPointDistance = 20f;

    [Space]
    [SerializeField] float minSpawnPointNavDistance = 20f;
    [SerializeField] Vector2 spawnEnemyIntervals;
    [SerializeField] EventPointExcluder eventPointExcluder;

    [Space]
    [SerializeField] float rangeToDisableAI = 30f;
    [SerializeField] LayerMask obstructionMask;

    [Header("Difficulty Curves")]
    [SerializeField] AnimationCurve statModifier;
    [SerializeField] AnimationCurve increasedSpawnChance;
    [SerializeField] float difficultyMod = 1f;
    [SerializeField] float extraSpawnChance = 0;

    private WorldGenerator worldGenerator;
    private bool hasSubscribedToWorldGenerator;

    private EnemySpawnPoint[] enemySpawnPoints;
    private PlayerController player;
    private float spawnTimer;
    private List<AI> enemiesAlive;

    NavMeshPath playerToSpawnPointPath;

    private float timerForDisable;
    private int AiID;

    public EnemySpawnPoint[] EnemySpawnPoints { get { return enemySpawnPoints; } }

    private void Start()
    {
        enemiesAlive = new List<AI>();
    }

    // Update is called once per frame
    void Update()
    {
        //Random spawns
        if (spawnRandomEnemies)
        {
            UpdateSpawnCounter();
        }

        timerForDisable += Time.deltaTime;
        if(timerForDisable < 0.5f)
        {
            DisableEnableAIFarAway();
            timerForDisable = 0;
        }

    }

    private void SpawnOneEnemy()
    {
        GameObject enemy = Instantiate(enemyType[0], player.transform.position, Quaternion.identity, transform);
    }

    /// <summary>
    /// Called by worldgenerator when world and spawnpoints are placed.
    /// </summary>
    public void InitSpawnManager()
    {
        if (!hasSubscribedToWorldGenerator)
        {
            worldGenerator = FindObjectOfType<WorldGenerator>();
            worldGenerator.OnWorldStart.AddListener(delegate { IncreaseDifficulty(); });
            hasSubscribedToWorldGenerator = true;
        }


        player = FindObjectOfType<PlayerController>();
        playerToSpawnPointPath = new NavMeshPath();

        enemySpawnPoints = FindObjectsOfType<EnemySpawnPoint>();

        spawnTimer = Random.Range(spawnEnemyIntervals.x, spawnEnemyIntervals.y);
        if(spawnRoomEnemies)
        {
            SpawnRoomEnemies();
        }
    }

    private void SpawnRoomEnemies()
    {
        for (int i = 0; i < enemySpawnPoints.Length; i++)
        {
            float chanceToSpawn = enemySpawnPoints[i].ChanceToSpawn + extraSpawnChance;
            if (Random.Range(0f, 1f) < chanceToSpawn && enemyCount < maxEnemyCount)
            {
                if(enemyCount >= maxEnemyCount)
                {
                    break;
                }
                //SpawnEnemy(enemySpawnPoints[i]);
                SpawnFromPack(enemySpawnPoints[i], enemySpawnPoints[i].EnemySpawnPack);
            }
        }
    }

    public void SpawnEventEnemies(EnemySpawnPoint enemySpawnPoint, EnemySpawnPack enemySpawnPack)
    {
        if(enemySpawnPack == null)
        {
            enemySpawnPack = enemySpawnPacks[Random.Range(0, enemySpawnPacks.Length)];
        }

        SpawnFromPack(enemySpawnPoint, enemySpawnPack);
    
    }

    private void UpdateSpawnCounter()
    {
        spawnTimer -= Time.deltaTime;
        if(spawnTimer <= 0)
        {
            if(SpawnEnemy(null))
            {
                spawnTimer = Random.Range(spawnEnemyIntervals.x, spawnEnemyIntervals.y);
            }
        }
    }

    private bool SpawnEnemy(EnemySpawnPoint enemySpawnPoint)
    {
        // If enemyspawnpoint is null then find closeSpawnpoint, else use the one passed into function-
        if(enemySpawnPoint == null)
        {
            if (enemySpawnPoints == null || enemyType == null || enemyCount >= maxEnemyCount)
            {
                return false;
            }

            EnemySpawnPoint[] closeSpawnPoints = GetCloseSpawnPoints(eventPointExcluder, maxSpawnPointDistance);

            if (closeSpawnPoints != null)
            {

                EnemySpawnPoint spawnPoint = closeSpawnPoints[Random.Range(0, closeSpawnPoints.Length)];
                EnemySpawnPack spawnPointPack = spawnPoint.EnemySpawnPack;

                if (spawnPointPack == null)
                {
                    EnemySpawnPack enemySpawnPack = enemySpawnPacks[Random.Range(0, enemySpawnPacks.Length)];

                    SpawnFromPack(spawnPoint, enemySpawnPack);
                    return true;
                }
                else 
                {
                    SpawnFromPack(spawnPoint, spawnPointPack);
                    return true;
                }
            }
            return false;
        }
        else
        {
            if(NavMesh.CalculatePath(player.transform.position, enemySpawnPoint.transform.position, NavMesh.AllAreas, playerToSpawnPointPath))
            {
                if(GetPathLength(playerToSpawnPointPath) >  minSpawnPointDistance)
                {
                    EnemySpawnPack spawnPointPack = enemySpawnPoint.EnemySpawnPack;
                    if (spawnPointPack == null)
                    {
                        EnemySpawnPack enemySpawnPack = enemySpawnPacks[Random.Range(0, enemySpawnPacks.Length)];

                        SpawnFromPack(enemySpawnPoint, enemySpawnPack);
                        return true;
                    }
                    else
                    {
                        SpawnFromPack(enemySpawnPoint, spawnPointPack);
                        return true;
                    }
                }
            }
            return false;
        }

    }

    private void SpawnFromPack(EnemySpawnPoint spawnPoint, EnemySpawnPack spawnPointPack)
    {
        for (int i = 0; i < spawnPointPack.enemies.Length; i++)
        {
            if (enemyCount >= maxEnemyCount)
            {
                break;
            }
            else
            {
                GameObject enemy = (spawnPointPack.enemies[i]);
                InitAi(spawnPoint, enemy);
            }
        }
    }

    private EnemySpawnPoint[] GetCloseSpawnPoints(EventPointExcluder eventPointExcluder, float maxRange)
    {
        List<EnemySpawnPoint> closeSpawnPoints = new List<EnemySpawnPoint>();
        foreach (EnemySpawnPoint esp in enemySpawnPoints)
        {
            if (esp == null || esp.IsClosed)
            {
                continue;
            }

            if (Vector3.Distance(esp.transform.position, player.transform.position) < maxRange)
            {
                if (eventPointExcluder == EventPointExcluder.NotSeenByPlayer)
                {
                    // Skip if the point is seen by player.
                    if (PointIsSeen(player.MainCamera, esp.transform.position))
                    {
                        continue;
                    }
                }
                else if (eventPointExcluder == EventPointExcluder.SeenByPlayer)
                {
                    // Skip if the point is NOT seen by player.
                    if (!PointIsSeen(player.MainCamera, esp.transform.position))
                    {
                        continue;
                    }
                }

                if (NavMesh.CalculatePath(player.transform.position, esp.transform.position, NavMesh.AllAreas, playerToSpawnPointPath))
                {
                    if (GetPathLength(playerToSpawnPointPath) > minSpawnPointNavDistance)
                    {
                        closeSpawnPoints.Add(esp);
                        //esp.IsClosed = true; use to allow only one to spawn per point.
                    }
                }
            }
        }

        if (closeSpawnPoints.Count > 0)
        {
            return closeSpawnPoints.ToArray();
        }
        else
        {
            return null;
        }
    }

    private void InitAi(EnemySpawnPoint enemySpawnPoint, GameObject enemyToSpawn)
    {
        GameObject enemy = Instantiate(enemyToSpawn, enemySpawnPoint.transform.position, Quaternion.identity, transform);
        enemy.GetComponent<AI>().InitAi(enemySpawnPoint.GetComponentInParent<Room>(), difficultyMod, AiID, this);
        if(enemy.GetComponent<AI>().TypeOfEnemy != global::enemyType.swarm)
        {
            enemy.GetComponentInChildren<Destructible>().OnDeath.AddListener(delegate { RemoveEnemy(); });
        }
        enemyCount++;
        AiID++;
        enemiesAlive.Add(enemy.GetComponent<AI>());
    }

    private static bool PointIsSeen(Camera camera, Vector3 point)
    {
        Vector3 viewPortPoint = camera.WorldToViewportPoint(point);
        return viewPortPoint.x >= 0 && viewPortPoint.x <= 1 &&
               viewPortPoint.y >= 0 && viewPortPoint.y <= 1 &&
               viewPortPoint.z >= 0;
    }

    private static float GetPathLength(NavMeshPath path)
    {
        float lng = 0.0f;

        if ((path.status != NavMeshPathStatus.PathInvalid) && (path.corners.Length > 1))
        {
            for (int i = 1; i < path.corners.Length; ++i)
            {
                lng += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }
        }

        return lng;
    }

    /// <summary>
    /// Sets the enemySpawnPoints array to null.
    /// </summary>
    public void ClearSpawnPoints()
    {
        enemySpawnPoints = null;
    }

    /// <summary>
    /// Decreases the enemy counter.
    /// </summary>
    public void RemoveEnemy()
    {
        enemyCount--;
        enemyCount = Mathf.Max(enemyCount, 0);
    }

    /// <summary>
    /// Removes (Destroys) all enemies.
    /// </summary>
    public void RemoveAllEnemies()
    {
        if (enemiesAlive != null && enemiesAlive.Count > 0)
        {
            for (int i = 0; i < enemiesAlive.Count; i++)
            {
                if(enemiesAlive[i] == null)
                {
                    continue;
                }

                enemiesAlive[i].StopSounds();
                Destroy(enemiesAlive[i].gameObject);           
            }
            enemyCount = 0;
            enemiesAlive.Clear();
        }
    }

    /// <summary>
    /// Spawns enemies in a radius of a position.
    /// </summary>
    public void SpawnWaveEnemies(Vector3 originPosision, float radiusOfOrigin, int amount)
    {
        EnemySpawnPoint[] enemySpawnPoints = GetCloseSpawnPoints(eventPointExcluder, radiusOfOrigin);
        if (enemySpawnPoints == null)
        {
            return;
        }
        for (int i = 0; i < amount; i++)
        {
            int randomSpawnPoint = Random.Range(0, enemySpawnPoints.Length);
            SpawnEnemy(enemySpawnPoints[randomSpawnPoint]);
        }
    }

    /// <summary>
    /// Removes an Ai from it's list.
    /// </summary>
    public void RemoveEnemyFromList(AI ai)
    {
        enemyCount--;
        enemiesAlive.Remove(ai);
    }


    /// <summary>
    /// Subscribes to the worldgenerators WorldStart event
    /// </summary>
    public void IncreaseDifficulty()
    {
        if(worldGenerator)
        {
            difficultyMod = 1f + statModifier.Evaluate(worldGenerator.CurrentLevel/10f);
            Debug.Log("CurrentLeve:" + worldGenerator.CurrentLevel + "     Stat modifier: " + difficultyMod);
            extraSpawnChance = increasedSpawnChance.Evaluate(worldGenerator.CurrentLevel / 10);
            Debug.Log("Extra spawn chance of enemies are now : " + extraSpawnChance);

        }
    }

    public bool CanPlayerSeePoint(Vector3 position)
    {
        bool pointInCamera = PointIsSeen(player.MainCamera, position);

        if (pointInCamera)
        {
            Vector3 dir = (position - player.MainCamera.transform.position);
            float dist = Vector3.Distance(position, player.MainCamera.transform.position);
            RaycastHit hit;
            if (Physics.Raycast(player.MainCamera.transform.position, dir, out hit, dist, obstructionMask))
            {
                //Debug.Log("false");
                return false;
            }
            else
            {
                //Debug.Log("True");
                return true;
            }
        }

        //Debug.Log(pointInCamera + " pointInCamera");
        return pointInCamera;

    }

    // Fix to disable AI far away from player.
    private void DisableEnableAIFarAway()
    {
        if(enemiesAlive == null || enemiesAlive.Count == 0)
        {
            return;
        }
        for (int i = 0; i < enemiesAlive.Count; i++)
        {
            if(enemiesAlive[i] == null)
            {
                continue;
            }
            if(Vector3.Distance(enemiesAlive[i].transform.position, player.transform.position) > rangeToDisableAI && !enemiesAlive[i].ChasingPlayer)
            {
                if (enemiesAlive[i].gameObject.activeSelf)
                {
                    enemiesAlive[i].gameObject.SetActive(false);
                    enemiesAlive[i].Rigidbody.velocity = new Vector3(0, 0, 0);
                    enemiesAlive[i].StopSounds();
                }
            }
            else if (!enemiesAlive[i].gameObject.activeSelf)
            {
                enemiesAlive[i].gameObject.SetActive(true);
            }
        } 
    }
}
