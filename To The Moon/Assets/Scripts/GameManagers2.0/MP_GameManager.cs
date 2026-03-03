using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MP_GameManager : GameManagerBase
{
    //[SerializeField] public mp_gameMode currGameMode;

    //[SerializeField] float freeForAllTimer = 0;

    //private void Awake()
    //{
    //    if (Instance)
    //    {
    //        Destroy(gameObject);
    //        return;
    //    }
    //    else
    //    {
    //        DontDestroyOnLoad(gameObject);
    //        Instance = this;
    //    }
    //}

    //public enum mp_gameMode
    //{
    //    FreeForAll = 0,
    //    Coop
    //}

    //private void Update()
    //{
    //    switch (currState)
    //    {
    //        case gameState.Lobby:
    //            break;
    //        case gameState.Running:
    //            if (currGameMode == mp_gameMode.FreeForAll)
    //            {
    //                freeForAllTimer -= Time.deltaTime;
    //                if (freeForAllTimer <= 0)
    //                {
    //                    currState = gameState.Win;
    //                }
    //            }
    //            break;
    //        case gameState.Win:
    //            break;
    //        case gameState.Lose:
    //            break;
    //    }
    //}

    //void setTimer(float time)
    //{
    //    freeForAllTimer = time;
    //}

    //float getTimer()
    //{
    //    return freeForAllTimer;
    //}

    //public override bool isMulti()
    //{
    //    return true;
    //}
}
