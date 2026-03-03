using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerListingMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Transform _content;
    [SerializeField]
    private PlayerListing _PlayerList;

    private List<PlayerListing> _listing = new List<PlayerListing>();

    private RoomsCanvas _rooms;



    private void GetCurrentRoomPlayers()
    {
        foreach (KeyValuePair<int,Player> playerinfo in PhotonNetwork.CurrentRoom.Players)
        {
            AddplayerListing(playerinfo.Value);
        } 
    }

    public void FirstInitialize(RoomsCanvas canvas)
    {
        _rooms = canvas;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        GetCurrentRoomPlayers();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        for (int i = 0; i < _listing.Count; i++)
        {
            Destroy(_listing[i].gameObject);

        }
        _listing.Clear();
    }



    private void AddplayerListing(Player player)
    {
        int index = _listing.FindIndex(x => x.Player == player);

        if (index != -1)
        {
            _listing[index].SetPlayerInfo(player);
        }
        else
        {
            PlayerListing listing = Instantiate(_PlayerList, _content);
            if (listing != null)
            {
                listing.SetPlayerInfo(player);
                _listing.Add(listing);
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddplayerListing(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        int index = _listing.FindIndex(x => x.Player == otherPlayer);
        if (index != -1)
        {
            Destroy(_listing[index].gameObject);
            _listing.RemoveAt(index);
        }
    }

   
}
