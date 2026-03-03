using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player3Body : MonoBehaviour
{
    public struct bodyStats
    {
        public float speed;
        public float accSpeed;
        public float decSpeed;
        public float maxHealth;
        public float maxBoost;
        public float boostStrength;
        public float boostDecreaseModifier;
        public float handlingSpeed;
        public float zRotationSpeed;
    }

    [Header("Body Stats")]
    [SerializeField] Mesh shipMesh;
    [SerializeField] float speed;
    [SerializeField] float accSpeed;
    [SerializeField] float decSpeed;
    [SerializeField] float handlingSpeed;
    [SerializeField] float zRotationSpeed;
    [SerializeField] float maxHealth;
    [SerializeField] float maxBoost;
    [SerializeField] float boostStrength;
    [SerializeField] float boostDecreaseModifier;
    [SerializeField] bool changesColor;

    Player3 playerMain;
    MeshRenderer[] meshes;

    private void Awake()
    {
        if (changesColor)
        {
            meshes = GetComponentsInChildren<MeshRenderer>();
        }
    }

    public void getStats(out bodyStats assign)
    {
        assign.speed = speed;
        assign.accSpeed = accSpeed;
        assign.decSpeed = decSpeed;
        assign.maxHealth = maxHealth;
        assign.maxBoost = maxBoost;
        assign.boostStrength = boostStrength;
        assign.boostDecreaseModifier = boostDecreaseModifier;
        assign.handlingSpeed = handlingSpeed;
        assign.zRotationSpeed = zRotationSpeed;
    }

    //Write this
    public void updateColor(Material color)
    {
        if (changesColor)
        {
            for (int i = 0; i < meshes.Length; i++)
            {
                meshes[i].material = color;
            }
        }
    }
    private void OnEnable()
    {
        playerMain = GetComponentInParent<Player3BodyHolder>().getPlayer();
        if (changesColor)
        {
            updateColor(playerMain.getActiveColor());
        }
    }

    public Mesh getMesh()
    {
        return shipMesh;
    }
}
