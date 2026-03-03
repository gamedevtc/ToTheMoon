using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuRotator : MonoBehaviour
{
    [SerializeField] public float rotateSpeed = 1;
   

    // Update is called once per frame
    void Update()
    {
        transform.rotation = transform.rotation * new Quaternion(0, rotateSpeed * Time.deltaTime, 0, 1);
    }
}
