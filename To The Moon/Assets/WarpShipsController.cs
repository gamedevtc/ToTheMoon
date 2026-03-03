using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class WarpShipsController : MonoBehaviour
{
    [SerializeField] WarpShip mainShip;
    [SerializeField] List<WarpShip> otherShips;

    // Start is called before the first frame update
    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            return;
        }

        mainShip.setShip(getSettings(PhotonNetwork.LocalPlayer));

        for (int i = 0; i < otherShips.Count; i++)
        {
            if (i >= PhotonNetwork.PlayerList.Count())
            {
                otherShips[i].gameObject.SetActive(false);
            }
            else
            {
                if (!PhotonNetwork.PlayerList[i].IsLocal)
                {
                    otherShips[i].setShip(getSettings(PhotonNetwork.PlayerList[i]));
                }
            }
        }
    }

    Ship.playerShipSettings getSettings(Player p)
    {
        Ship.playerShipSettings set = new Ship.playerShipSettings();

        set.activeColor = (Ship.ColorOption)p.CustomProperties["Color"];
        set.activeBody = (Ship.BodyOption)p.CustomProperties["Body"];
        set.activePrimary = (Ship.PrimaryOption)p.CustomProperties["Prim"];
        set.activeSecondary = (Ship.SecondaryOption)p.CustomProperties["Sec"];
        set.activeUltimate = (Ship.UltimateOption)p.CustomProperties["Ult"];

        return set;
    }
}
