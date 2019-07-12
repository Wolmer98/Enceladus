using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [Range(0,1)]
    [SerializeField] float chanceToSpawn = 0.5f;

    [SerializeField] EnemySpawnPack[] enemySpawnPointPacks;

    private EnemySpawnManager enemySpawnManager;

    public float ChanceToSpawn { get { return chanceToSpawn; } }
    public bool IsClosed { get; set; }
    public EnemySpawnPack EnemySpawnPack
    {
        get
        {
            if (enemySpawnPointPacks != null)
            {
                
                return FindCurrectPackForLevel();
            }
            else
            {
                return null;
            }

        }
    }

    private EnemySpawnPack FindCurrectPackForLevel()
    {
        if (enemySpawnManager == null)
        {
            enemySpawnManager = FindObjectOfType<EnemySpawnManager>();
        }
        EnemySpawnPack pack = enemySpawnPointPacks[Random.Range(0, enemySpawnPointPacks.Length)];
        if (pack != null)
        {
            if (pack.minLevel <= enemySpawnManager.CurrentLevel)
            {
                return pack;
            }
            else
            {
                for (int i = 0; i < enemySpawnPointPacks.Length; i++)
                {
                    pack = enemySpawnPointPacks[i];
                    if (pack.minLevel <= enemySpawnManager.CurrentLevel)
                    {
                        return pack;
                    }
                }
                return null;
            }
        }
        else
            return null;
    }

    /// <summary>
    /// Calls SpawnManger to spawn enemies on this point.
    /// </summary>
    public void SpawnEnemy()
    {
        if (enemySpawnManager == null)
        {
            enemySpawnManager = FindObjectOfType<EnemySpawnManager>();
        }
        enemySpawnManager.SpawnEventEnemies(this, EnemySpawnPack);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(1f,1f,1f));
    }
}
