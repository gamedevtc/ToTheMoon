using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using System.IO;
using UnityEngine;

public class SingleShotWeapon : WeaponBase
{
    public override void MultiplayerFire()
    {
        if (rateOfFire >= stats.fireCoolDown && !paused)
        {
          
            GameObject bull = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", stats.projectilePrefabMulti.name), gunOrigin.transform.position, 
                                                            Quaternion.LookRotation(shipMain.transform.forward), 0, new object[] { shipMain.PV.ViewID });
            bull.GetComponent<ProjectileBase>().callRPC(shipMain);
            rateOfFire -= rateOfFire;
            
        }
    }
    public override void SingleplayerFire()
    {
        if (rateOfFire >= stats.fireCoolDown && !paused)
        {
            GameObject bull = Instantiate(stats.projectilePrefabSingle, gunOrigin.transform.position, Quaternion.LookRotation(shipMain.transform.forward));
            bull.GetComponent<ProjectileBase>().setShooter(shipMain.getPlayerMain().gameObject);
            rateOfFire -= rateOfFire;
        }
    }
}
