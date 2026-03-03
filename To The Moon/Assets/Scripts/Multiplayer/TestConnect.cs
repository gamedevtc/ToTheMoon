using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class TestConnect : MonoBehaviourPunCallbacks
{
    public string Name = "DefaultName";
    public string GameVersion = "0.5.0";
    public bool SyncLevels = true;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Connecting to server . . .");
        //Sets Name, *Set localy that will bet sent to the server
        PhotonNetwork.NickName = Name;

        // locks users to this version of game. If user has diffrent verson it wont connect
        PhotonNetwork.GameVersion = GameVersion;

        // Connet to server
        PhotonNetwork.ConnectUsingSettings();

        PhotonNetwork.ConnectToRegion("us");

        PhotonNetwork.AutomaticallySyncScene = true;
        Hashtable hash = new Hashtable();
        hash.Add("Skybox", 0);
        hash.Add("Color", 0);
        hash.Add("Body", 0);
        hash.Add("Prim", 0);
        hash.Add("Sec", 0);
        hash.Add("Ult", 0);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected", this);
        // Name that is on the server
        Debug.Log("Name: " + PhotonNetwork.LocalPlayer.NickName);


        if (!PhotonNetwork.InLobby)
             PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected, Reason: " + cause.ToString());

        // DisconnectCause. = all possable reasons
    }

}
