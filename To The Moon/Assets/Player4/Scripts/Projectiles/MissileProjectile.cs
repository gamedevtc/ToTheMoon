using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MissileProjectile : ProjectileBase
{
    Vector3 position;
    Vector3 velocity;
    Transform cachedTransform;

    void Start()
    {
        cachedTransform = transform;
        position = cachedTransform.position;
        float startSpeed = (stats.minSpeed + stats.maxSpeed) * 0.5f;
        velocity = transform.forward * startSpeed;
        Destroy(this.gameObject, stats.bulletLife);
    }


    private void Update()
    {
        if (paused)
        {
            return;
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, stats.detectRadius, stats.mask);
        foreach (Collider nearObj in colliders)
        {
            if (nearObj.CompareTag(FlareLockTag))
            {
                setTarget(nearObj.gameObject);
                break;
            }
        }
    }

    void FixedUpdate()
    {
        if (paused)
        {
            return;
        }

        Vector3 acceleration = Vector3.zero;
        if (target != null)
        {
            Vector3 offsetToTarget = (target.transform.position - position);
            Vector3 outVec = offsetToTarget.normalized * stats.maxSpeed - velocity;
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

        speed = Mathf.Clamp(speed, stats.minSpeed, stats.maxSpeed);

        velocity = dir * speed;

        cachedTransform.position += velocity * Time.deltaTime;
        cachedTransform.forward = dir;
        position = cachedTransform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag != null)
        {
            if (collision.transform.CompareTag(EnemyLockTag))
            {
                collision.transform.gameObject.GetComponent<AIUnit>().TakeDamage((int)stats.damage, shooter);
                Instantiate(stats.onHitEffect, collision.contacts[0].point, collision.transform.rotation);
                Destroy(this.gameObject);
            }

            if (collision.transform.CompareTag(AsterLockTag))
            {
                Instantiate(stats.onHitEffect, collision.contacts[0].point, collision.transform.rotation);
                Destroy(this.gameObject);
            }

            if (collision.transform.CompareTag(FlareLockTag))
            {
                Instantiate(stats.onHitEffect, collision.contacts[0].point, collision.transform.rotation);
                Destroy(collision.gameObject);
                Destroy(this.gameObject);
            }
        } 
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (GameManagerBase.Instance.isMulti())
        {
            if (collision.transform.CompareTag(PlayerLockTag))
            {
                if (!M_isSelf(collision.transform.GetComponent<PhotonView>().ViewID))
                {
                    collision.gameObject.GetComponentInParent<MultiPlayer4>().M_TakeDamage(stats.damage, m_shooter);
                    Instantiate(stats.onHitEffect, this.transform.position, collision.transform.rotation);
                    Destroy(this.gameObject);
                }
            }
        }
    }





}
