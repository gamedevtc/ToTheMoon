using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCameraScript : MonoBehaviour
{
    [SerializeField] public GameObject target;
    [SerializeField] public Vector3 staticPosition;
    [SerializeField] public float rotationSpeed = 0.5f;
    [SerializeField] public Camera deathCam;
    [SerializeField] public float RespawnTime = 5;

    void FixedUpdate()
    {
        if (target)
        {
            transform.LookAt(target.transform);
        }
        else if (staticPosition != Vector3.zero)
        {
            transform.LookAt(staticPosition);
        }
        else
        {
            transform.Rotate(new Vector3(0, rotationSpeed, 0));
        }

        Destroy(this.gameObject, RespawnTime);
    }

    public void setTarget(GameObject t)
    {
        target = t;
    }

    public void setPosition(Vector3 pos)
    {
        staticPosition = pos;
    }
}
