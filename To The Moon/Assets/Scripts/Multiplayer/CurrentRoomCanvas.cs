using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentRoomCanvas : MonoBehaviour
{

    [SerializeField]
    private PlayerListingMenu _playerListingMenu;
    [SerializeField]
    private LeaveRoomMenu _leaveRoom;


    private RoomsCanvas _roomsCanvas;

    public void FirstInitialize(RoomsCanvas canvases)
    {
        _roomsCanvas = canvases;
        _playerListingMenu.FirstInitialize(canvases);
        _leaveRoom.FirstInitalize(canvases);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }


}
