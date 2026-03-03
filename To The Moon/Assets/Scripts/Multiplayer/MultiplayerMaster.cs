using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MultiplayerMaster : MonoBehaviourPunCallbacks
{
    [Header("Player Settings")]
    [SerializeField] GameObject _playerName;
    [SerializeField] GameObject _textbox;

    [Header("Room Settings")]
    [SerializeField] byte Max_Players = 16;
    [SerializeField] Text _lobbyName;

    [Header("RoomList")]
    [SerializeField]  LobbyListing _lobbyList;
    [SerializeField]  Transform _content;
    private List<LobbyListing> _ListOfRooms = new List<LobbyListing>();

    [Header("Room")]
    [SerializeField] GameObject StartButton;
    [SerializeField] GameObject SettingsButton;
    [SerializeField] GameObject ReconnectButton;

    [Header("ye")]
    [SerializeField] GameObject CreateOrJoinCanvas;
    [SerializeField] GameObject LobbyCanvas;
    [SerializeField] GameObject StatCanvas;
    [SerializeField] GameObject MainLobby;
    [SerializeField] GameObject _Settings;
    [SerializeField] GameObject modifyCanvas;
    [SerializeField] GameObject modifyButton;
    [SerializeField] LobbySettings lobbySettings;
    [SerializeField] LevelSettings levelSettings;

    [SerializeField] GameObject MainCamera;
    [SerializeField] GameObject ShopCamera;
    //  [SerializeField] GameManager Manager;

    [SerializeField] ShopAttachment dummyShip;

    public void IsMultiplayer(bool multi)
    {
        //FindObjectOfType<GameManager>().setMulti(multi);
    }

    public void Awake()
    {
      //  Manager = FindObjectOfType<GameManager>();
      //  PhotonNetwork.AutomaticallySyncScene = Manager.Sync;
    }

    public void Start()
    {
        _playerName.GetComponentInChildren<Text>().text = PhotonNetwork.LocalPlayer.NickName;
        if (PhotonNetwork.InRoom)
        {
            LobbyCanvas.SetActive(true);
            CreateOrJoinCanvas.SetActive(false);
            StatCanvas.SetActive(false);
        }
        else
        {
            LobbyCanvas.SetActive(false);
            CreateOrJoinCanvas.SetActive(true);
            StatCanvas.SetActive(false);
        }
    }

    public void OnClick_Reconnect()
    {
    //    Manager.Sync = true;
     //   PhotonNetwork.AutomaticallySyncScene = Manager.Sync;
    }

    public void OnClick_ChangeName()
    {
        PhotonNetwork.NickName = _playerName.GetComponentInChildren<Text>().text;
        Debug.Log("Name: " + PhotonNetwork.LocalPlayer.NickName);
    }

    public override void OnRoomListUpdate(List<RoomInfo> lobbyList)
    {
        foreach (RoomInfo item in lobbyList)
        {
            if (item.RemovedFromList) //removes from lobby list
            {
                int index = _ListOfRooms.FindIndex(x => x.Roominfo.Name == item.Name);
                if (index != -1)
                {
                    Destroy(_ListOfRooms[index].gameObject);
                    _ListOfRooms.RemoveAt(index);
                }
            }
            else // adds to the lobby list
            {
                int index = _ListOfRooms.FindIndex(x => x.Roominfo.Name == item.Name);
                if (index == -1)
                {
                    LobbyListing listing = Instantiate(_lobbyList, _content);
                    if (listing != null)
                    {
                        listing.SetRoomInfo(item);
                        _ListOfRooms.Add(listing);
                    }
                }
            }
        }
    }

    public void OnClick_Modify()
    {
        MainLobby.SetActive(false);
        modifyCanvas.SetActive(true);
        MainCamera.SetActive(false);
        ShopCamera.SetActive(true);
    }

    public void OnClick_ExitModifyModify()
    {
        MainLobby.SetActive(true);
        modifyCanvas.SetActive(false);
        MainCamera.SetActive(true);
        ShopCamera.SetActive(false);
    }

    public void OnClick_CJtoStat()
    {
        CreateOrJoinCanvas.SetActive(false);
        StatCanvas.SetActive(true);
    }

    public void OnClick_StattoCJ()
    {
        CreateOrJoinCanvas.SetActive(true);
        StatCanvas.SetActive(false);
    }

    public void OnClick_CreateRoom()
    {
        //Makesure your connected to the server b4 makeing room
        if (!PhotonNetwork.IsConnected)
        {
            return;
        }

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = Max_Players;
        
        //JoinOrCreateRoom - Makes room, if it exists, you will join it
        PhotonNetwork.JoinOrCreateRoom(_lobbyName.text, options, TypedLobby.Default);

    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room", this);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room Not Created, Reason: " + message, this);
    }

    public override void OnJoinedRoom()
    {
        LobbyCanvas.SetActive(true);
        CreateOrJoinCanvas.SetActive(false);
        _content.DestroyChildren();
        _ListOfRooms.Clear();
        StartButton.SetActive(PhotonNetwork.IsMasterClient);
        SettingsButton.SetActive(PhotonNetwork.IsMasterClient);

        dummyShip.savePhotonPlayerData();
       
        foreach (Player item in PhotonNetwork.PlayerList)
        {
            if (item.IsMasterClient)
            {
                //Debug.Log("Host "+ item.UserId + " " + item.CustomProperties.ToString());
            }
            else
            {
                //Debug.Log(item.UserId + " "+ item.CustomProperties.ToString());

            }
        }
    }

    public void OnClick_LobbySettings()
    {
        MainLobby.SetActive(false);
        _Settings.SetActive(true);
    }

    public void OnClick_ReturnToLobby()
    {
        MainLobby.SetActive(true);
        _Settings.SetActive(false);
        modifyCanvas.SetActive(false);
    }

    public void OnClick_ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void OnClick_LeaveLobby()
    {
        PhotonNetwork.LeaveRoom();

        LobbyCanvas.SetActive(false);
        CreateOrJoinCanvas.SetActive(true);
    }

    public override void OnLeftRoom()
    {
        
    
        Debug.Log(PhotonNetwork.NickName + " Has Left Lobby");
        
    }

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

    public void StartGame()
    {
        if (lobbySettings.isCoop)
        {
            PhotonNetwork.LoadLevel("M_level");
            //SceneManager.LoadScene("M_level");
        }
        else
        {
            levelSettings.SetLevelData(6);
            PhotonNetwork.LoadLevel("M_PVP");
            //SceneManager.LoadScene("M_PVP");
        }
        StatSaveManager.Instance.updateDic(dummyShip.activeSettings);
    }
}
