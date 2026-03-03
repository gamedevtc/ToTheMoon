using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingLoadingScene : MonoBehaviour
{
    [SerializeField] float movingSpeed = 50;
    // Update is called once per frame
    void Update()
    {
        this.transform.position += this.transform.forward * movingSpeed * Time.deltaTime;
    }
}
