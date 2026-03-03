using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
public class ResultScreen : MonoBehaviour
{
    [SerializeField] GameObject winScreen;
    [SerializeField] GameObject loseScreen;
    [SerializeField] Button WRestart;
    [SerializeField] Button LRestart;
    // Start is called before the first frame update
    void Start()
    {
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
        if (GameManagerBase.Instance.isMulti() == true)
        {
            WRestart.gameObject.SetActive(false);
            LRestart.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManagerBase.Instance.getState() == GameManagerBase.gameState.Win)
        {
            winScreen.SetActive(true);
            if (GameManagerBase.Instance.isMulti() == true)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    WRestart.gameObject.SetActive(true);
                    LRestart.gameObject.SetActive(true);
                }
            }
        }
        if (GameManagerBase.Instance.getState() == GameManagerBase.gameState.Lose)
        {
            loseScreen.SetActive(true);
            if (GameManagerBase.Instance.isMulti() == true)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    WRestart.gameObject.SetActive(true);
                    LRestart.gameObject.SetActive(true);
                }
            }
        }
    }

    public void setState(int state)
    {
        GameManagerBase.Instance.setState((GameManagerBase.gameState)state);
    }

    public void quitLobby()
    {
        PhotonNetwork.Disconnect();
        // PhotonNetwork.AutomaticallySyncScene = false;
    }
}
