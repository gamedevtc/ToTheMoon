using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using System.IO;
using UnityEngine;

public class MissileWeap : Weapon
{
    [SerializeField] string LockTag = "Enemy";

    [SerializeField] GameObject target;
    [SerializeField] float collisionBoundsRadius;
    [SerializeField] float LockOnDistance;

    public override void Shoot(GameObject shooter)
    {
        if (cooldown >= weaponCooldown)
        {
            RaycastHit hit;
            if (Physics.SphereCast(gunOrigin.transform.position, collisionBoundsRadius, gunOrigin.transform.forward, out hit, LockOnDistance))
            {
                if (hit.transform.CompareTag(LockTag))
                {

                    target = hit.transform.gameObject;
                }
            }

            weaponSound.Play();
            GameObject bullet;
            if (GameManagerBase.Instance.isMulti() == true)
            {
                bullet = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "M_bullet"), gunOrigin.transform.position, Quaternion.LookRotation(gunOrigin.transform.forward));
            }
            else
            {
                bullet = Instantiate(bulletOG, gunOrigin.transform.position, Quaternion.LookRotation(gunOrigin.transform.forward));
            }
            bullet.GetComponent<Missile>().setTarget(target, LockTag, shooter);
            target = null;
            cooldown -= cooldown;
        }
    }
}
