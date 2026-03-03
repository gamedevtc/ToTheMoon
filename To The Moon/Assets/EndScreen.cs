using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class EndScreen : MonoBehaviour
{
    [SerializeField] Text countdown;

    [SerializeField] Text playerlistz;
    string t;
    void FinalScreen()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            t += "Name: " + PhotonNetwork.PlayerList[i].NickName + ", K:" + PhotonNetwork.PlayerList[i].GetKills() + " D:" + PhotonNetwork.PlayerList[i].GetDeaths() + "\n";
        }
        playerlistz.text = t;
    }


    private void OnEnable()
    {
        FinalScreen();
        StartCoroutine(endtime());
    }

    public void Disconnect()
    {
        StartCoroutine(Disconnecting());
    }

    IEnumerator Disconnecting()
    {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
        {
            yield return null;
        }
        SceneManager.LoadScene("TestMainMenu");
    }


    IEnumerator endtime()
    {
        for (int i = 15; i > 0; i--)
        {
            countdown.text = "Next Match Starts in:" + "\n" + i.ToString();
            yield return new WaitForSecondsRealtime(1);
        }


        PhotonNetwork.LoadLevel("M_PVP");
    }

   
}
