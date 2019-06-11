using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [Range(0,1)]
    [SerializeField] float chanceToSpawn = 0.5f;

    [SerializeField] EnemySpawnPack[] enemySpawnPointPacks;

    public float ChanceToSpawn { get { return chanceToSpawn; } }
    public bool IsClosed { get; set; }
    public EnemySpawnPack EnemySpawnPack
    {
        get
        {
            if (enemySpawnPointPacks != null)
            {
                return enemySpawnPointPacks[Random.Range(0, enemySpawnPointPacks.Length)];
            }
            else
            {
                return null;
            }

        }
    }

    /// <summary>
    /// Calls SpawnManger to spawn enemies on this point.
    /// </summary>
    public void SpawnEnemy()
    {
        FindObjectOfType<EnemySpawnManager>().SpawnEventEnemies(this, EnemySpawnPack);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(1f,1f,1f));
    }
}
