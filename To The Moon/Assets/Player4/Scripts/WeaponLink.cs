using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponLink : MonoBehaviour
{
    public enum WeaponTypes
    {
        Primary = 0,
        Secondary,
        Ultimate,
        Flare
    }

    [SerializeField] List<GameObject> weaponContainers;
    [SerializeField] List<GameObject> holoweaponContainers;
    [SerializeField] List<WeaponBase> weapons;
    [SerializeField] WeaponTypes type;

    [SerializeField] Ship shipMain;

    private void Awake()
    {
       // shipMain = GetComponentInParent<Ship>();
    }

    public void Fire()
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i].gameObject.activeInHierarchy)
            {
                weapons[i].Fire();
            }
        }
    }

    public void setActiveWeapons(int set)
    {
        
        for (int i = 0; i < weaponContainers.Count; i++)
        {
            if(weaponContainers[i].activeInHierarchy)
            {
                weaponContainers[i].SetActive(false);
            }
        }
        int correction = 0;
        if (type == WeaponTypes.Primary)
        {
            correction = (int)Ship.BodyOption.COUNT;
        }
        else if (type == WeaponTypes.Secondary)
        {
            correction = (int)Ship.PrimaryOption.COUNT;
        }
        else if (type == WeaponTypes.Ultimate)
        {
            correction = (int)Ship.SecondaryOption.COUNT;
        }
        weaponContainers[set - correction].SetActive(true);
    }
    public void setHoloWeapons(int set)
    {
        for (int i = 0; i < weaponContainers.Count; i++)
        {
            if (weaponContainers[i].activeInHierarchy)
            {
                weaponContainers[i].SetActive(false);
            }
        }
        for (int i = 0; i < holoweaponContainers.Count; i++)
        {
            if (holoweaponContainers[i].activeInHierarchy)
            {
                holoweaponContainers[i].SetActive(false);
            }
        }
        int correction = 0;
        if (type == WeaponTypes.Primary)
        {
            correction = (int)Ship.BodyOption.COUNT;
        }
        else if (type == WeaponTypes.Secondary)
        {
            correction = (int)Ship.PrimaryOption.COUNT;
        }
        else if (type == WeaponTypes.Ultimate)
        {
            correction = (int)Ship.SecondaryOption.COUNT;
        }
        holoweaponContainers[set - correction].SetActive(true);
    }

    public void setColor(Material mat)
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i].gameObject.activeInHierarchy)
            {
                weapons[i].setColor(mat);
            }
        }
    }
    public void HoloOff()
    {
        for (int i = 0; i < holoweaponContainers.Count; i++)
        {
            if (holoweaponContainers[i].activeInHierarchy)
            {
                holoweaponContainers[i].SetActive(false);
            }
        }
    }

    public Ship getShipMain()
    {
        return shipMain;
    }

    #region ult functions
    public void fillUlt()
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i].gameObject.activeInHierarchy)
            {
                weapons[i].fillUlt();
            }
        }
    }
    public float getCurrentUltValue()
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i].gameObject.activeInHierarchy)
            {
                return weapons[i].getCurrentUltValue();
            }
        }
        return 0;
    }

    public float getUltChargeTime()
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i].gameObject.activeInHierarchy)
            {
                return weapons[i].getUltChargeTime();
            }
        }
        return 0;
    }

    #endregion
}
