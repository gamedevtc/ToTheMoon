using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateJoinCanvas : MonoBehaviour
{
    [SerializeField]
    private CreateLobbyMenu _createLobbyMenu;
    [SerializeField]
    private LobbyListingMenu _lobbylistmenu;

    private RoomsCanvas _roomsCanvas;

    public void FirstInitialize(RoomsCanvas canvases)
    {
        _roomsCanvas = canvases;
        _createLobbyMenu.FirstInitialize(canvases);
        _lobbylistmenu.FirstInitialize(canvases);


    }



}
