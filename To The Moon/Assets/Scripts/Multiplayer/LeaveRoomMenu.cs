using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveRoomMenu : MonoBehaviour
{

    private RoomsCanvas _roomscanvas;

    public void FirstInitalize(RoomsCanvas canvas)
    {
        _roomscanvas = canvas;
    }


    public void OnClick_LeaveRoom()
    {
        PhotonNetwork.LeaveRoom(true);
        _roomscanvas.CurrentRoomCanvus.Hide();
    }



}
