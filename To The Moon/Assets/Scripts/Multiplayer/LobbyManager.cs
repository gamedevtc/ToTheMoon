using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.IO;
using Photon.Realtime;
using UnityEditor;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public static LobbyManager lobby;

    public GameManager manager;

    private void Awake()
    {
        if (lobby)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        lobby = this;

        manager = FindObjectOfType<GameManager>();
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

    public void OnSceneLoaded(Scene scene,LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == 0)
        {
            Destroy(this.gameObject);
        }
        if (scene.buildIndex == 1 || scene.buildIndex == 2 )
        {

            //PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManger"), Vector3.zero, Quaternion.identity);

        }

    }

}
