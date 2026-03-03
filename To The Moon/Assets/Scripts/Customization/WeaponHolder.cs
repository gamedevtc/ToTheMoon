using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : BasePart
{
    [SerializeField] List<Weapon> weaps;
    [SerializeField] bool ultimate;
    [SerializeField] Player3 playerMain;
    

    public void Shoot(GameObject shooter)
    {
        for (int i = 0; i < weaps.Count; i++)
        {
            if (weaps[i].isActiveAndEnabled)
            {
                weaps[i].Shoot(shooter);
            }
        }
    }

    public override void updateColor(Material color)
    {
        for (int i = 0; i < weaps.Count; i++)
        {
            if (weaps[i].isActiveAndEnabled)
            {
                weaps[i].updateColor(color);
            }
        }
    }

    public void FillUlt()
    {
        if (ultimate)
        {
            for (int i = 0; i < weaps.Count; i++)
            {
                if (weaps[i].isActiveAndEnabled)
                {
                    weaps[i].FillUlt();
                }
            }
        }
    }
    public float getCurrUltValue()
    {
        if (ultimate)
        {
            for (int i = 0; i < weaps.Count; i++)
            {
                if (weaps[i].isActiveAndEnabled)
                {
                    return weaps[i].getCurrUltValue();
                }
            }
            return 0;
        }
        else
        {
            return 0;
        }
    }

    public float getChargeTime()
    {
        if (ultimate)
        {
            for (int i = 0; i < weaps.Count; i++)
            {
                if (weaps[i].isActiveAndEnabled)
                {
                   return weaps[i].getChargeTime();
                }
            }
            return 0;
        }
        else
        {
            return 0;
        }
    }

    public Player3 getPlayer()
    {
        return playerMain;
    }
}
