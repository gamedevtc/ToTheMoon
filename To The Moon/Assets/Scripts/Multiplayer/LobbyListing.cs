using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LobbyListing : MonoBehaviour
{

    [SerializeField]
    private Text _text;


    public RoomInfo Roominfo { get; private set; }

    public void SetRoomInfo(RoomInfo roomInfo)
    {
        Roominfo = roomInfo;
        _text.text = roomInfo.Name + ", Size: " + roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers; 
    }
    // ftt, Size: 1/16Room: 'ftt' visible,open 1/16 players.

    public void OnClick_Button()
    {
        PhotonNetwork.JoinRoom(Roominfo.Name);
    }


}
