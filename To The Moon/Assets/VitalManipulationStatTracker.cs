using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VitalManipulationStatTracker : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
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

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        
    }
}
