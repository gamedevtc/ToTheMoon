using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class MissileSilo : WeaponBase
{

    [SerializeField] GameObject target;
    [SerializeField] string targetTag;


    public override void MultiplayerFire()
    {
        if (currUltValue >= stats.ultChargeTime && !paused)
        {
            RaycastHit hit;
            if (Physics.SphereCast(gunOrigin.transform.position, stats.collisionBoundsRadius, shipMain.transform.forward, out hit, stats.LockOnDistance))
            {
                
                //add check to make sure missile doesnt target shooter
                if (hit.transform.gameObject.CompareTag(stats.PlayerLockTag))
                {
                    
                    if (hit.transform.gameObject.GetComponent<PhotonView>().ViewID != shipMain.getPlayerMain().GetComponent<PhotonView>().ViewID)
                    {
                        
                        target = hit.transform.gameObject;
                        targetTag = hit.transform.gameObject.tag;
                    }
                }
            }

            {
                GameObject miss = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", stats.projectilePrefabMulti.name), gunOrigin.transform.position+ new Vector3(stats.burstSpread, stats.burstSpread), Quaternion.LookRotation(shipMain.transform.forward));
                miss.GetComponent<ProjectileBase>().callRPC(shipMain);
                miss.GetComponent<ProjectileBase>().setTarget(target);
            }

            {
                GameObject miss = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", stats.projectilePrefabMulti.name), gunOrigin.transform.position+ new Vector3(-stats.burstSpread, stats.burstSpread), Quaternion.LookRotation(shipMain.transform.forward));
                miss.GetComponent<ProjectileBase>().callRPC(shipMain);
                miss.GetComponent<ProjectileBase>().setTarget(target);
            }

            {
                GameObject miss = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", stats.projectilePrefabMulti.name), gunOrigin.transform.position+ new Vector3(-stats.burstSpread, -stats.burstSpread), Quaternion.LookRotation(shipMain.transform.forward));
                miss.GetComponent<ProjectileBase>().callRPC(shipMain);
                miss.GetComponent<ProjectileBase>().setTarget(target);
            }

            {
                GameObject miss = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", stats.projectilePrefabMulti.name), gunOrigin.transform.position + new Vector3(stats.burstSpread, -stats.burstSpread), Quaternion.LookRotation(shipMain.transform.forward));
                miss.GetComponent<ProjectileBase>().callRPC(shipMain);
                miss.GetComponent<ProjectileBase>().setTarget(target);
            }

            target = null;
            currUltValue -= currUltValue;
        }
    }
    public override void SingleplayerFire()
    {
        if (currUltValue >= stats.ultChargeTime && !paused)
        {
            RaycastHit hit;
            if (Physics.SphereCast(gunOrigin.transform.position, stats.collisionBoundsRadius, shipMain.transform.forward, out hit, stats.LockOnDistance))
            {
                if (hit.transform.CompareTag(stats.EnemyLockTag))
                {
                    target = hit.transform.gameObject;
                }
            }

            {
                GameObject miss = Instantiate(stats.projectilePrefabSingle, gunOrigin.transform.position + new Vector3(stats.burstSpread, stats.burstSpread), Quaternion.LookRotation(shipMain.transform.forward));
                miss.GetComponent<ProjectileBase>().setShooter(shipMain.getPlayerMain().gameObject);
                miss.GetComponent<ProjectileBase>().setTarget(target);
            }

            {
                GameObject miss = Instantiate(stats.projectilePrefabSingle, gunOrigin.transform.position + new Vector3(-stats.burstSpread, stats.burstSpread), Quaternion.LookRotation(shipMain.transform.forward));
                miss.GetComponent<ProjectileBase>().setShooter(shipMain.getPlayerMain().gameObject);
                miss.GetComponent<ProjectileBase>().setTarget(target);
            }

            {
                GameObject miss = Instantiate(stats.projectilePrefabSingle, gunOrigin.transform.position + new Vector3(-stats.burstSpread, -stats.burstSpread), Quaternion.LookRotation(shipMain.transform.forward));
                miss.GetComponent<ProjectileBase>().setShooter(shipMain.getPlayerMain().gameObject);
                miss.GetComponent<ProjectileBase>().setTarget(target);
            }


            {
                GameObject miss = Instantiate(stats.projectilePrefabSingle, gunOrigin.transform.position + new Vector3(stats.burstSpread, -stats.burstSpread), Quaternion.LookRotation(shipMain.transform.forward));
                miss.GetComponent<ProjectileBase>().setShooter(shipMain.getPlayerMain().gameObject);
                miss.GetComponent<ProjectileBase>().setTarget(target);
            }

            target = null;
            currUltValue -= currUltValue;

        }
    }
}
