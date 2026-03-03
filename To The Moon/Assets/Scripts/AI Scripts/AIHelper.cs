using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIHelper : MonoBehaviour
{
    //Settings
    [Header("Aggressive Ship Settings")]
    public float AggressiveMinSpeed = 100;
    public float AggressiveMaxSpeed = 150;
    public float AggressiveCollisionBoundsRadius = 20;
    public float AggressiveCollisionAvoidDistance = 100;
    public float AggressivePlayerDetectionRadius = 100;
    public float AggressiveTargetForgetDistance = 300;
    public float AggressiveTargetRadius = 15;
    public float AggressiveTargetDistance = 30;
    public int AggressiveHealth = 100;
    public float AggressiveBulletCooldown = 0.3f;

    [Header("Defensive Ship Settings")]
    public float DefensiveMinSpeed = 50;
    public float DefensiveMaxSpeed = 100;
    public float DefensiveCollisionBoundsRadius = 20;
    public float DefensiveCollisionAvoidDistance = 100;
    public float DefensivePlayerDetectionRadius = 50;
    public float DefensiveTargetForgetDistance = 150;
    public float DefensiveTargetRadius = 15;
    public float DefensiveTargetDistance = 30;
    public int DefensiveHealth = 100;
    public float ADefensiveBulletCooldown = 0.3f;

    [Header("Cover Ship Settings")]
    public float CoverMinSpeed = 75;
    public float CoverMaxSpeed = 125;
    public float CoverCollisionBoundsRadius = 10;
    public float CoverCollisionAvoidDistance = 40;
    public float CoverPlayerDetectionRadius = 75;
    public float CoverTargetForgetDistance = 150;
    public float CoverTargetRadius = 15;
    public float CoverTargetDistance = 30;
    public int CoverHealth = 100;
    public float CoverBulletCooldown = 0.3f;

    [Header("Layers")]
    public LayerMask obstacleMask;
    public LayerMask playerTargetMask;

    [Header("Perception")]
    public float NeighborVisionRadius = 50;
    public float AvoidRadius = 15;
    public float SteerForceCap = 5;

    [Header("Weights")]
    public float AvoidCollisionWeight = 10;
    public float SeperationWeight = 10;
    public float TargetWeight = 10;

    [Header("Testing")]
    public float bulletCoolDown = 0.3f;
    public int bursts = 3;


    const int numRayDirections = 300;

    public static readonly Vector3[] directions;

    static AIHelper()
    {
        directions = new Vector3[numRayDirections];

        float goldenRatio = (1 + Mathf.Sqrt(5)) * 0.5f;
        float angleIncrement = Mathf.PI * 2 * goldenRatio;

        for (int i = 0; i < numRayDirections; i++)
        {
            float t = (float)i / numRayDirections;
            float incline = Mathf.Acos(1 - 2 * t);
            float az = angleIncrement * i;

            float x = Mathf.Sin(incline) * Mathf.Cos(az);
            float y = Mathf.Sin(incline) * Mathf.Sin(az);
            float z = Mathf.Cos(incline);

            directions[i] = new Vector3(x, y, z);
        }
    }
}
