using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player3BodyHolder : BasePart
{
    [SerializeField] Player3 playerMain;
    [SerializeField] Player3Body[] bodyArray;
    [SerializeField] List<Player3Body> bodyList;

    [SerializeField] Player3Body activeBody;
    Player3Body.bodyStats currStats;

    public void updateActiveBody()
    {
        if (GameManagerBase.Instance.isMulti())
        {
            currStats = new Player3Body.bodyStats();
            for (int i = 0; i < bodyList.Count; i++)
            {
                if (bodyList[i].gameObject.activeInHierarchy)
                {
                    activeBody = bodyList[i];
                    updateStats();
                    break;
                }
            }
        }
        else
        {
            bodyArray = GetComponentsInChildren<Player3Body>();
            currStats = new Player3Body.bodyStats();
            for (int i = 0; i < bodyArray.Length; i++)
            {
                if (bodyArray[i].isActiveAndEnabled)
                {
                    activeBody = bodyArray[i];
                    updateStats();
                    break;
                }
            }
        }
    }

    public override void updateColor(Material color)
    {
        activeBody.updateColor(color);
    }

    public void updateStats()
    {
        activeBody.getStats(out currStats);
    }
    public float getSpeed()
    {
        return currStats.speed;
    }
    public float getAccSpeed()
    {
        return currStats.accSpeed;
    }
    public float getDecSpeed()
    {
        return currStats.decSpeed;
    }
    public float getMaxHealth()
    {
        return currStats.maxHealth;
    }
    public float getMaxBoost()
    {
        return currStats.maxBoost;
    }
    public float getBoostStrength()
    {
        return currStats.boostStrength;
    }
    public float getBoostDecrease()
    {
        return currStats.boostDecreaseModifier;
    }
    public float getHandlingSpeed()
    {
        return currStats.handlingSpeed;
    }
    public float getZRotationSpeed()
    {
        return currStats.zRotationSpeed;
    }

    public Player3 getPlayer()
    {
        return playerMain;
    }

    public Mesh getMesh()
    {
        return activeBody.getMesh();
    }
}
