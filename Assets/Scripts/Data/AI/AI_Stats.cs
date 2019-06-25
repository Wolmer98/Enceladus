using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI_Stats", menuName = "AI/Stats")]
public class AI_Stats : ScriptableObject
{
    [Header("Combat")]
    public float attackRange = 2.5f;
    public float damage = 20;
    public float attackSpeed = 2;
    public float stunDuration = 2f;

    [Header("Charge")]
    public float chargeCooldown = 6.0f;
    public Vector2 chargeRangeInterval;
    public float chargeWindupTime = 0.5f;

    [Header("Detection")]
    public LayerMask targetsLayerMask;
    public float detectionRadius = 2f;
    public float chaseDetectionMultiplier = 2f;
    public float timeBetweenAggroSound = 30f;
    public float distanceToMoveTowardsSound = 10f;

    [Header("Movement")]
    public float moveSpeed = 1f;
    public float chaseSpeed = 3f;
    public float rotationDuration = 2.0f;
    public float rotationSpeed = 200f;
    public float chargeSpeed = 8f;

    [Header("Search Paramaters")]
    public float searchRadius = 3f;
    public float searchTime = 20f;
    public int nbrSearchPos = 10;

    [Header("Echo Location")]
    [Range(0, 1)]
    public float chanceOfNearRadius = 0.3f;
    public float nearRadiusLerpSpeed = 3f;
    public float echoDetectionNearRadius = 4f;
    public float nearRadiusDuration = 3f;
    [Range(0, 1)]
    public float chanceOfFarRadius= 0.2f;
    public float farRadiusLerpSpeed = 2f;
    public float echoDetectionFarRadius = 6f;
    public float farRadiusDuration = 2f;

    [Header("Allies")]
    public float awareOfFriendRadius = 5f;

    [Header("Stat Threshold")]
    [Range(0, 1)]
    public float healthThresholdForReaction;


    [Header("Stat boosts(Enrage)")]
    public float speedMultiplier = 1;
    public float damageMultiplier = 1;
    public float chargeSpeedMultiplier = 1;

}
