using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI Settings/New AI Behaviour")]
public class BehaviourSettings : ScriptableObject
{
    [SerializeField] public string BehaviourName;
    [SerializeField] public float rarity;

    [Header("Ship Stats")]
    [SerializeField] public int Health = 100;
    [SerializeField] public float Mass = 100;
    [SerializeField] public bool IsLeader = false;
    [SerializeField] public int NumOfAllowedFollowers = 0;
    [SerializeField] public float leaderFollowTime = 3060;

    [Header("Behaviour Steering Speeds")]
    [SerializeField] public float PursuitMinSpeed = 100;
    [SerializeField] public float PursuitMaxSpeed = 150;
    [SerializeField] public float FleeMinSpeed = 100;
    [SerializeField] public float FleeMaxSpeed = 150;
    [SerializeField] public float WanderMinSpeed = 100;
    [SerializeField] public float WanderMaxSpeed = 150;
    [SerializeField] public float FollowMinSpeed = 100;
    [SerializeField] public float FollowMaxSpeed = 150;
    [SerializeField] public float AttackMinSpeed = 100;
    [SerializeField] public float AttackMaxSpeed = 150;
    [SerializeField] public float ChaseMinSpeed = 100;
    [SerializeField] public float ChaseMaxSpeed = 150;
    [SerializeField] public float EscapeMinSpeed = 100;
    [SerializeField] public float EscapeMaxSpeed = 150;

    [Header("Behaviour Steering Caps")]
    [SerializeField] public float PursuitSteerForceCap = 5;
    [SerializeField] public float FleeSteerForceCap = 5;
    [SerializeField] public float WanderSteerForceCap = 5;
    [SerializeField] public float FollowSteerForceCap = 5;
    [SerializeField] public float AttackSteerForceCap = 5;
    [SerializeField] public float ChaseSteerForceCap = 5;
    [SerializeField] public float EscapeSteerForceCap = 5;

    [Header("Behaviour Weights")]
    [SerializeField] public float BehaviourForceWeightPursuit = 100;
    [SerializeField] public float BehaviourForceWeightFlee = 100;
    [SerializeField] public float BehaviourForceWeightWander = 100;
    [SerializeField] public float BehaviourForceWeightFollow = 100;
    [SerializeField] public float BehaviourForceWeightAttack = 100;
    [SerializeField] public float BehaviourForceWeightChase = 100;
    [SerializeField] public float BehaviourForceWeightEscape = 100;

    [Header("Collision Detection Values")]
    [SerializeField] public float CollisionAvoidRayRadius = 20;
    [SerializeField] public float CollisionAvoidRayDistance = 100;
    [SerializeField] public float CollisionAvoidForceWeightPursuit = 100;
    [SerializeField] public float CollisionAvoidForceWeightFlee = 100;
    [SerializeField] public float CollisionAvoidForceWeightWander = 100;
    [SerializeField] public float CollisionAvoidForceWeightFollow = 100;
    [SerializeField] public float CollisionAvoidForceWeightAttack = 100;
    [SerializeField] public float CollisionAvoidForceWeightChase = 100;
    [SerializeField] public float CollisionAvoidForceWeightEscape = 100;

    [Header("Separation Values")]
    [SerializeField] public float SeperationWeight = 100;
    [SerializeField] public float SeparationForceWeightPursuit = 100;
    [SerializeField] public float SeparationForceWeightFlee = 100;
    [SerializeField] public float SeparationForceWeightWander = 100;
    [SerializeField] public float SeparationForceWeightFollow = 100;
    [SerializeField] public float SeparationForceWeightAttack = 100;
    [SerializeField] public float SeparationForceWeightChase = 100;
    [SerializeField] public float SeparationForceWeightEscape = 100;

    [Header("Arrival Numbers")]
    [SerializeField] public float ArrivalRadius = 150;
    [SerializeField] public float ArrivalMaxCloseness = 25;

    [Header("Wander Numbers")]
    [SerializeField] public float WanderCircleDistance = 150;
    [SerializeField] public float WanderCircleRadius = 150;
    [SerializeField] public float WanderAngleChange = 2.5f;
    [SerializeField] public float WanderBeforeSearchingTime = 5;
    [SerializeField] public float wanderChangeTime = 7.5f;

    [Header("Time before giving up pursuit")]
    [SerializeField] public float GiveUpPursuitTime = 20;
    [SerializeField] public float GiveUpChaseTime = 10;

    [Header("If Units get too close")]
    [SerializeField] public float TooCloseRadius = 25;
    [SerializeField] public float TooCloseFleeTime = 2;

    [Header("Target Escape Distance")]
    [SerializeField] public float TargetDetectRadius = 100;
    [SerializeField] public float TargetForgetDistance = 200;

    [Header("Follow numbers")]
    [SerializeField] public float leaderFollowDistance = 25;

    [Header("Combat Values")]
    [SerializeField] public float fireSightDistance = 150;
    [SerializeField] public float fireSightRadius = 15;
    [SerializeField] public float BulletDamage = 10;
    [SerializeField] public float BulletCooldown = 0.5f;
    [SerializeField] public GameObject bulletPrefabSingle;
    [SerializeField] public GameObject bulletPrefabMulti;

}
