using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotifyEnemiesInRadius : MonoBehaviour
{
    [SerializeField] int spawnExtraPacks;
    [SerializeField] float maxSpawnEnemyDistance;
    [SerializeField] float maxRadiusForDetection;
    [SerializeField] LayerMask enemyDetection;
    [SerializeField] bool notifyExactLocation = true;
    [SerializeField] Transform transformForSoundPosition;


    private EnemySpawnManager spawnManager;
    // Start is called before the first frame update
    void Start()
    {
        spawnManager = FindObjectOfType<EnemySpawnManager>();
    }

    public void SpawnExtraEnemies(int packAmount)
    {
        if (transformForSoundPosition)
        {
            if (packAmount == 0)
            {
                packAmount = spawnExtraPacks;
            }
            spawnManager.SpawnWaveEnemies(transformForSoundPosition.position, maxSpawnEnemyDistance, packAmount);
            Debug.Log("Enemies spawned around: " + transformForSoundPosition.position);
        }
        else
        {
            spawnManager.SpawnWaveEnemies(transform.position, maxSpawnEnemyDistance, spawnExtraPacks);
            Debug.Log("Notifiy Enemies In Radius had no sound position set, using own transform: " + transform.position + " for SpawnExtraEnemies.");
        }
    }

    public void NotifyEnemies()
    {
        if (transformForSoundPosition)
        {
            DetectableSound.PlayDetectableSound(transformForSoundPosition.position, maxRadiusForDetection, enemyDetection, notifyExactLocation);
            Debug.Log("Played sound from: " + transformForSoundPosition.position);
        }
        else
        {
            DetectableSound.PlayDetectableSound(transform.position, maxRadiusForDetection, enemyDetection, notifyExactLocation);
            Debug.Log("Notifiy Enemies In Radius had no sound position set, using own transform: " + transform.position + " for NotifyEnemies.");
        }
    }

}
