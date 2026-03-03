using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ship Stats/New Weapon Stat Sheet")]
public class WeaponStats : ScriptableObject
{
    [Header("Weapon Specific")]
    [SerializeField] public GameObject projectilePrefabSingle;
    [SerializeField] public GameObject projectilePrefabMulti;
    [SerializeField] public float fireCoolDown;
    [SerializeField] public LayerMask layerTarget;

    [Header("Ult Specific")]
    [SerializeField] public float ultChargeTime;
    [SerializeField] public float ultGainMultiplier;

    
    [Header("Missile Specific")]
    [SerializeField] public float collisionBoundsRadius;
    [SerializeField] public float LockOnDistance;
    [SerializeField] public string EnemyLockTag = "Enemy";
    [SerializeField] public string PlayerLockTag = "Player";

    [Header("Burst Laser Specific")]
    [SerializeField] public float burstSpread;
    [SerializeField] public float burstRotation;

    [Header("Devastator Specific")]
    [SerializeField] public float damageDev;
    [SerializeField] public float maxBeamDistance;
}
