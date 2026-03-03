using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;





public class CreateLobbyMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Text _lobbyName;


    private RoomsCanvas _roomsCanvas;

    public void FirstInitialize(RoomsCanvas canvases)
    {
        _roomsCanvas = canvases;
    }


    public void OnClick_CreateRoom()
    {
        //Makesure your connected to the server b4 makeing room
        if (!PhotonNetwork.IsConnected)
        {
            return;
        }


        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 10;

        
        //JoinOrCreateRoom - Makes room, if it exists, you will join it
        PhotonNetwork.JoinOrCreateRoom(_lobbyName.text, options, TypedLobby.Default);

        


    }


    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room", this);
        _roomsCanvas.CurrentRoomCanvus.Show();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room Not Created, Reason: " + message, this);
    }

}
