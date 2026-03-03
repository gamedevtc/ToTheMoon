using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    [SerializeField] private Material DissolveMat;
    private float timer = 0;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            timer += 0.01f;
        }
        DissolveMat.SetFloat("Timer", timer);
    }
}
