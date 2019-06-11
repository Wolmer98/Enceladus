using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawnPack", menuName = "AI/EnemySpawnPack")]
public class EnemySpawnPack : ScriptableObject
{
    public GameObject[] enemies;
}
