using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class FlareShotWeap : WeaponBase
{
    public override void MultiplayerFire()
    {
        if (rateOfFire >= stats.fireCoolDown && !paused)
        {
            GameObject flare = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", stats.projectilePrefabMulti.name), gunOrigin.transform.position, 
                                                                        Quaternion.LookRotation(shipMain.transform.forward));
            //flare.GetComponent<ProjectileBase>().callRPC(shipMain);
            rateOfFire -= rateOfFire;
        }
    }

    public override void SingleplayerFire()
    {
        //if (rateOfFire >= stats.fireCoolDown && !paused)
        //{
        //    GameObject bull = Instantiate(stats.projectilePrefabSingle, gunOrigin.transform.position, Quaternion.LookRotation(shipMain.transform.forward));
        //    bull.GetComponent<ProjectileBase>().setShooter(shipMain.getPlayerMain().gameObject);
        //    rateOfFire -= rateOfFire;
        //}
    }
}
