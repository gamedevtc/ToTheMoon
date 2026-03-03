using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ship Stats/New Body Stat Sheet")]
public class BodyStats : ScriptableObject
{
    [Header("Body Stats")]
    [SerializeField] public float speed;
    [SerializeField] public float accSpeed;
    [SerializeField] public float decSpeed;
    [SerializeField] public float handlingSpeed;
    [SerializeField] public float zRotationSpeed;
    [SerializeField] public float maxHealth;
    [SerializeField] public float maxBoost;
    [SerializeField] public float boostStrength;
    [SerializeField] public float boostDecreaseModifier;
}
