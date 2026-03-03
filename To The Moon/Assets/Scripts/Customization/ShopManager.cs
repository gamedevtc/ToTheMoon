using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{

    [Header("Player")]
    [SerializeField] ShopAttachment player;
    [SerializeField] SaveData save;
    [SerializeField] List<int> cost;
    [SerializeField] TestShop shopButtons; // used for updating lists for buttons
    private Dictionary<int, bool> unlockedDictionary;
    private Dictionary<int, int> costDictionary;
    

    void Start()
    {
        readUnlockedList();
        InitializeCostDic();
    }

    public bool CheckUnlocked(int num)
    {
        return unlockedDictionary[num];
    }


    public void PreviewItem(int num)
    {
        if (0 <= num && num < (int)Ship.ColorOption.COUNT)
        {
            player.previewColor((Ship.ColorOption)num);
        }
        else if ((int)Ship.ColorOption.COUNT <= num && num < (int)Ship.BodyOption.COUNT)
        {
            player.previewHoloBody((Ship.BodyOption)num);
        }
        else if ((int)Ship.BodyOption.COUNT <= num && num < (int)Ship.PrimaryOption.COUNT)
        {
            player.previewHoloPrimary((Ship.PrimaryOption)num);
        }
        else if ((int)Ship.PrimaryOption.COUNT <= num && num < (int)Ship.SecondaryOption.COUNT)
        {
            player.previewHoloSecondary((Ship.SecondaryOption)num);
        }
        else if ((int)Ship.SecondaryOption.COUNT <= num && num < (int)Ship.UltimateOption.COUNT)
        {
            player.previewHoloUltimate((Ship.UltimateOption)num);
        }
    }

    public void EquipItem(int num)
    {
        if (0 <= num && num < (int)Ship.ColorOption.COUNT)
        {
            player.confirmColor();
        }
        else if ((int)Ship.ColorOption.COUNT <= num && num < (int)Ship.BodyOption.COUNT)
        {
            player.previewBody((Ship.BodyOption)num);
            player.confirmBody();
        }
        else if ((int)Ship.BodyOption.COUNT <= num && num < (int)Ship.PrimaryOption.COUNT)
        {
            player.previewPrimary((Ship.PrimaryOption)num);
            player.confirmPrimary();
        }
        else if ((int)Ship.PrimaryOption.COUNT <= num && num < (int)Ship.SecondaryOption.COUNT)
        {
            player.previewSecondary((Ship.SecondaryOption)num);
            player.confirmSecondary();
        }
        else if ((int)Ship.SecondaryOption.COUNT <= num && num < (int)Ship.UltimateOption.COUNT)
        {
            player.previewUltimate((Ship.UltimateOption)num);
            player.confirmUltimate();
        }
    }

    public void HoloOff()
    {
        player.HoloOff();
    }

    public void Purchase(int num)
    {
        if (save.GetCurrency() >= costDictionary[num])
        {
            save.SetCurrency(-costDictionary[num]);
            updateUnlockedDic(num);
        }
    }

    public void Return()
    {
        player.resetAllToActiveParts();
        save.saveData();
        if (GameManagerBase.Instance.isMulti())
        {
            player.savePhotonPlayerData();
        }
    }

    public void readUnlockedList()
    {
        unlockedDictionary = save.getUnlockedDic();
    }

    public void updateUnlockedDic(int num)
    {
        unlockedDictionary[num] = true;
        save.setUnlockedDic(unlockedDictionary);
        shopButtons.MakePurchase(num);
    }

    public void InitializeCostDic()
    {
        costDictionary = new Dictionary<int, int>();
        for (int i = 0; i < unlockedDictionary.Count; i++)
        {
            costDictionary.Add(i, cost[i]);
        }
    }

}
