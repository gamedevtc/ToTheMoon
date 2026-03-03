using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class StatSaveManager : MonoBehaviour
{
    public struct StatData 
    {
        public int Kills;
        public int Deaths;
        public int Wins;
        public int Lose;
        public float TimePlayed;
        public Ship.BodyOption favoriteBody;
        public Ship.PrimaryOption favoritePrim;
        public Ship.SecondaryOption favoriteSec;
        public Ship.UltimateOption favoriteUlt;
    }

    private Dictionary<int, int> partDictionary = new Dictionary<int, int>();
    int partCount = (int)Ship.UltimateOption.COUNT;
    public static StatSaveManager Instance;
    [SerializeField] string fileName = "StatTest.txt";
    public StatData saveStatData;
    //[SerializeField] List<int> part;
    //[SerializeField] List<int> uses;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }

        if (!File.Exists(fileName))
        {
            defaultDic();
            setDefaults();
            saveStat();
        }

        readStat();
    }

    private void Update()
    {
        //part.Clear();
        //uses.Clear();
        //for (int i = 0; i < partCount; i++)
        //{
        //    part.Add(i);
        //    uses.Add(partDictionary[i]);
        //}
    }

    private void setDefaults()
    {
        saveStatData.Kills = 0;
        saveStatData.Deaths = 0;
        saveStatData.Wins = 0;
        saveStatData.Lose = 0;
        saveStatData.TimePlayed = 0;
        saveStatData.favoriteBody = Ship.BodyOption.Body1;
        saveStatData.favoritePrim = Ship.PrimaryOption.FusionBlaster;
        saveStatData.favoriteSec = Ship.SecondaryOption.Missiles;
        saveStatData.favoriteUlt = Ship.UltimateOption.MineLauncher;
    }

    public void updateStatData(StatData stats)
    {
        saveStatData.Kills += stats.Kills;
        saveStatData.Deaths += stats.Deaths;
        saveStatData.Wins += stats.Wins;
        saveStatData.Lose += stats.Lose;
        saveStatData.TimePlayed += stats.TimePlayed;
        //saveStatData.favoriteBody = stats.favoriteBody;
        //saveStatData.favoritePrim = stats.favoritePrim;
        //saveStatData.favoriteSec  = stats.favoriteSec;
        //saveStatData.favoriteUlt  = stats.favoriteUlt;
        saveStat();
    }

    public void saveStat()
    {
        var writer = new StreamWriter(File.Open(fileName, FileMode.OpenOrCreate));
        writer.WriteLine(saveStatData.Kills);
        writer.WriteLine(saveStatData.Deaths);
        writer.WriteLine(saveStatData.Wins);
        writer.WriteLine(saveStatData.Lose);
        writer.WriteLine(saveStatData.TimePlayed);
        writer.WriteLine((int)saveStatData.favoriteBody);
        writer.WriteLine((int)saveStatData.favoritePrim);
        writer.WriteLine((int)saveStatData.favoriteSec);
        writer.WriteLine((int)saveStatData.favoriteUlt);
        writer.WriteLine("----PART DATA----");
        writer.WriteLine(partCount);
        for (int i = 0; i < partCount; i++)
        {
            writer.WriteLine(partDictionary[i]);
        }
        writer.Close();
    }

    public void readStat()
    {
        var reader = new StreamReader(File.Open(fileName, FileMode.Open));
        saveStatData.Kills = int.Parse(reader.ReadLine());
        saveStatData.Deaths = int.Parse(reader.ReadLine());
        saveStatData.Wins = int.Parse(reader.ReadLine());
        saveStatData.Lose = int.Parse(reader.ReadLine());
        saveStatData.TimePlayed = float.Parse(reader.ReadLine());
        saveStatData.favoriteBody = (Ship.BodyOption)int.Parse(reader.ReadLine());
        saveStatData.favoritePrim = (Ship.PrimaryOption)int.Parse(reader.ReadLine());
        saveStatData.favoriteSec = (Ship.SecondaryOption)int.Parse(reader.ReadLine());
        saveStatData.favoriteUlt = (Ship.UltimateOption)int.Parse(reader.ReadLine());
        reader.ReadLine();
        partCount = int.Parse(reader.ReadLine());
        for (int i = 0; i < partCount; i++)
        {
            partDictionary[i] = int.Parse(reader.ReadLine());
        }
        reader.Close();
    }

    public Dictionary<int, int> getPartDic()
    {
        return partDictionary;
    }

    public void setPartDic(Dictionary<int, int> diction)
    {
        partDictionary = diction;
    }

    public void updateDic(Ship.playerShipSettings activeParts)
    {
        for (int i = 0; i < partCount; i++)
        {
            if (i == (int)activeParts.activeColor)
            {
                partDictionary[i] = partDictionary[i] += 1;
            }
            else if (i == (int)activeParts.activeBody)
            {
                partDictionary[i] = partDictionary[i] += 1;
            }
            else if (i == (int)activeParts.activePrimary)
            {
                partDictionary[i] = partDictionary[i] += 1;
            }
            else if (i == (int)activeParts.activeSecondary)
            {
                partDictionary[i] = partDictionary[i] += 1;
            }
            else if (i == (int)activeParts.activeUltimate)
            {
                partDictionary[i] = partDictionary[i] += 1;
            }
        }

        findFavorite();
    }

    void defaultDic()
    {
        for (int i = 0; i < partCount; i++)
        {
            partDictionary.Add(i, 0);
        }
    }

    void findFavorite() 
    {
        for (int i = 8; i < partCount; i++)
        {
            if (i < (int)Ship.BodyOption.COUNT)
            {
                if (partDictionary[i] > partDictionary[(int)saveStatData.favoriteBody])
                {
                    saveStatData.favoriteBody = (Ship.BodyOption)i;
                }
            }
            else if (i >= (int)Ship.BodyOption.COUNT && i < (int)Ship.PrimaryOption.COUNT)
            {
                if (partDictionary[i] > partDictionary[(int)saveStatData.favoritePrim])
                {
                    saveStatData.favoritePrim = (Ship.PrimaryOption)i;
                }
            }
            else if(i >= (int)Ship.PrimaryOption.COUNT && i < (int)Ship.SecondaryOption.COUNT)
            {
                if (partDictionary[i] > partDictionary[(int)saveStatData.favoriteSec])
                {
                    saveStatData.favoriteSec = (Ship.SecondaryOption)i;
                }
            }
            else if (i >= (int)Ship.SecondaryOption.COUNT && i < (int)Ship.UltimateOption.COUNT)
            {
                if (partDictionary[i] > partDictionary[(int)saveStatData.favoriteUlt])
                {
                    saveStatData.favoriteUlt = (Ship.UltimateOption)i;
                }
            }
        }
    }
}
