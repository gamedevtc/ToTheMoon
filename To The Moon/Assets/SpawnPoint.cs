using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] bool Taken;
    [SerializeField] int viewID;
    [SerializeField] string PlayerName;
    [SerializeField] PhotonView pv;
    void Start()
    {
        Taken = false;
        viewID = 0;
        PlayerName = null;
        pv = GetComponent<PhotonView>();
    }
    public bool IsTaken()
    {
        return Taken;
    }

    public void Unclaim()
    {
        pv.RPC("RPC_Unclaim", RpcTarget.All);
    }

    public void Claim(int id2)
    {
        Taken = true;
        viewID = id2;
        pv.RPC("RPC_ClaimSpawn", RpcTarget.All, viewID, Taken);
    }

    [PunRPC]
    void RPC_Unclaim()
    {
        Taken = false;
        viewID = 0;
        PlayerName = null;
    }


    [PunRPC]
    void RPC_ClaimSpawn(int id,bool y)
    {
        
        Taken = y;
        viewID = id;
        
    }


}
