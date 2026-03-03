using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SceneLoadSync : MonoBehaviourPunCallbacks
{
    public static SceneLoadSync Instance;

    [SerializeField] PhotonView PV;
    [SerializeField] Canvas loadscreen;
    [SerializeField] Slider loadBar;
    [SerializeField] TMP_Text waitingForText;
    [SerializeField] string sceneName = "M_PVP";
    [SerializeField] Player[] playersJoined;

    List<bool> playersLoaded = new List<bool>();
    int loadedCount = 0;
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
    }
    
    public void OnClick_MasterStartGame()
    {
        PV.RPC("RPC_StartTransition", RpcTarget.All);
    }

    [PunRPC]
    void RPC_StartTransition()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Host Process Started");
            hostProcess();
        }
        else
        {
            Debug.Log("Join Process Started");
            joinerProcess();
        }
    }
    
    public void joinerProcess()
    {
        loadscreen.gameObject.SetActive(true);
        playersJoined = PhotonNetwork.PlayerList;
        for (int i = 0; i < playersJoined.Length; i++)
        {
            playersLoaded.Add(false);
        }
        Debug.Log("Joiner - LoadScreen ON and playersloadedlist initialized");
        StartCoroutine(JoinerLoad());
    }

    public void hostProcess()
    {
        loadscreen.gameObject.SetActive(true);
        playersJoined = PhotonNetwork.PlayerList;
        for (int i = 0; i < playersJoined.Length; i++)
        {
            playersLoaded.Add(false);
        }
        Debug.Log("Host - LoadScreen ON and playersloadedlist initialized");
        StartCoroutine(HostLoad());
    }

    [PunRPC]
    void RPC_NotifyOthers(Player readyPlayer)
    {
        //loadedCount++;
        for (int i = 0; i < playersJoined.Length; i++)
        {
            if (playersJoined[i] == readyPlayer)
            {
                playersLoaded[i] = true;
            }
        }
    }

    IEnumerator HostLoad()
    {
        Debug.Log("HostLoad started, Loading level");
        PhotonNetwork.LoadLevel(sceneName);
        while (PhotonNetwork.LevelLoadingProgress < 1)
        {
            float value = (PhotonNetwork.LevelLoadingProgress * 100) * 0.5f + (loadedCount / playersJoined.Length * 100) * 0.5f;
            loadBar.value = value;
            yield return null;
        }
        Debug.Log("Host - level load complete, Notifying Others");
        PV.RPC("RPC_NotifyOthers", RpcTarget.All, PhotonNetwork.LocalPlayer);
        Debug.Log("Host - Other Users notified, waiting for players");
        while (loadedCount < playersJoined.Length)
        {
            float value = (PhotonNetwork.LevelLoadingProgress * 100) * 0.5f + (loadedCount / playersJoined.Length * 100) * 0.5f;
            loadBar.value = value;
            loadedCount = 0;
            for (int i = 0; i < playersLoaded.Count; i++)
            {
                if (playersLoaded[i] == true)
                {
                    loadedCount++;
                }
            }
            Debug.Log("Host - Loaded Players checked, Loaded: (" + loadedCount + "/" + playersJoined.Length + ")");
            waitingForText.gameObject.SetActive(true);
            waitingForText.text = "Waiting for Players: (" + loadedCount + "/" + playersJoined.Length + ")";
            yield return new WaitForSeconds(0.5f);
        }
        Debug.Log("Host - All Players loaded - deactivating loadscreen");
        loadscreen.gameObject.SetActive(false);
        Destroy(gameObject, 1);
    }

    IEnumerator JoinerLoad()
    {
        Debug.Log("JoinerLoad started, Loading level");
        while (PhotonNetwork.LevelLoadingProgress < 1)
        {
            float value = (PhotonNetwork.LevelLoadingProgress * 100) * 0.5f + (loadedCount / playersJoined.Length * 100) * 0.5f;
            loadBar.value = value;
            yield return null;
        }
        Debug.Log("Joiner - level load complete, Notifying Others");
        PV.RPC("RPC_NotifyOthers", RpcTarget.All, PhotonNetwork.LocalPlayer);
        Debug.Log("Joiner - Other Users notified, waiting for players");
        while (loadedCount < playersJoined.Length)
        {
            float value = (PhotonNetwork.LevelLoadingProgress * 100) * 0.5f + (loadedCount / playersJoined.Length * 100) * 0.5f;
            loadBar.value = value;
            loadedCount = 0;
            for (int i = 0; i < playersLoaded.Count; i++)
            {
                if (playersLoaded[i] == true)
                {
                    loadedCount++;
                }
            }
            Debug.Log("Joiner - Loaded Players checked, Loaded: (" + loadedCount + "/" + playersJoined.Length + ")");
            waitingForText.gameObject.SetActive(true);
            waitingForText.text = "Waiting for Players: (" + loadedCount + "/" + playersJoined.Length + ")";
            yield return new WaitForSeconds(0.5f);
        }
        Debug.Log("Joiner - All Players loaded - deactivating loadscreen");
        loadscreen.gameObject.SetActive(false);
        Destroy(gameObject, 1);
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
