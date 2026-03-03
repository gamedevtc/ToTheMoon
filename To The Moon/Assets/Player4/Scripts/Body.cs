using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    [SerializeField] public BodyStats stats;
    [SerializeField] public Mesh shipMesh;
    [SerializeField] public bool changesColor;

    [SerializeField] Ship shipMain;
    [SerializeField] MeshRenderer[] meshes;
    private void Awake()
    {
        shipMain = GetComponentInParent<BodyLink>().getShipMain();
        meshes = GetComponentsInChildren<MeshRenderer>();
    }

    public void setColor(Material mat)
    {
        if (changesColor)
        {
            for (int i = 0; i < meshes.Length; i++)
            {
                meshes[i].material = mat;
            }
        }
    }

    private void OnEnable()
    {
        if (changesColor)
        {
            if (shipMain)
            {
                setColor(shipMain.getActiveColor());
            }
        }
    }
}
