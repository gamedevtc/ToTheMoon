using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField] protected WeaponStats stats;
    [SerializeField] protected bool changesColor;
    [SerializeField] protected bool ultimate;
    [SerializeField] protected float rateOfFire;
    [SerializeField] protected float currUltValue = 0;
    [SerializeField] protected bool paused;

    [SerializeField] protected Ship shipMain;
    [SerializeField] protected MeshRenderer[] meshes;
    [SerializeField] protected GameObject gunOrigin;
    [SerializeField] protected AudioSource weaponSound;


    public virtual void Awake()
    {
        if (ultimate)
        {
            shipMain = GetComponentInParent<WeaponLink>().getShipMain();
        }
        else
        {
            shipMain = GetComponentInParent<Transform>().GetComponentInParent<WeaponLink>().getShipMain();
        }
        meshes = GetComponentsInChildren<MeshRenderer>();
    }

    public virtual void Update()
    {
        if (paused)
        {
            return;
        }

        rateOfFire += Time.deltaTime;

        if (ultimate)
        {
            if (currUltValue >= stats.ultChargeTime)
            {
                currUltValue = stats.ultChargeTime;
            }
            else
            {
                currUltValue += Time.deltaTime * stats.ultGainMultiplier;
            }
        }
    }


    public virtual void setColor(Material mat)
    {
        if (changesColor)
        {
            for (int i = 0; i < meshes.Length; i++)
            {
                meshes[i].material = mat;
            }
        }
    }

    private void OnEnable()
    {
        if (!GameManagerBase.Instance.isMulti())
        {
            //add pause and unpause to event
            EventManager.pauseEvent += Pause;
            EventManager.unPauseEvent += unPause;
        }
        if (changesColor)
        {
            if (shipMain)
            {
                setColor(shipMain.getActiveColor());
            }
        }
    }

    private void OnDisable()
    {
        if (!GameManagerBase.Instance.isMulti())
        {
            //remove from event
            EventManager.pauseEvent -= Pause;
            EventManager.unPauseEvent -= unPause;
        }
    }

    public virtual void Fire()
    {
        if (GameManagerBase.Instance.isMulti())
        {
            MultiplayerFire();
        }
        else
        {
            SingleplayerFire();
        }
    }

    public virtual void fillUlt()
    {
        currUltValue = stats.ultChargeTime;
    }

    public virtual float getCurrentUltValue()
    {
        return currUltValue;
    }

    public virtual float getUltChargeTime()
    {
        return stats.ultChargeTime;
    }

    public abstract void MultiplayerFire();
    public abstract void SingleplayerFire();


    protected virtual void Pause()
    {
        paused = true;
    }

    protected virtual void unPause()
    {
        paused = false;
    }
}
