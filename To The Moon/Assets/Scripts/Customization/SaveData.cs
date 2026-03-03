using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class SaveData : MonoBehaviour
{
    Ship.playerShipSettings saveStorage;
    [SerializeField] string fileName = "Test.txt";
    [SerializeField] string header = "----Ship Save Data----";
    private int currency = 0;
    int unlockedCount = (int)Ship.UltimateOption.COUNT;
    private Dictionary<int, bool> unlockedDic = new Dictionary<int, bool>();
    [SerializeField] int readInColor = 0;
    [SerializeField] int readInBody = 0;
    [SerializeField] int readInPrimary = 0;
    [SerializeField] int readInSecondary = 0;
    [SerializeField] int readInUlt = 0;

    private void Update()
    {
        readInColor = (int)saveStorage.activeColor;
        readInBody = (int)saveStorage.activeBody;
        readInPrimary = (int)saveStorage.activePrimary;
        readInSecondary = (int)saveStorage.activeSecondary;
        readInUlt = (int)saveStorage.activeUltimate;

    }
    private void Awake()
    {
        dictionaryInitialize();
        if (!File.Exists(fileName))
        {
            setDefaults();
            saveData();
        }
        
        readData();
        
    }

    public int GetCurrency()
    {
        return currency;
    }

    public void SetCurrency(int num)
    {
        currency += num;
    }

    public Dictionary<int, bool> getUnlockedDic()
    {
        return unlockedDic;
    }

    public void setUnlockedDic(Dictionary<int, bool> diction)
    {
        unlockedDic = diction;
    }

    public void getActiveSettings(out Ship.playerShipSettings settings)
    {
        settings.activeColor = saveStorage.activeColor;
        settings.activeBody = saveStorage.activeBody;
        settings.activePrimary = saveStorage.activePrimary;
        settings.activeSecondary = saveStorage.activeSecondary;
        settings.activeUltimate = saveStorage.activeUltimate;
    }

    public void saveShipPrefs(Ship.playerShipSettings ship)
    {
        saveStorage.activeColor = ship.activeColor;
        saveStorage.activeBody = ship.activeBody;
        saveStorage.activePrimary = ship.activePrimary;
        saveStorage.activeSecondary = ship.activeSecondary;
        saveStorage.activeUltimate = ship.activeUltimate;
    }

    //public methods
    public void saveData()
    {
        //StatSaveManager.Instance.updateDic(saveStorage);
        var writer = new StreamWriter(File.Open(fileName, FileMode.OpenOrCreate));
        writer.WriteLine(header);
        writer.WriteLine((int)saveStorage.activeColor);
        writer.WriteLine((int)saveStorage.activeBody);
        writer.WriteLine((int)saveStorage.activePrimary);
        writer.WriteLine((int)saveStorage.activeSecondary);
        writer.WriteLine((int)saveStorage.activeUltimate);
        writer.WriteLine(currency);
        writer.WriteLine("----Shop Data----");
        writer.WriteLine(unlockedCount);
        for (int i = 0; i < unlockedCount; i++)
        {
            writer.WriteLine(unlockedDic[i]);
        }
        writer.Close();
    }

    public void readData()
    {
        var reader = new StreamReader(File.Open(fileName, FileMode.Open));
        if (reader.ReadLine() == header)
        {
            saveStorage.activeColor = (Ship.ColorOption)int.Parse(reader.ReadLine());
            saveStorage.activeBody = (Ship.BodyOption)int.Parse(reader.ReadLine());
            saveStorage.activePrimary = (Ship.PrimaryOption)int.Parse(reader.ReadLine());
            saveStorage.activeSecondary = (Ship.SecondaryOption)int.Parse(reader.ReadLine());
            saveStorage.activeUltimate = (Ship.UltimateOption)int.Parse(reader.ReadLine());
            currency = int.Parse(reader.ReadLine());
            reader.ReadLine();
            unlockedCount = int.Parse(reader.ReadLine());
            for (int i = 0; i < unlockedCount; i++)
            {
                unlockedDic[i] = bool.Parse(reader.ReadLine());
            }
        }
        reader.Close();
    }

    void setDefaults()
    {
        saveStorage.activeColor = Ship.ColorOption.Red;
        saveStorage.activeBody = Ship.BodyOption.Body1;
        saveStorage.activePrimary = Ship.PrimaryOption.FusionBlaster;
        saveStorage.activeSecondary = Ship.SecondaryOption.Missiles;
        saveStorage.activeUltimate = Ship.UltimateOption.MineLauncher;
    }

    void dictionaryInitialize()
    {
        for (int i = 0; i < unlockedCount; i++)
        {
            if (i == (int)saveStorage.activeColor)
            {
                unlockedDic.Add(i, true);
            }
            else if (i == (int)saveStorage.activeBody)
            {
                unlockedDic.Add(i, true);
            }
            else if (i == (int)saveStorage.activePrimary)
            {
                unlockedDic.Add(i, true);
            }
            else if (i == (int)saveStorage.activeSecondary)
            {
                unlockedDic.Add(i, true);
            }
            else if (i == (int)saveStorage.activeUltimate)
            {
                unlockedDic.Add(i, true);
            }
            else
            {
                unlockedDic.Add(i, false);
            }
        }
    }
}
