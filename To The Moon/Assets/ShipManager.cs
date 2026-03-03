using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class ShipManager : MonoBehaviour
{


    [SerializeField] SpawnPoint[] Spawnpoints;
    //[SerializeField] PhotonView pv;
   
    [SerializeField]public GameObject Player;

    [SerializeField] SpawnPoint SavedSpawn;
    [SerializeField] MatchManager _MatchManager;
    [SerializeField] GameObject DeathCam;
    [SerializeField] float RespawnTime = 5;

    [SerializeField] Transform savedSpawn;

    bool once = false;
    int t;
    // Start is called before the first frame update
    void Start()
    {
        //DeathScreen.SetActive(false);
        Spawnpoints = FindObjectsOfType<SpawnPoint>();
        // t = Random.Range(0, Spawnpoints.Length);
       // savedSpawn = LevelBuilder.Instance.getOpenSpawn();
        Respawn();
        //  Player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "m_Player4.0"), SavedSpawn.transform.position, SavedSpawn.transform.localRotation);
        // SavedSpawn.Claim(Player.GetComponent<PhotonView>().ViewID);
        Debug.Log("Player " + PhotonNetwork.LocalPlayer.NickName + " took spawn point " + SavedSpawn.name);
    }



    public void Respawn()
    {
        //bool found = false;
        //if (SavedSpawn == null)
        //{
        //    for (int i = 0; i < Spawnpoints.Length; i++)
        //    {
        //        if (Spawnpoints[i].IsTaken() == false && found == false)
        //        {

        //            Spawnpoints[i].Claim(1);
        //            SavedSpawn = Spawnpoints[i];
        //            found = true;
        //        }
        //    }

        //}
        t = Random.Range(0, Spawnpoints.Length);


        SavedSpawn = Spawnpoints[t];



        Player = PhotonNetwork.Instantiate("m_Player4.0 2", SavedSpawn.transform.position, SavedSpawn.transform.localRotation);

        if (DeathCam)
        {
            Destroy(DeathCam.gameObject);
            DeathCam = null;
        }
    }

    public void setDeathCam(GameObject cam)
    {
        DeathCam = cam;
    }

    public void Death()
    {
        
        //DeathScreen.SetActive(true);

        PhotonNetwork.Destroy(Player);


        StartCoroutine(Respawning());
    }

    IEnumerator Respawning()
    {


        yield return new WaitForSeconds(RespawnTime);

        //DeathScreen.SetActive(false);
        
        Respawn();
    }
 

}
