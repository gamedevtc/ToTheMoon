using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject mmConfirm;
    [SerializeField] private GameObject options;
    [SerializeField] private GameObject hangConfirm;
    [SerializeField] private GameObject lobConfirm;
    [SerializeField] private GameObject hangButton;
    [SerializeField] private GameObject lobButton;
    [SerializeField] private Player4Base player;
    [SerializeField] private SceneSwitcher switcher;
    Player4Base.HUDData data = new Player4Base.HUDData();
    [SerializeField] bool pauseDataFromPlayer = false;
    bool rMenu = false;
    bool rHang = false;
    bool rLob = false;
    bool optionOpen = false;
    bool displaying = false;
    // Start is called before the first frame update
    void Start()
    {
        menu.SetActive(false);
        mmConfirm.SetActive(false);
        hangConfirm.SetActive(false);
        lobConfirm.SetActive(false);
        if (GameManagerBase.Instance.isMulti())
        {
            lobButton.SetActive(true);
            hangButton.SetActive(false);
        }
        else
        {
            lobButton.SetActive(false);
            hangButton.SetActive(true);
        }
    }

    public void Returning()
    {
        GameManagerBase.Instance.setState(GameManagerBase.gameState.Running);
        if (GameManagerBase.Instance.isMulti())
        {
            player.unPause();
        }
        else
        {
        EventManager.unPause();

        }
    }



    public void CancelExit()
    {
        rMenu = false;
        rHang = false;
        rLob = false;
    }

    public void IsExit()
    {
        rMenu = true;
        optionOpen = false;
        rHang = false;
        rLob = false;
    }

    public void IsHang()
    {
        rHang = true;
        optionOpen = false;
        rMenu = false;
        rLob = false;
    }

    public void IsLob()
    {
        rLob = true;
        optionOpen = false;
        rHang = false;
        rMenu = false;
    }

    public void OptionSelected()
    {
        optionOpen = true;
        rMenu = false;
        rHang = false;
        rLob = false;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (GameManagerBase.Instance.isMulti())
        {
            player.updateHUD(out data);

            pauseDataFromPlayer = data.pausedState;
            if (data.pausedState)
            {
                if (!displaying)
                {
                    menu.SetActive(true);
                    displaying = true;
                }
                options.SetActive(optionOpen);
                mmConfirm.SetActive(rMenu);
                hangConfirm.SetActive(rHang);
                lobConfirm.SetActive(rLob);
            }
            else
            {
                if (menu.activeSelf)
                {
                    menu.SetActive(false);
                    mmConfirm.SetActive(false);
                    displaying = false;
                    rMenu = false;
                    rHang = false;
                    rLob = false;
                    optionOpen = false;
                }
            }
        }
        else
        {
            if (GameManagerBase.Instance.getState() == GameManagerBase.gameState.Pause)
            {
                if (!displaying)
                {
                    menu.SetActive(true);
                    displaying = true;
                }
                options.SetActive(optionOpen);
                mmConfirm.SetActive(rMenu);
                hangConfirm.SetActive(rHang);
                lobConfirm.SetActive(rLob);

            }
            if (GameManagerBase.Instance.getState() != GameManagerBase.gameState.Pause && menu.activeSelf)
            {
                menu.SetActive(false);
                mmConfirm.SetActive(false);
                displaying = false;
                rMenu = false;
                rHang = false;
                rLob = false;
                optionOpen = false;
            }
        }
    }

    public void setState(int state)
    {
        GameManagerBase.Instance.setState((GameManagerBase.gameState)state);
    }

    public void setRunning()
    {
        if (SP_GameManager.Instance)
        {
            GameManagerBase.Instance.setState(GameManagerBase.gameState.Running);
        }
    }

    public void returntoMainMenu(string scene)
    {
        if (GameManagerBase.Instance.isMulti())
        {
            ReturnToLobby();
        }
        else
        {
            switcher.OnClick_ChangeScene(scene);
        }
    }

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
        SceneManager.LoadScene("TestMainMenu");
    }

}
