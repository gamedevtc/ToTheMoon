using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class DevastatorBeam : WeaponBase
{
    [SerializeField] GameObject target;
    [SerializeField] string targetTag;

    public override void MultiplayerFire()
    {
        if (currUltValue >= stats.ultChargeTime && !paused)
        {
            RaycastHit hit;
            if (Physics.SphereCast(gunOrigin.transform.position, stats.collisionBoundsRadius, shipMain.transform.forward, out hit, stats.maxBeamDistance))
            {
                //add check to make sure missile doesnt target shooter
                if (hit.transform.CompareTag(stats.PlayerLockTag) && hit.transform.gameObject.GetComponent<PhotonView>().ViewID != shipMain.getPlayerMain().GetComponent<PhotonView>().ViewID)
                {
                    target = hit.transform.gameObject;
                    targetTag = hit.transform.gameObject.tag;
                    if(targetTag == stats.PlayerLockTag)
                    {
                        target.GetComponent<Player4Base>().TakeDamage(stats.damageDev, shipMain.getPlayerMain().gameObject);
                    }
                    currUltValue -= currUltValue;
                }
            }
        }
    }
    public override void SingleplayerFire()
    {
        if (currUltValue >= stats.ultChargeTime && !paused)
        {
            RaycastHit hit;
            if (Physics.SphereCast(gunOrigin.transform.position, stats.collisionBoundsRadius, shipMain.transform.forward, out hit, stats.maxBeamDistance))
            {
                //add check to make sure missile doesnt target shooter
                if (hit.transform.CompareTag(stats.EnemyLockTag) || hit.transform.CompareTag(stats.PlayerLockTag) && hit.transform.gameObject != shipMain.getPlayerMain().gameObject)
                {
                    target = hit.transform.gameObject;
                    targetTag = hit.transform.gameObject.tag;

                    if (targetTag == stats.EnemyLockTag)
                    {
                        target.GetComponent<AIUnit>().TakeDamage(stats.damageDev,shipMain.getPlayerMain().gameObject);
                        currUltValue -= currUltValue;
                    }
                }
            }
        }
    }
}
