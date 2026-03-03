using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using System.IO;
using UnityEngine;

public class MissileShotWeapon : WeaponBase
{
    [SerializeField] GameObject target;
    [SerializeField] string targetTag;

    public override void MultiplayerFire()
    {
        if (rateOfFire >= stats.fireCoolDown)
        {
            RaycastHit hit;
            if (Physics.SphereCast(gunOrigin.transform.position, stats.collisionBoundsRadius, shipMain.transform.forward, out hit, stats.LockOnDistance))
            {
                if (hit.transform.gameObject.CompareTag(stats.PlayerLockTag))
                {
                    if (hit.transform.gameObject.GetComponent<PhotonView>().ViewID != shipMain.getPlayerMain().gameObject.GetComponent<PhotonView>().ViewID)
                    {
                        target = hit.transform.gameObject;
                        targetTag = hit.transform.gameObject.tag;
                    }
                }
            }

            GameObject miss = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", stats.projectilePrefabMulti.name), gunOrigin.transform.position, Quaternion.LookRotation(shipMain.transform.forward));
            miss.GetComponent<ProjectileBase>().callRPC(shipMain);
            miss.GetComponent<ProjectileBase>().setTarget(target);
            target = null;
            rateOfFire -= rateOfFire;
        }
    }
    public override void SingleplayerFire()
    {
        if (rateOfFire >= stats.fireCoolDown && !paused)
        {
            RaycastHit hit;
            if (Physics.SphereCast(gunOrigin.transform.position, stats.collisionBoundsRadius, shipMain.transform.forward, out hit, stats.LockOnDistance))
            {
                if (hit.transform.CompareTag(stats.EnemyLockTag))
                {
                    target = hit.transform.gameObject;
                }
            }

            GameObject miss = Instantiate(stats.projectilePrefabSingle, gunOrigin.transform.position, Quaternion.LookRotation(shipMain.transform.forward));
            miss.GetComponent<ProjectileBase>().setShooter(shipMain.getPlayerMain().gameObject);
            miss.GetComponent<ProjectileBase>().setTarget(target);
            target = null;
            rateOfFire -= rateOfFire;

        }
    }
}
