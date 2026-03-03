using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition.Attributes;

[ExecuteInEditMode]
public class ReplaceAllMaterials : MonoBehaviour
{
    [SerializeField] Material materialToApply;
    [SerializeField] bool run = false;

    private void Update()
    {
        if (run)
        {
            FindandReplaceAll();
            run = false;
        }
    }

    void FindandReplaceAll()
    {
        MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mesh in meshes)
        {
            List<Material> mats = new List<Material>();
            mesh.GetSharedMaterials(mats);
            for (int i = 0; i < mats.Count; i++)
            {
                mats[i] = materialToApply;
            }
            mesh.sharedMaterials = mats.ToArray();
        }
    }
}
