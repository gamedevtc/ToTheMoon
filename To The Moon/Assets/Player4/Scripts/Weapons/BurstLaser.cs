using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class BurstLaser : WeaponBase
{
    float tempTime;
    public override void MultiplayerFire()
    {
        if (rateOfFire >= stats.fireCoolDown && !paused)
        {
            {
                GameObject bull = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", stats.projectilePrefabMulti.name), gunOrigin.transform.position + new Vector3(0.0f, stats.burstSpread, 0.0f), 
                    Quaternion.LookRotation(shipMain.transform.forward + new Vector3(0.0f, stats.burstRotation, 0.0f)));
                bull.GetComponent<ProjectileBase>().callRPC(shipMain);

                GameObject bull2 = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", stats.projectilePrefabMulti.name), gunOrigin.transform.position + new Vector3(stats.burstSpread, 0.0f, 0.0f), 
                    Quaternion.LookRotation(shipMain.transform.forward + new Vector3(stats.burstRotation, 0.0f, 0.0f)));
                bull2.GetComponent<ProjectileBase>().callRPC(shipMain);
            }
                
            {
                GameObject bull = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", stats.projectilePrefabMulti.name), gunOrigin.transform.position + new Vector3(0.0f, -stats.burstSpread, 0.0f), 
                    Quaternion.LookRotation(shipMain.transform.forward + new Vector3(0.0f, -stats.burstRotation, 0.0f)));
                bull.GetComponent<ProjectileBase>().callRPC(shipMain);

                GameObject bull2 = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", stats.projectilePrefabMulti.name), gunOrigin.transform.position + new Vector3(-stats.burstSpread, 0.0f, 0.0f), 
                    Quaternion.LookRotation(shipMain.transform.forward + new Vector3(-stats.burstRotation, 0.0f, 0.0f)));
                bull2.GetComponent<ProjectileBase>().callRPC(shipMain);
            }
            
            rateOfFire -= rateOfFire;
            
        }
    }
    public override void SingleplayerFire()
    {
        if (rateOfFire >= stats.fireCoolDown && !paused)
        {
            {
                GameObject bull = Instantiate(stats.projectilePrefabSingle, gunOrigin.transform.position + new Vector3(0.0f, stats.burstSpread, 0.0f), 
                    Quaternion.LookRotation(shipMain.transform.forward + new Vector3(0.0f, stats.burstRotation, 0.0f)));
                bull.GetComponent<ProjectileBase>().setShooter(shipMain.getPlayerMain().gameObject);

                GameObject bull2 = Instantiate(stats.projectilePrefabSingle, gunOrigin.transform.position + new Vector3(stats.burstSpread, 0.0f, 0.0f),
                    Quaternion.LookRotation(shipMain.transform.forward + new Vector3(stats.burstRotation, 0.0f, 0.0f)));
                bull2.GetComponent<ProjectileBase>().setShooter(shipMain.getPlayerMain().gameObject);
            }
            
            {
                GameObject bull = Instantiate(stats.projectilePrefabSingle, gunOrigin.transform.position + new Vector3(0.0f, -stats.burstSpread, 0.0f), 
                    Quaternion.LookRotation(shipMain.transform.forward + new Vector3(0.0f, -stats.burstRotation, 0.0f)));
                bull.GetComponent<ProjectileBase>().setShooter(shipMain.getPlayerMain().gameObject);

                GameObject bull2 = Instantiate(stats.projectilePrefabSingle, gunOrigin.transform.position + new Vector3(-stats.burstSpread, 0.0f, 0.0f),
                    Quaternion.LookRotation(shipMain.transform.forward + new Vector3(-stats.burstRotation, 0.0f, 0.0f)));
                bull2.GetComponent<ProjectileBase>().setShooter(shipMain.getPlayerMain().gameObject);
            }
            
            rateOfFire -= rateOfFire;
        }
    }
}
