using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class EMPShot : ProjectileBase
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
    void FixedUpdate()
    {
        if (paused)
        {
            return;
        }

        if (target != null)
        {
            count -= Time.deltaTime;
        }

        if (target == null)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, stats.detectRadius);
            foreach (Collider nearObj in colliders)
            {
                if (nearObj.CompareTag(EnemyLockTag))
                {
                    setTarget(nearObj.gameObject);
                    break;
                }

                if (nearObj.CompareTag(PlayerLockTag))
                {
                    if (GameManagerBase.Instance.isMulti())
                    {
                        if (!M_isSelf(nearObj.gameObject.GetComponent<PhotonView>().ViewID))
                        {
                            setTarget(nearObj.gameObject);
                        }
                    }
                    else
                    {
                        if (!isSelf(nearObj.gameObject))
                        {
                            setTarget(nearObj.gameObject);
                        }
                    }
                    break;
                }
            }
        }

        Vector3 acceleration = Vector3.zero;
        if (target != null)
        {
            Vector3 offsetToTarget = (target.transform.position - position);
            Vector3 outVec = offsetToTarget.normalized * stats.maxSpeedChase - velocity;
            outVec = Vector3.ClampMagnitude(outVec, stats.steerForceCap);

            acceleration += outVec * stats.targetWeight;
        }
        else
        {
            acceleration += stats.minSpeed * transform.forward;
        }

        velocity += acceleration * Time.deltaTime;
        float speed = velocity.magnitude;
        Vector3 dir = velocity / speed;

        if (target != null)
        {
            speed = Mathf.Clamp(speed, stats.minSpeedChase, stats.maxSpeedChase);
        }
        else
        {
            speed = Mathf.Clamp(speed, stats.minSpeed, stats.maxSpeed);
        }


        velocity = dir * speed;

        cachedTransform.position += velocity * Time.deltaTime;
        cachedTransform.forward = dir;
        position = cachedTransform.position;

        if (count <= 0.0f)
        {
            Explode();
        }
    }

    void Explode()
    {

        Instantiate(stats.onHitEffect, transform.position, transform.rotation);
        //get nearby objects
        Collider[] colliders = Physics.OverlapSphere(transform.position, stats.blastRadius);

        foreach (Collider nearbyObj in colliders)
        {
            if (nearbyObj.CompareTag("Enemy"))
            {
                nearbyObj.GetComponent<AIUnit>().setStunnedTime(stats.stunTime);
            }

            if (GameManagerBase.Instance.isMulti())
            {
                if (nearbyObj.CompareTag("Player") && !M_isSelf(nearbyObj.gameObject.GetComponent<PhotonView>().ViewID))
                {
                    nearbyObj.GetComponent<Player4Base>().setStunTime(stats.stunTime);
                }
            }
            else
            {
                if (nearbyObj.CompareTag("Player") && !isSelf(nearbyObj.gameObject))
                {
                    nearbyObj.GetComponent<Player4Base>().setStunTime(stats.stunTime);
                }
            }

        }

        Destroy(this.gameObject);
    }
}
