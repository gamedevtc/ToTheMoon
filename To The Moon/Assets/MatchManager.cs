using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.Animations;


public class MatchManager : MonoBehaviour
{
    public enum gameState
    {
        PreGame,
        Running,
        Win,
        Lose
    }



    [SerializeField] public gameState currState;

    [SerializeField] PhotonView pv;

    [SerializeField] MultiPlayer4 startupz;


    [SerializeField] ShipManager _ShipManager;


    [Header("Match Time")]
    [SerializeField] Text textTime;
    [SerializeField] GameObject MatchTimer;
    [SerializeField] float timer;

    [Header("StartUp Time")]
    [SerializeField] Text StartText;
    [SerializeField] GameObject _startObject;
    [SerializeField] public float Startup;


    [Header("StartUpText")]
    [SerializeField] string first = "Current Objective";
    [SerializeField] string second = "Eliminate all targets";
    [SerializeField] string third = "Leave no survivors";
    [SerializeField] string currentInfo;


    [Header("Game Over Stuff")]
    [SerializeField] GameObject EndScreen;
    [SerializeField] GameObject DeathScreenDisable;
    [SerializeField] Text endText;
    [SerializeField] string end = "Match Finished";

    bool once = false;

    // Start is called before the first frame update
    void Start()
    {
        currState = gameState.PreGame;
        Startup = 10;
        timer = 300;
        pv = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
      

        switch (currState)
        {
            case gameState.PreGame:
                if (pv.IsMine)
                {
                    pv.RPC("RemoveTime", RpcTarget.All, Startup,true);
                 
                }
                if (Startup <= 0)
                {
                    currState = gameState.Running;
                    StartCoroutine(StartUpText());
                }
                _startObject.SetActive(true);
                MatchTimer.SetActive(false);

                    break;
            case gameState.Running:
                if (once)
                {

                }

                if (pv.IsMine)
                {
                    
                    pv.RPC("RemoveTime", RpcTarget.All, timer,false);
                }
                MatchTimer.SetActive(true);
                if (timer <=0)
                {
                    currState = gameState.Win;
                    StartCoroutine(GAmeover());
                }

               
                break;
            case gameState.Win:
                GameOver();
                
                break;
            case gameState.Lose:
                break;
        }



    }


    void GameOver()
    {
        _ShipManager.Player.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        EndScreen.SetActive(true);
        DeathScreenDisable.SetActive(false);
        MatchTimer.SetActive(false);

        StatSaveManager.StatData statUpdate = new StatSaveManager.StatData();
        statUpdate.Deaths = PhotonNetwork.LocalPlayer.GetDeaths();
        statUpdate.Kills = PhotonNetwork.LocalPlayer.GetKills();
        statUpdate.Lose = 0;
        statUpdate.Wins = 0;
        statUpdate.TimePlayed = PhotonNetwork.LocalPlayer.GetTime();

        StatSaveManager.Instance.updateStatData(statUpdate);
    }
    IEnumerator GAmeover()
    {

        for (int i = 0; i < end.Length + 1; i++)
        {
            currentInfo = end.Substring(0, i);
            endText.text = currentInfo;
            yield return new WaitForSeconds(0.075f);
        }
       

    }

    IEnumerator StartUpText()
    {

        for (int i = 0; i < first.Length + 1; i++)
        {
            currentInfo = first.Substring(0, i);
            StartText.text = currentInfo;
            yield return new WaitForSeconds(0.075f);
        }
        yield return new WaitForSeconds(1);
        for (int i = 0; i < second.Length + 1; i++)
        {
            currentInfo = second.Substring(0, i);
            StartText.text = currentInfo;
            yield return new WaitForSeconds(0.075f);
        }
        yield return new WaitForSeconds(1.5f);
        for (int i = 0; i < third.Length + 1; i++)
        {
            currentInfo = third.Substring(0, i);
            StartText.text = currentInfo;
            yield return new WaitForSeconds(0.075f);
        }
        yield return new WaitForSeconds(1.5f);
        _startObject.SetActive(false);

    }
    //

    [PunRPC]
    void RemoveTime(float time2,bool start)
    {
        time2 -= Time.deltaTime;
        if (start)
        {
            Startup = time2;
            StartText.text = "Match Begins in: "+ "\n" + Startup.ToString("F0");
        }
        else
        {
            timer = time2;
            textTime.text = "Time Remaining: " + "\n" + timer.ToString("F0");
        }

    }


}
