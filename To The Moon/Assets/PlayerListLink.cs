using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerListLink : MonoBehaviourPunCallbacks
{
    [SerializeField] MultiplayerLobbyMaster master;

    public override void OnEnable()
    {
        base.OnEnable();
        master.EnableLink();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        master.DisableLink();
    }
}
