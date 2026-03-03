using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ship Stats/New Projectile Stat Sheet")]
public class ProjectileStats : ScriptableObject
{
    [SerializeField] public float bulletLife = 4f;
    [SerializeField] public float damage;
    [SerializeField] public float speed;
    [SerializeField] public bool tracking;
    [SerializeField] public GameObject onHitEffect;
    [SerializeField] public float minSpeed;
    [SerializeField] public float maxSpeed;
    [SerializeField] public LayerMask mask;

    [Header("Missile Specific")]
    [SerializeField] public float steerForceCap;
    [SerializeField] public float targetWeight;

    [Header("Bomb Specific")]
    [SerializeField] public float blastRadius;
    [SerializeField] public float detectRadius;
    [SerializeField] public float countDown;
    [SerializeField] public float minSpeedChase;
    [SerializeField] public float maxSpeedChase;

    [Header("EMP Specific")]
    [SerializeField] public float stunTime;

}
