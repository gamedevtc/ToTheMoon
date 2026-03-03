using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

using Hashtable = ExitGames.Client.Photon.Hashtable;
public class PlayerManager : MonoBehaviour
{
    [SerializeField] string playerPrefab;
    [SerializeField] int kills = 0;
    [SerializeField] int deaths = 0;
    GameObject player;
    [SerializeField] string name;

    [SerializeField] PhotonView view;
    GameObject _ui;

    GameObject[] spawnpoints;

    Transform spawn;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
        _ui = GameObject.Find("DeathScreen");
    }

    // Start is called before the first frame update
    void Start()
    {
        name = PhotonNetwork.LocalPlayer.NickName;
        spawnpoints = GameObject.FindGameObjectsWithTag("Spawnpoint");
        if (view.IsMine)
        {
            PhotonNetwork.LocalPlayer.SetTime(0);
            PhotonNetwork.LocalPlayer.SetDeaths(0);
            PhotonNetwork.LocalPlayer.SetKills(0);
            CreatePlayerShip();
        }
        if (_ui)
        {
            _ui.SetActive(false);
        }
    }

    private void Update()
    {
        if (view.IsMine)
        {
            PhotonNetwork.LocalPlayer.AddTime(Time.deltaTime);
        }
    }

    [PunRPC]
    public void addDeath()
    {
        if (view.IsMine)
        {
            return;
        }
        deaths += 1;
    }

    public PhotonView getView()
    {
        return view;
    }

    public string getname()
    {
        return name;
    }
   
    public void addKill(int id)
    {
        if (PhotonNetwork.GetPhotonView(id).IsMine)
        {
           // kills += 1;
        }
      
        
    }


    public int getkills()
    {
        return kills;
    }

   public int getdeaths()
    {
        return deaths;
    }

    IEnumerator Respawning()
    {
        

        yield return new WaitForSeconds(5);

        _ui.SetActive(false);
        CreatePlayerShip();
    }

    public void GameOver()
    {
        PhotonNetwork.Destroy(player);
        
    }

    void CreatePlayerShip()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.CustomProperties.ToString());
        int ran;
        ran = Random.Range(0, spawnpoints.Length);
        if (!spawn)
        {
            spawn = LevelBuilder.Instance.getOpenSpawn();
        }
        player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "m_Player4.0"), spawn.position, spawn.rotation, 0, new object[] { view.ViewID });
        spawn.gameObject.GetComponent<SpawnPoint>().Claim(view.ViewID);
        //player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "m_Player4.0"), spawnpoints[ran].transform.position, spawnpoints[ran].transform.rotation, 0, new object[] { view.ViewID });
        //Debug.Log("Ship spawned at " + spawnpoints[ran].transform.position);
    }
    public void Death()
    {
        if (view.IsMine)
        {
            deaths += 1;
        }
        view.RPC("addDeath", RpcTarget.All);

        PhotonNetwork.Destroy(player);
        _ui.SetActive(true);
        StartCoroutine(Respawning());
    }

}
