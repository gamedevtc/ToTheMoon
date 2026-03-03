using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class Ultimate : WeaponBase
{
    public override void MultiplayerFire()
    {
        if (currUltValue >= stats.ultChargeTime && !paused)
        {
            GameObject bom = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", stats.projectilePrefabMulti.name), gunOrigin.transform.position, Quaternion.LookRotation(shipMain.transform.forward));
            bom.GetComponent<ProjectileBase>().callRPC(shipMain);
            currUltValue -= currUltValue;
        }
     }
    public override void SingleplayerFire()
    {
        if (currUltValue >= stats.ultChargeTime && !paused)
        {
            GameObject bom = Instantiate(stats.projectilePrefabSingle, gunOrigin.transform.position, Quaternion.LookRotation(shipMain.transform.forward));
            bom.GetComponent<ProjectileBase>().setShooter(shipMain.getPlayerMain().gameObject);
            currUltValue -= currUltValue;
        }
    }
}
