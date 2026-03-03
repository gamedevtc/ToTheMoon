using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] protected bool changesColor;
    [SerializeField] protected Player3 playerMain;

    [Header("Projectile Variables")]
    [SerializeField] protected GameObject gunOrigin;
    [SerializeField] protected AudioSource weaponSound;
    [SerializeField] protected GameObject bulletOG;
    [SerializeField] protected float cooldown = 0.0f;
    [SerializeField] protected float weaponCooldown = 0.15f;
    //[SerializeField] private int weaponSpot = 0;

    [Header("Ultimate Data")]
    [SerializeField] protected bool ultimate;
    [SerializeField] protected float ultChargeTime = 15;
    [SerializeField] protected float ultGainMultiplier = 1;
    [Header("Internal values")]
    [SerializeField] protected float currUltValue = 0;
    [SerializeField] protected bool paused;

    private void Update()
    {
        if (paused)
        {
            return;
        }

        if (ultimate)
        {
            if (currUltValue <= ultChargeTime)
            {
                currUltValue += (Time.deltaTime * ultGainMultiplier);
            }
            else if (currUltValue > ultChargeTime)
            {
                currUltValue = ultChargeTime;
            }
        }
        else
        {
            cooldown += Time.deltaTime;
        }
    }

    public virtual void Shoot(GameObject shooter)
    {
        if (ultimate && !paused && currUltValue == ultChargeTime)
        {
            if (GameManagerBase.Instance.isMulti() == true)
            {
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "M_Bomb"), gunOrigin.transform.position, Quaternion.LookRotation(gunOrigin.transform.forward));
            }
            else
            {
                Instantiate(bulletOG, gunOrigin.transform.position, Quaternion.LookRotation(gunOrigin.transform.forward));
            }
            currUltValue -= ultChargeTime;
        }
        else if (cooldown >= weaponCooldown && !paused)
        {
            weaponSound.Play();
            if (GameManagerBase.Instance.isMulti() == true)
            {
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "M_bullet"), gunOrigin.transform.position, Quaternion.LookRotation(gunOrigin.transform.forward));
            }
            else
            {
                Instantiate(bulletOG, gunOrigin.transform.position, Quaternion.LookRotation(gunOrigin.transform.forward));
            }
            cooldown -= cooldown;
        }
    }

    public void FillUlt()
    {
        currUltValue = ultChargeTime;
    }

    public float getCurrUltValue()
    {
        if (ultimate)
        {
            return currUltValue;
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
            return ultChargeTime;
        }
        else
        {
            return 0;
        }
    }

    public void updateColor(Material color)
    {
        if (changesColor)
        {

        }
    }

    private void Pause()
    {
        paused = true;
    }

    private void unPause()
    {
        paused = false;
    }

    private void OnEnable()
    {
        playerMain = GetComponentInParent<WeaponHolder>().getPlayer();
        if (changesColor)
        {
            updateColor(playerMain.getActiveColor());
        }
        EventManager.pauseEvent += Pause;
        EventManager.unPauseEvent += unPause;
    }

    private void OnDisable()
    {
        EventManager.pauseEvent -= Pause;
        EventManager.unPauseEvent -= unPause;
    }
}
