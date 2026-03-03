using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
public class quit : MonoBehaviour
{
    public void ReturnToLobby()
    {
        if (GameManagerBase.Instance.isMulti())
        {
            StartCoroutine(Disconnecting());
        }
    }
    IEnumerator Disconnecting()
    {
        Debug.Log("Disconnecting");
        PhotonNetwork.LeaveRoom();
        while (PhotonNetwork.InRoom)
        {
            Debug.Log("Waiting to disconnect from room");
            yield return null;
        }
        PhotonNetwork.Disconnect();
        //while (PhotonNetwork.IsConnected)
        //{
        //    Debug.Log("Waiting to disconnect");
        //    yield return null;
        //}
        Debug.Log("Disconnected");
        //PhotonNetwork.LoadLevel("Main Menu");
        SceneManager.LoadScene("Main Menu");
    }
}
