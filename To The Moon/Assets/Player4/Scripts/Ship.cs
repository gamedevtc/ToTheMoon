using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;
using Photon.Realtime;
using Unity.IO.LowLevel.Unsafe;

public class Ship : MonoBehaviour
{
    #region Enums
    public enum ColorOption
    {
        Red = 0,
        DarkBlue,
        LightBlue,
        Green,
        Orange,
        Purple,
        Black,
        Yellow,
        COUNT
    }
    public enum BodyOption //-(ShipColor.COUNT)
    {
        Body1 = (ColorOption.COUNT),//8
        Body2,//9
        Body3,//10
        Body4,//11
        COUNT
    }
    public enum PrimaryOption //-(Body.COUNT)
    {
        FusionBlaster = (BodyOption.COUNT),//12
        FusionBlasterMkII,//13
        PlasmaAccelerator,//14
        BurstLaser,//15
        COUNT
    }
    public enum SecondaryOption //-(Primary.COUNT)
    {
        Missiles = (PrimaryOption.COUNT),//16
        ThermalMissiles,//17
        PositronBallista,//18
        PulseCannon,//19
        FusionBlasterHeavy,//20
        FusionBlasterMkIIHeavy,//21
        PlasmaAcceleratorHeavy,//22
        BurstLaserHeavy,//23
        COUNT
    }
    public enum UltimateOption //-(Secondary.COUNT)
    {
        MineLauncher = (SecondaryOption.COUNT),//24
        MissileSilo,//25
        DevastatorBeam,//26
        EMP,//27
        COUNT
    }
    #endregion

    public struct playerShipSettings
    {
        public ColorOption activeColor;
        public BodyOption activeBody;
        public PrimaryOption activePrimary;
        public SecondaryOption activeSecondary;
        public UltimateOption activeUltimate;
    }

    [SerializeField] public SaveData saveScript;

    [Header("Set In Prefab")]
    [SerializeField] protected BodyLink bodyLink;
    [SerializeField] protected WeaponLink PrimaryLink;
    [SerializeField] protected WeaponLink SecondaryLink;
    [SerializeField] protected WeaponLink UltimateLink;
    [SerializeField] protected WeaponLink FlareLink;
    [SerializeField] protected List<Material> shipColors;
    [SerializeField] protected Material holoMat;
    [SerializeField] protected bool dummyShip;
    [SerializeField] protected Player4Base playerMain;
    [SerializeField] public PhotonView PV;

    public playerShipSettings activeSettings;

    bool once = true;

    protected virtual void Awake()
    {
        saveScript = GetComponent<SaveData>();
        PV = GetComponentInParent<PhotonView>();
    }

    protected virtual void Start()
    {
        activeSettings = new playerShipSettings();
        if (dummyShip)
        {
            //Load settings from SaveData
            saveScript.getActiveSettings(out activeSettings);
            activateGameParts(activeSettings);
        }
        else
        {
            if (GameManagerBase.Instance.isMulti())
            {
                if (PV.IsMine)
                {
                    RPC_getPhotonPlayerData(PhotonNetwork.LocalPlayer);
                    RPC_activateGameParts();
                }
                else
                {

                    for (int i = 0; i < PhotonNetwork.PlayerListOthers.Length; i++)
                    {
                        if (PV.Controller == PhotonNetwork.PlayerListOthers[i])
                        {
                            RPC_getPhotonPlayerData(PhotonNetwork.PlayerListOthers[i]);
                            RPC_activateGameParts();
                        }
                    }
                }
               // PV.RPC("RPC_activateGameParts", RpcTarget.All);
            }
            else
            {
                //Load settings from SaveData
                saveScript.getActiveSettings(out activeSettings);
                activateGameParts(activeSettings);
            }
        }
    }


  

  //  [PunRPC]
    public void RPC_activateGameParts()
    {
        playerShipSettings set = activeSettings;
        
        bodyLink.setActiveBody(set.activeBody);
        bodyLink.setColor(shipColors[(int)set.activeColor]);
        PrimaryLink.setActiveWeapons((int)set.activePrimary);
        SecondaryLink.setActiveWeapons((int)set.activeSecondary);
        UltimateLink.setActiveWeapons((int)set.activeUltimate);
    }

    //[PunRPC]
    public void RPC_getPhotonPlayerData(Player play)
    {
        Hashtable hash = play.CustomProperties;
      
        activeSettings.activeColor = (ColorOption)hash["Color"];
        activeSettings.activeBody = (BodyOption)hash["Body"];
        activeSettings.activePrimary = (PrimaryOption)hash["Prim"];
        activeSettings.activeSecondary = (SecondaryOption)hash["Sec"];
        activeSettings.activeUltimate = (UltimateOption)hash["Ult"];
       
    }

    public void activateGameParts(playerShipSettings set)
    {
        bodyLink.setActiveBody(set.activeBody);
        bodyLink.setColor(shipColors[(int)set.activeColor]);
        PrimaryLink.setActiveWeapons((int)set.activePrimary);
        PrimaryLink.setColor(shipColors[(int)set.activeColor]);
        SecondaryLink.setActiveWeapons((int)set.activeSecondary);
        SecondaryLink.setColor(shipColors[(int)set.activeColor]);
        UltimateLink.setActiveWeapons((int)set.activeUltimate);
        UltimateLink.setColor(shipColors[(int)set.activeColor]);
    }

    public void firePrimary()
    {
        PrimaryLink.Fire();
    }

    public void fireSecondary()
    {
        SecondaryLink.Fire();
    }

    public void fireUltimate()
    {
        UltimateLink.Fire();
    }

    public void fireFlare()
    {
        //FlareLink.Fire();
    }

    public void fillUlt()
    {
        UltimateLink.fillUlt();
    }

    public BodyStats stats()
    {
        return bodyLink.getStats();
    }

    public Mesh mesh()
    {
        return bodyLink.getMesh();
    }

    public Material getActiveColor()
    {
        return shipColors[(int)activeSettings.activeColor];
    }
    public void setCredits(int num)
    {
        saveScript.SetCurrency(num);
    }
    public void saveData()
    {
        saveScript.saveData();
    }

    public float getCurrentUltValue()
    {
        return UltimateLink.getCurrentUltValue();
    }

    public float getUltChargeTime()
    {
        return UltimateLink.getUltChargeTime();
    }

    public Player4Base getPlayerMain()
    {
        return playerMain;
    }
}
