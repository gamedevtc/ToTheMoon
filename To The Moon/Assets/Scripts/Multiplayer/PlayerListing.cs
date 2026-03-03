using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerListing : MonoBehaviour
{
    [SerializeField] private Text _text;
    [SerializeField] Image hostCrown;
    [SerializeField] Toggle readyUpCheck;
    [SerializeField] public bool ready;

    public Player Player { get; private set; }

    public void SetPlayerInfo(Player player)
    {
        Player = player;
        _text.text = player.NickName;
        if (player.IsMasterClient)
        {
            hostCrown.gameObject.SetActive(true);
        }
        else
        {
            hostCrown.gameObject.SetActive(false);
        }
        if (player == PhotonNetwork.LocalPlayer)
        {
            readyUpCheck.interactable = true;
        }
        else
        {
            readyUpCheck.interactable = false;
        }
        ready = false;
        readyUpCheck.isOn = false;
    }

    public void updateReady(bool io)
    {
        ready = io;
        readyUpCheck.isOn = io;
    }
    public void resetReady()
    {
        if (ready == true)
        {
            ready = false;
            readyUpCheck.isOn = false;
            updateProperties();
        }
    }

    public void updateName(string newName)
    {
        _text.text = newName;
    }

    public void updateProperties()
    {
        Hashtable hash = new Hashtable();
        if (ready)
        {
            hash.Add("R", 1);
        }
        else
        {
            hash.Add("R", 0);
        }
        Player.SetCustomProperties(hash);
    }

    public string getCurrentName()
    {
        return _text.text;
    }

    public void OnToggleUpdateReady()
    {
        ready = readyUpCheck.isOn;
        updateProperties();
    }
}
