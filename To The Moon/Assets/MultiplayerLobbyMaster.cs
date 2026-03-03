using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun.Simple;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;
using WebSocketSharp;

public class MultiplayerLobbyMaster : MonoBehaviourPunCallbacks
{
    public static MultiplayerLobbyMaster Instance;

    [SerializeField] PhotonView PV;
    [SerializeField] string GameVersion = "0.5.0";
    [SerializeField] bool AutomaticallySyncScenePref = true;
    [SerializeField] string Region = "us";
    [SerializeField] bool debug_ShowPlayerCustomProperties = false;

    [Header("Canvases and Objects")]
    [SerializeField] GameObject CreateOrJoinCanvas;
    [SerializeField] GameObject RoomCanvas;
    [SerializeField] GameObject HostRoomBox;
    [SerializeField] GameObject StatCanvas;
    [SerializeField] GameObject ModifyCanvas;
    [SerializeField] GameObject LocalPlayerCanvas;
    [SerializeField] GameObject PlayerCanvasStatButton;
    [SerializeField] GameObject PlayerCanvasReturnButton;
    [SerializeField] PlayerNameSave PlayerNameSaveScript;
    
    [SerializeField] MLoad loadSync;

    //[SerializeField] GameObject RoomSettingsMenu;

    [Header("Deactivated While Connecting")]
    [SerializeField] TMP_InputField PlayerNameInputField;
    [SerializeField] Button ModifyShipButton;
    [SerializeField] Button StartGameButton;

    [Header("Create Or Join Menu Objects")]
    [SerializeField] TMP_Text ConnectingText;
    [SerializeField] Button CreateRoomButton;
    [SerializeField] Slider CreateRoomPlayerCountSlider;
    [SerializeField] TMP_Text CreateRoomPlayerCountNumber;
    [SerializeField] Transform RoomList;
    [SerializeField] GameObject RoomListingPrefab;
    [SerializeField] List<LobbyListing> ListOfRooms = new List<LobbyListing>();
    [SerializeField] TMP_InputField CreateRoomField;

    [Header("In Room Menu Objects")]
    [SerializeField] string defaultHangarTitle = "'s Hangar";
    [SerializeField] string currHangarTitle;
    [SerializeField] TMP_Text HangarTitleText;
    [SerializeField] TMP_Text PlayerCountText;
    [SerializeField] Transform PlayerListTransform;
    [SerializeField] GameObject PlayerListingPrefab;
    [SerializeField] List<PlayerListing> ListOfPlayers = new List<PlayerListing>();

    [Header("Cameras")]
    [SerializeField] GameObject MainCamera;
    [SerializeField] GameObject ShopCamera;

    [Header("Create Room Settings")]
    [SerializeField] byte MaxPlayers = 16;

    [SerializeField] ShopAttachment dummyShip;

    public enum MultiplayerMenuState
    {
        Connecting = 0,
        CreateOrJoin,
        Room,
        Modify,
        Stats,
    }

    public MultiplayerMenuState currState;
    public MultiplayerMenuState prevState;
    public bool stateChanged;
    public bool allPlayersReady = false;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        PV = GetComponent<PhotonView>();
    }

    public void Start()
    {
        Debug.Log("Connecting to server . . .");
        PhotonNetwork.GameVersion = GameVersion;

        PhotonNetwork.ConnectUsingSettings();

        PhotonNetwork.ConnectToRegion(Region);

        PhotonNetwork.AutomaticallySyncScene = AutomaticallySyncScenePref;
        Hashtable hash = new Hashtable();
        hash.Add("Color", 0);
        hash.Add("Body", 0);
        hash.Add("Prim", 0);
        hash.Add("Sec", 0);
        hash.Add("Ult", 0);
        hash.Add("R", 0);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        currState = MultiplayerMenuState.Connecting;
        stateChanged = true;
        defaultHangarTitle = PhotonNetwork.LocalPlayer.NickName + defaultHangarTitle;
        currHangarTitle = defaultHangarTitle;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected", this);
        // Name that is on the server
        Debug.Log("Name: " + PhotonNetwork.LocalPlayer.NickName);

        if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby();
        switchState(MultiplayerMenuState.CreateOrJoin);
    }

    private void Update()
    {
        if (stateChanged)
        {
            if (prevState == MultiplayerMenuState.CreateOrJoin && currState != MultiplayerMenuState.CreateOrJoin)
            {
                RoomList.DestroyChildren();
                ListOfRooms.Clear();
            }
            switch (currState)
            {
                case MultiplayerMenuState.Connecting:
                    CreateRoomPlayerCountSlider.value = MaxPlayers;
                    CreateRoomPlayerCountNumber.text = CreateRoomPlayerCountSlider.value.ToString();

                    CreateRoomPlayerCountSlider.interactable = false;
                    PlayerNameInputField.interactable = false;
                    CreateRoomField.interactable = false;
                    ModifyShipButton.interactable = false;
                    StartGameButton.interactable = false;

                    LocalPlayerCanvas.SetActive(true);
                    PlayerCanvasStatButton.SetActive(true);
                    PlayerCanvasReturnButton.SetActive(false);
                    ConnectingText.gameObject.SetActive(true);
                    CreateRoomButton.interactable = false;
                    CreateOrJoinCanvas.SetActive(true);
                    RoomCanvas.SetActive(false);
                    StatCanvas.SetActive(false);
                    ModifyCanvas.SetActive(false);
                    MainCamera.SetActive(true);
                    ShopCamera.SetActive(false);
                    ModifyShipButton.gameObject.SetActive(true);
                    CreateRoomField.text = currHangarTitle;
                    break;
                case MultiplayerMenuState.CreateOrJoin:
                    CreateRoomPlayerCountSlider.value = MaxPlayers;
                    CreateRoomPlayerCountNumber.text = CreateRoomPlayerCountSlider.value.ToString();

                    CreateRoomPlayerCountSlider.interactable = true;
                    PlayerNameInputField.interactable = true;
                    CreateRoomField.interactable = true;
                    ModifyShipButton.interactable = true;
                    StartGameButton.interactable = true;

                    LocalPlayerCanvas.SetActive(true);
                    PlayerCanvasStatButton.SetActive(true);
                    PlayerCanvasReturnButton.SetActive(false);
                    ConnectingText.gameObject.SetActive(false);
                    CreateRoomButton.interactable = true;
                    CreateOrJoinCanvas.SetActive(true);
                    RoomCanvas.SetActive(false);
                    StatCanvas.SetActive(false);
                    ModifyCanvas.SetActive(false);
                    MainCamera.SetActive(true);
                    ShopCamera.SetActive(false);
                    ModifyShipButton.gameObject.SetActive(true);
                    CreateRoomField.text = currHangarTitle;
                    break;
                case MultiplayerMenuState.Room:
                    PlayerCountText.text = PhotonNetwork.PlayerList.Length.ToString() + "/" + PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
                    LocalPlayerCanvas.SetActive(true);
                    PlayerCanvasStatButton.SetActive(true);
                    PlayerCanvasReturnButton.SetActive(false);
                    CreateOrJoinCanvas.SetActive(false);
                    RoomCanvas.SetActive(true);
                    StatCanvas.SetActive(false);
                    ModifyCanvas.SetActive(false);
                    MainCamera.SetActive(true);
                    ShopCamera.SetActive(false);
                    HostRoomBox.SetActive(PhotonNetwork.IsMasterClient);
                    HangarTitleText.text = PhotonNetwork.CurrentRoom.Name;
                    ModifyShipButton.gameObject.SetActive(true);
                    break;
                case MultiplayerMenuState.Modify:
                    LocalPlayerCanvas.SetActive(false);
                    PlayerCanvasStatButton.SetActive(false);
                    PlayerCanvasReturnButton.SetActive(false);
                    CreateOrJoinCanvas.SetActive(false);
                    RoomCanvas.SetActive(false);
                    StatCanvas.SetActive(false);
                    ModifyCanvas.SetActive(true);
                    MainCamera.SetActive(false);
                    ShopCamera.SetActive(true);
                    break;
                case MultiplayerMenuState.Stats:
                    CreateOrJoinCanvas.SetActive(false);
                    RoomCanvas.SetActive(false);
                    LocalPlayerCanvas.SetActive(true);
                    PlayerCanvasStatButton.SetActive(false);
                    PlayerCanvasReturnButton.SetActive(true);
                    StatCanvas.SetActive(true);
                    ModifyCanvas.SetActive(false);
                    ModifyShipButton.gameObject.SetActive(false);
                    MainCamera.SetActive(false);
                    ShopCamera.SetActive(true);
                    break;
                default:
                    break;
            }
            stateChanged = false;
        }

        //if (debug_ShowPlayerCustomProperties)
        //{
        //    foreach (Player player in PhotonNetwork.PlayerList)
        //    {
        //        Debug.Log("PlayerCustomProps - " + player.NickName + " - 'R' = " + player.CustomProperties["R"]);
        //        foreach (PlayerListing item in ListOfPlayers)
        //        {
        //            Debug.Log("PlayerCustomProps - " + item.Player.NickName + " - 'R' = " + item.Player.CustomProperties["R"]);
        //            if (item.Player == player)
        //            {
        //                if ((int)player.CustomProperties["R"] == 1)
        //                {
        //                    item.updateReady(true);
        //                }
        //                else
        //                {
        //                    item.updateReady(false);
        //                }
        //            }
        //        }
        //    }
        //}

        if (currState == MultiplayerMenuState.Room)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (allPlayersReady)
                {
                    StartGameButton.interactable = true;
                }
                else
                {
                    StartGameButton.interactable = false;
                }
            }
        }
    }

    public void switchState(MultiplayerMenuState newState)
    {
        prevState = currState;
        currState = newState;
        stateChanged = true;
    }


    #region PunOverrides
    public override void OnRoomListUpdate(List<RoomInfo> lobbyList)
    {
        if (currState == MultiplayerMenuState.CreateOrJoin)
        {
            foreach (RoomInfo item in lobbyList)
            {
                if (item.RemovedFromList) //removes from lobby list
                {
                    int index = ListOfRooms.FindIndex(x => x.Roominfo.Name == item.Name);
                    if (index != -1)
                    {
                        Destroy(ListOfRooms[index].gameObject);
                        ListOfRooms.RemoveAt(index);
                    }
                }
                else // adds to the lobby list
                {
                    int index = ListOfRooms.FindIndex(x => x.Roominfo.Name == item.Name);
                    if (index == -1)
                    {
                        LobbyListing listing = Instantiate(RoomListingPrefab, RoomList).GetComponent<LobbyListing>();
                        if (listing != null)
                        {
                            listing.SetRoomInfo(item);
                            ListOfRooms.Add(listing);
                        }
                    }
                }
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        foreach (PlayerListing item in ListOfPlayers)
        {
            if (item.Player == newPlayer)
            {
                ListOfPlayers.Remove(item);
                Destroy(item.gameObject);
            }
        }
        PlayerListing newP = Instantiate(PlayerListingPrefab, PlayerListTransform).GetComponent<PlayerListing>();
        ListOfPlayers.Add(newP);
        newP.SetPlayerInfo(newPlayer);
        newP.updateReady(false);
        PlayerCountText.text = PhotonNetwork.PlayerList.Length.ToString() + "/" + PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        foreach(PlayerListing item in ListOfPlayers)
        {
            if (item.Player == otherPlayer)
            {
                ListOfPlayers.Remove(item);
                Destroy(item.gameObject);
            }
        }
        PlayerCountText.text = PhotonNetwork.PlayerList.Length.ToString() + "/" + PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
    }

    public override void OnJoinedRoom()
    {
        switchState(MultiplayerMenuState.Room);

        foreach (Player item in PhotonNetwork.PlayerList)
        {
            foreach(PlayerListing listing in ListOfPlayers)
            {
                if (listing.Player == item)
                {
                    ListOfPlayers.Remove(listing);
                    Destroy(listing.gameObject);
                }
            }
            PlayerListing newP = Instantiate(PlayerListingPrefab, PlayerListTransform).GetComponent<PlayerListing>();
            ListOfPlayers.Add(newP);
            newP.SetPlayerInfo(item);
            newP.updateReady(false);
        }
        PlayerCountText.text = PhotonNetwork.PlayerList.Length.ToString() + "/" + PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("R"))
        {
            foreach (PlayerListing item in ListOfPlayers)
            {
                if (item.Player == targetPlayer)
                {
                    if ((int)changedProps["R"] == 1)
                    {
                        item.updateReady(true);
                    }
                    else if ((int)changedProps["R"] == 0)
                    {
                        item.updateReady(false);
                    }
                }
            }
        }
        PV.RPC("RPC_CheckIfReady", RpcTarget.All);
    }

    [PunRPC]
    void RPC_CheckIfReady()
    {
        bool foundNotReady = false;
        foreach (PlayerListing item in ListOfPlayers)
        {
            foreach(Player p in PhotonNetwork.PlayerList)
            {
                if (item.Player == p)
                {
                    if ((int)p.CustomProperties["R"] == 1)
                    {
                        item.updateReady(true);
                    }
                    else if ((int)p.CustomProperties["R"] == 0)
                    {
                        item.updateReady(false);
                    }
                }
            }
            if (!item.ready)
            {
                foundNotReady = true;
            }
        }
        if (foundNotReady)
        {
            allPlayersReady = false;
        }
        else
        {
            allPlayersReady = true;
        }
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room ", this);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room Not Created, Reason: " + message, this);
    }

    public override void OnLeftRoom()
    {
        Debug.Log(PhotonNetwork.NickName + " Has Left The Hangar.");
        switchState(MultiplayerMenuState.CreateOrJoin);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected, Reason: " + cause.ToString());
    }

    private void GetCurrentRoomPlayers()
    {
        if (PhotonNetwork.InRoom)
        {
            foreach (KeyValuePair<int, Player> playerinfo in PhotonNetwork.CurrentRoom.Players)
            {
                AddplayerListing(playerinfo.Value);
            }
        }
    }

    private void AddplayerListing(Player player)
    {
        int index = ListOfPlayers.FindIndex(x => x.Player == player);

        if (index != -1)
        {
            ListOfPlayers[index].SetPlayerInfo(player);
        }
        else
        {
            PlayerListing listing = Instantiate(PlayerListingPrefab, PlayerListTransform).GetComponent<PlayerListing>();
            if (listing != null)
            {
                listing.SetPlayerInfo(player);
                ListOfPlayers.Add(listing);
            }
        }
    }

    public void EnableLink()
    {
        GetCurrentRoomPlayers();
    }

    public void DisableLink()
    {
        for (int i = 0; i < ListOfPlayers.Count; i++)
        {
            Destroy(ListOfPlayers[i].gameObject);
        }
        ListOfPlayers.Clear();
    }

    #endregion

    #region OnClicks/Edits
    public void OnValueChanged_PlayerCount()
    {
        CreateRoomPlayerCountNumber.text = CreateRoomPlayerCountSlider.value.ToString();
    }
    public void OnEndEdit_PlayerName()
    {
        if (currState == MultiplayerMenuState.CreateOrJoin || currState == MultiplayerMenuState.Stats)
        {
            if (CreateRoomField.text == defaultHangarTitle)
            {
                defaultHangarTitle = PhotonNetwork.LocalPlayer.NickName + "'s Hangar";
                currHangarTitle = defaultHangarTitle;
                CreateRoomField.text = currHangarTitle;
            }
        }
        else if (currState == MultiplayerMenuState.Room || currState == MultiplayerMenuState.Stats)
        {
            foreach (PlayerListing item in ListOfPlayers)
            {
                if (item.Player == PhotonNetwork.LocalPlayer)
                {
                    if (item.getCurrentName() != item.Player.NickName)
                    {
                        item.updateName(PhotonNetwork.LocalPlayer.NickName);
                        PV.RPC("RPC_UpdateName", RpcTarget.All);
                    }
                }
            }
        }
    }

    [PunRPC]
    void RPC_UpdateName()
    {
        foreach (PlayerListing item in ListOfPlayers)
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (item.Player == player)
                {
                    if (item.getCurrentName() != player.NickName)
                    {
                        item.updateName(player.NickName);
                    }
                }
            }
        }
    }

    public void OnEndEdit_HangarName()
    {
        currHangarTitle = CreateRoomField.text;
    }

    public void OnClick_CreateRoom()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Create Room Failed, not connected to Photon Networking.");
            return;
        }
        if (CreateRoomField.text.IsNullOrEmpty())
        {
            CreateRoomField.text = defaultHangarTitle;
        }
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)CreateRoomPlayerCountSlider.value;
        PhotonNetwork.JoinOrCreateRoom(CreateRoomField.text, options, TypedLobby.Default);
    }

    public void OnClick_StatsButton()
    {
        switchState(MultiplayerMenuState.Stats);
    }

    public void OnClick_StatsReturnButton()
    {
        if (!PhotonNetwork.InRoom)
        {
            switchState(MultiplayerMenuState.CreateOrJoin);
            return;
        }
        else
        {
            switchState(MultiplayerMenuState.Room);
            return;
        }
    }

    public void OnClick_ReturnToMainMenu()
    {
        Disconnect();
    }

    public void OnClick_ModifyShip()
    {
        switchState(MultiplayerMenuState.Modify);
        foreach (PlayerListing item in ListOfPlayers)
        {
            if (item.Player == PhotonNetwork.LocalPlayer)
            {
                item.resetReady();
            }
        }
    }

    public void OnClick_ExitModifyShip()
    {
        switch (prevState)
        {
            case MultiplayerMenuState.CreateOrJoin:
                switchState(MultiplayerMenuState.CreateOrJoin);
                break;
            case MultiplayerMenuState.Room:
                switchState(MultiplayerMenuState.Room);
                break;
            case MultiplayerMenuState.Modify:
                break;
            case MultiplayerMenuState.Stats:
                switchState(MultiplayerMenuState.Stats);
                break;
            default:
                break;
        }
    }

    public void OnClick_LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    #endregion

    #region HostOnly
    public void OnClick_StartGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PV.RPC("RPC_SetPlayerData", RpcTarget.All);
        Debug.Log("Start Game Pressed");
        loadSync.OnClick_HostStartTransition();
    }
    #endregion

    public void Disconnect()
    {
        StartCoroutine(Disconnecting());
    }

    IEnumerator Disconnecting()
    {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
        {
            yield return null;
        }
        SceneManager.LoadScene("TestMainMenu");
    }

    [PunRPC]
    public void RPC_SetPlayerData()
    {
        dummyShip.savePhotonPlayerData();
    }

}
