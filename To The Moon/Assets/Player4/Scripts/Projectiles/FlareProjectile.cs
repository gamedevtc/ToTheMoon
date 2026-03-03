using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlareProjectile : ProjectileBase
{
    Vector3 position;
    Vector3 velocity;
    Transform cachedTransform;
    float count;

    // Start is called before the first frame update
    void Start()
    {
        cachedTransform = transform;
        position = cachedTransform.position;
        float startSpeed = (stats.minSpeed + stats.maxSpeed) * 0.5f;
        velocity = transform.forward * -startSpeed;
        count = stats.countDown;
    }

    // Update is called once per frame
    void Update()
    {
        if (paused)
        {
            return;
        }

        count -= Time.deltaTime;

        if (count <= 0.0f)
        {
            Destroy(this.gameObject);
        }
    }
}
