using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InvertMeshCollider : MonoBehaviour
{
    [SerializeField] bool debug;

    private void Awake()
    {
        if (debug)
        {
            Invert();
        }
    }
    public void Invert()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.triangles = mesh.triangles.Reverse().ToArray();

        gameObject.AddComponent<MeshCollider>();

        MeshCollider col = GetComponent<MeshCollider>();
        col.convex = true;
    }
}
