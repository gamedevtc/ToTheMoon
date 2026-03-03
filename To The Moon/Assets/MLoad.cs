using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Compression;
using Photon.Pun.Simple;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Runtime.InteropServices.WindowsRuntime;

[PackObject]
[RequireComponent(typeof(NetObject))]
public class MLoad : MonoBehaviourPunCallbacks
{
    public static MLoad Instance;

    [SyncVar]
    public int readyPlayerCount = 0;
    [SyncVar]
    public bool releaseFromLoadWait = true;

    PhotonView PV;
    Player[] playerList;
    Dictionary<Player, bool> readyPlayers = new Dictionary<Player, bool>();

    [Header("Set This Stuff")]
    [SerializeField] const string PlayerTimeLabel = "LR";
    [SerializeField] string loadSceneName;
    [SerializeField] string levelSceneName;
    [SerializeField] string waitingForPlayersDefault = "Waiting for players: (";
    [SerializeField] GameObject countDownCanvas;
    [SerializeField] Animator Anim;
    [SerializeField] TMP_Text waitingForPlayersText;

    [Header("Animation Triggers")]
    [SerializeField] string fadeInTrigger = "FadeIn";
    [SerializeField] string fadeOutTrigger = "FadeOut";

    [Header("Transition Timing Numbers")]
    [SerializeField] float waitForFadeInFinish = 2;
    [SerializeField] float waitForFadeOutToFinish = 2;
    [SerializeField] float waitForWarpTime = 5;
    [SerializeField] float waitAndDestroyTime = 3;

    private void Start()
    {
        if (Instance)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        PV = GetComponent<PhotonView>();
    }

    public void OnClick_HostStartTransition()
    {
        playerList = PhotonNetwork.PlayerList;
        for (int i = 0; i < playerList.Count(); i++)
        {
            readyPlayers.Add(playerList[i], false);
        }
        countDownCanvas.SetActive(true);
        PV.RPC("RPC_TransitionStarting", RpcTarget.Others);
        Anim.SetTrigger(fadeInTrigger);
        StartCoroutine(HostTransitionToWarp());
    }

    [PunRPC]
    void RPC_TransitionStarting()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            return;
        }
        countDownCanvas.SetActive(true);
        StartCoroutine(JoinTrainsitionToWarp());
    }

    IEnumerator HostTransitionToWarp()
    {
        yield return new WaitForSeconds(waitForFadeInFinish);
        countDownCanvas.SetActive(false);
        PhotonNetwork.LoadLevel(loadSceneName);
        while (PhotonNetwork.LevelLoadingProgress < 1)
        {
            yield return null;
        }
        readyPlayers[PhotonNetwork.LocalPlayer] = true;
        while (releaseFromLoadWait)
        {
            activateReadyCountText();
            releaseFromLoadWait = checkPlayers();
            yield return null;
        }
        deactivateReadyCountText();
        Anim.SetTrigger(fadeOutTrigger);
        yield return new WaitForSeconds(waitForFadeOutToFinish);
        StartCoroutine(HostWarpWait());
    }

    IEnumerator HostWarpWait()
    {
        yield return new WaitForSeconds(waitForWarpTime);
        hostStartMatch();
    }

    IEnumerator JoinTrainsitionToWarp()
    {
        yield return new WaitForSeconds(waitForFadeInFinish);
        countDownCanvas.SetActive(false);
        while (PhotonNetwork.LevelLoadingProgress < 1)
        {
            yield return null;
        }
        PV.RPC("RPC_NotifyHost", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer);
        while (releaseFromLoadWait)
        {
            activateReadyCountText();
            yield return null;
        }
        deactivateReadyCountText();
    }

    [PunRPC]
    void RPC_NotifyHost(Player p)
    {
        readyPlayers[p] = true;
    }

    public void hostStartMatch()
    {
        playerList = PhotonNetwork.PlayerList;
        readyPlayers.Clear();
        for (int i = 0; i < playerList.Count(); i++)
        {
            readyPlayers.Add(playerList[i], false);
        }
        releaseFromLoadWait = true;
        readyPlayerCount = 0;
        PV.RPC("RPC_MatchStarting", RpcTarget.Others);
        Anim.SetTrigger(fadeInTrigger);
        StartCoroutine(HostTransitionToLevel());
    }

    [PunRPC]
    void RPC_MatchStarting()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            return;
        }
        StartCoroutine(JoinTransitionToLevel());
    }

    IEnumerator HostTransitionToLevel()
    {
        yield return new WaitForSeconds(waitForFadeInFinish);
        PhotonNetwork.LoadLevel(levelSceneName);
        while (PhotonNetwork.LevelLoadingProgress < 1)
        {
            yield return null;
        }
        readyPlayers[PhotonNetwork.LocalPlayer] = true;
        while (releaseFromLoadWait)
        {
            releaseFromLoadWait = checkPlayers();
            activateReadyCountText();
            yield return null;
        }
        deactivateReadyCountText();
        Anim.SetTrigger(fadeOutTrigger);
        yield return new WaitForSeconds(waitForFadeOutToFinish);
        PV.RPC("RPC_DestroySelf", RpcTarget.All);
    }

    IEnumerator JoinTransitionToLevel()
    {
        yield return new WaitForSeconds(waitForFadeInFinish);
        while (PhotonNetwork.LevelLoadingProgress < 1)
        {
            yield return null;
        }
        PV.RPC("RPC_NotifyHost", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer);
        while (releaseFromLoadWait)
        {
            activateReadyCountText();
            yield return null;
        }
        deactivateReadyCountText();
    }

    [PunRPC]
    void RPC_DestroySelf()
    {
        Destroy(this.gameObject, waitAndDestroyTime);
    }
    



    bool checkPlayers()
    {
        int numReady = 0;
        bool returnVal = false;
        foreach (KeyValuePair<Player, bool> item in readyPlayers)
        {
            if (item.Value == false)
            {
                returnVal = true;
            }
            else
            {
                numReady++;
            }
        }
        readyPlayerCount = numReady;
        return returnVal;
    }

    void activateReadyCountText()
    {
        waitingForPlayersText.gameObject.SetActive(true);
        waitingForPlayersText.text = waitingForPlayersDefault + readyPlayerCount + "/" + PhotonNetwork.PlayerList.Count() + ")";
    }

    void deactivateReadyCountText()
    {
        waitingForPlayersText.gameObject.SetActive(false);
        waitingForPlayersText.text = waitingForPlayersDefault + readyPlayerCount + "/" + PhotonNetwork.PlayerList.Count() + ")";
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == 0)
        {
            Destroy(gameObject);
        }
    }
}
