using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPath : MonoBehaviour
{
    [SerializeField] public List<Transform> nodes = new List<Transform>();
    [SerializeField] public float nodeRadius = 10;
    [SerializeField] public bool isPathKey = false;
    [SerializeField] public bool isChaseKey = false;
    [SerializeField] public float chaseDistance = 100;
    [SerializeField] public bool isPursuitKey = false;
}
