using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MatchSettings : MonoBehaviourPunCallbacks
{
    [SerializeField]public bool foggy;
    [SerializeField] Toggle t;
    [SerializeField] PhotonView pv;

    public static MatchSettings Instance;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        foggy = false;
    }

   public void setFog()
    {
        foggy = t.isOn;

        pv.RPC("RPC_EnableFog", RpcTarget.All, foggy);

    }


    [PunRPC]
    void RPC_EnableFog(bool e)
    {
        foggy = e;
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
