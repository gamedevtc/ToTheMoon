using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LobbyListingMenu : MonoBehaviourPunCallbacks
{

    [SerializeField]
    private Transform _content;
    [SerializeField]
    private LobbyListing _lobbyList;

    private RoomsCanvas _roomsCanvas;

    private List<LobbyListing> _listing = new List<LobbyListing>();


    [SerializeField]
    private GameObject StartButton;
    public void FirstInitialize(RoomsCanvas canvas)
    {
        _roomsCanvas = canvas;
    }


    public override void OnJoinedRoom()
    {
        _roomsCanvas.CurrentRoomCanvus.Show();
        _content.DestroyChildren();
        _listing.Clear();
        StartButton.SetActive(PhotonNetwork.IsMasterClient);
    }


    public override void OnRoomListUpdate(List<RoomInfo> lobbyList)
    {
        foreach (RoomInfo item in lobbyList)
        {

            if (item.RemovedFromList) //removes from lobby list
            {
                int index = _listing.FindIndex(x => x.Roominfo.Name == item.Name);
                if (index != -1)
                {
                    Destroy(_listing[index].gameObject);
                    _listing.RemoveAt(index);
                }
            }
            else // adds to the lobby list
            {
                int index = _listing.FindIndex(x => x.Roominfo.Name == item.Name);
                if (index == -1)
                {
                    LobbyListing listing = Instantiate(_lobbyList, _content);
                    if (listing != null)
                    {
                        listing.SetRoomInfo(item);
                        _listing.Add(listing);
                    }

                }
             

            }


        }



    }




}
