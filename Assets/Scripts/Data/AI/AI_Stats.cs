using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "AI_Stats", menuName = "AI/Stats")]
public class AI_Stats : ScriptableObject
{
    [Header("Combat")]
    public float attackRange = 2.5f;
    public float damage = 20;
    public float attackSpeed = 2;
    public float stunDuration = 2f;

    [Header("Charge")]
    public bool ShowCharge = true;
    [ShowIf("ShowCharge")]public float chargeCooldown = 6.0f;
    [ShowIf("ShowCharge")] public Vector2 chargeRangeInterval;
    [ShowIf("ShowCharge")] public float chargeWindupTime = 0.5f;

    [Header("Detection")]
    public bool ShowDetection = true;
    [ShowIf("ShowDetection")] public LayerMask targetsLayerMask;
    [ShowIf("ShowDetection")] public float detectionRadius = 2f;
    [ShowIf("ShowDetection")] public float chaseDetectionMultiplier = 2f;
    [ShowIf("ShowDetection")] public float timeBetweenAggroSound = 30f;
    [ShowIf("ShowDetection")] public float distanceToMoveTowardsSound = 10f;

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
    [Range(0f,1f)]
    public float echoDetectionMultiplier = 0.2f;
    [HideInInspector] public float echoDetectionNearRadius { get { return detectionRadius * (1 + echoDetectionMultiplier); } }

    public float nearRadiusDuration = 3f;
    [Range(0, 1)]
    public float chanceOfFarRadius= 0.2f;
    public float farRadiusLerpSpeed = 2f;
    [Range(0f, 1f)]
    public float echoDetectionFarMultiplier = 0.4f;
    [HideInInspector] public float echoDetectionFarRadius { get { return detectionRadius * (1 + echoDetectionFarMultiplier); } }
    public float farRadiusDuration = 2f;

    [Header("Allies")]
    public float awareOfFriendRadius = 5f;

    [Header("Stat Threshold")]
    [Range(0, 1)]
    public float healthThresholdForReaction;
    [Range(0, 1)]
    public float procentHealthStun;


    [Header("Stat boosts(Enrage)")]
    public float speedMultiplier = 1;
    public float damageMultiplier = 1;
    public float chargeSpeedMultiplier = 1;

}
