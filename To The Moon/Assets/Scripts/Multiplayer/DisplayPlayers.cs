using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;


public class DisplayPlayers : MonoBehaviour
{
    //[SerializeField] public PlayerManager[] PlayerList;

    public Text _ui;
    public Text Ping;
    string playerlistz;
    string _ping;
    private void Start()
    {
        //PlayerList = FindObjectsOfType<PlayerManager>();
    }
    void Update()
    {
        //PlayerList = FindObjectsOfType<PlayerManager>();

        //for (int i = 0; i < PlayerList.Length; i++)
        //{
        //    if (PhotonNetwork.PlayerList[i].IsMasterClient)
        //    {
        //        playerlistz = "Host| Name: " + PhotonNetwork.PlayerList[i].NickName + ", K:" + PlayerList[i].getkills() + " D:" + PlayerList[i].getdeaths() + "\n";
        //    }
        //    else
        //    {
        //        playerlistz += "Name: " + PhotonNetwork.PlayerList[i].NickName + ", K:" + PlayerList[i].getkills() + " D:" + PlayerList[i].getdeaths() + "\n";

        //    }
        //}


        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (i == 0)
            {
                playerlistz = "Name: " + PhotonNetwork.PlayerList[i].NickName + ", K:" + PhotonNetwork.PlayerList[i].GetKills() + " D:" + PhotonNetwork.PlayerList[i].GetDeaths() + "\n";
            }
            else
            {
                 playerlistz += "Name: " + PhotonNetwork.PlayerList[i].NickName + ", K:" + PhotonNetwork.PlayerList[i].GetKills() + " D:" + PhotonNetwork.PlayerList[i].GetDeaths() + "\n";

            }
        }
        _ui.text = playerlistz;

        _ping = "Ping: " + PhotonNetwork.GetPing().ToString();
        Ping.text = _ping;

    }
}
