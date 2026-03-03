using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class BulletProjectile : ProjectileBase
{
    // Start is called before the first frame update
    void Start()
    {
        Vector3 moveDirection = new Vector3(0, 0, stats.speed);
        moveDirection = transform.TransformDirection(moveDirection);
        rb.velocity = new Vector3(moveDirection.x, moveDirection.y, moveDirection.z);
        Destroy(this.gameObject, stats.bulletLife);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag != null)
        {
            if (!isSelf(collision.transform.gameObject))
            {
                if (collision.transform.CompareTag(EnemyLockTag))
                {
                    collision.transform.gameObject.GetComponent<AIUnit>().TakeDamage(stats.damage, shooter);
                    Instantiate(stats.onHitEffect, collision.contacts[0].point, collision.transform.rotation);
                    Destroy(this.gameObject);
                }
            }
            
            if (collision.transform.CompareTag(AsterLockTag))
            {
                Instantiate(stats.onHitEffect, collision.contacts[0].point, collision.transform.rotation);
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
                if (!M_isSelf(collision.gameObject.GetComponent<PhotonView>().ViewID) && !isSelf(collision.gameObject))
                {
                    collision.transform.gameObject.GetComponentInParent<MultiPlayer4>().M_TakeDamage(stats.damage, m_shooter);
                    Instantiate(stats.onHitEffect, this.transform.position, collision.transform.rotation);
                    Destroy(this.gameObject);
                }
            }
        }
    }


}



