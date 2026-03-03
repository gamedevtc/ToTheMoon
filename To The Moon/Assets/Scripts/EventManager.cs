using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    //anytime the game needs to freeze
    public delegate void PauseHandler();
    public static event PauseHandler pauseEvent;
    public static event PauseHandler unPauseEvent;

    public static void pause() { pauseEvent(); }
    public static void unPause() { unPauseEvent(); }


    public delegate Transform GetRespawnPoint();
    public static event GetRespawnPoint getpoint;

    public static Transform getSpawnPoint() { return getpoint(); }

}
