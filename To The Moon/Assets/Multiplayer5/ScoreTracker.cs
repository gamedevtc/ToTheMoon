using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ScoreTracker : MonoBehaviour
{
    public const string PlayerKillsLabel = "kills";
    public const string PlayerDeathsLabel = "deaths";
    public const string PlayerTimeLabel = "time";
}

public static class ScoreTrackerFunctions
{
    public static void SetKills(this Player player, int newCount)
    {
        Hashtable hash = new Hashtable();
        hash[ScoreTracker.PlayerKillsLabel] = newCount;
        player.SetCustomProperties(hash);
    }

    public static void AddKill(this Player player)
    {
        int current = player.GetKills();
        current += 1;

        Hashtable hash = new Hashtable();
        hash[ScoreTracker.PlayerKillsLabel] = current;
        player.SetCustomProperties(hash);
    }

    public static int GetKills(this Player player)
    {
        object kills;
        if (player.CustomProperties.TryGetValue(ScoreTracker.PlayerKillsLabel, out kills))
        {
            return (int)kills;
        }
        else
        {
            return 0;
        }
    }

    public static void SetDeaths(this Player player, int newCount)
    {
        Hashtable hash = new Hashtable();
        hash[ScoreTracker.PlayerDeathsLabel] = newCount;
        player.SetCustomProperties(hash);
    }

    public static void AddDeath(this Player player)
    {
        int current = player.GetDeaths();
        current += 1;

        Hashtable hash = new Hashtable();
        hash[ScoreTracker.PlayerDeathsLabel] = current;
        player.SetCustomProperties(hash);
    }

    public static int GetDeaths(this Player player)
    {
        object deaths;
        if (player.CustomProperties.TryGetValue(ScoreTracker.PlayerDeathsLabel, out deaths))
        {
            return (int)deaths;
        }
        else
        {
            return 0;
        }
    }

    public static void SetTime(this Player player, int newCount)
    {
        Hashtable hash = new Hashtable();
        hash[ScoreTracker.PlayerTimeLabel] = newCount;
        player.SetCustomProperties(hash);
    }

    public static void AddTime(this Player player, float time)
    {
        float current = player.GetTime();
        current += time;

        Hashtable hash = new Hashtable();
        hash[ScoreTracker.PlayerTimeLabel] = current;
        player.SetCustomProperties(hash);
    }

    public static float GetTime(this Player player)
    {
        object time;
        if (player.CustomProperties.TryGetValue(ScoreTracker.PlayerTimeLabel, out time))
        {
            return (float)time;
        }
        else
        {
            return 0;
        }
    }
}