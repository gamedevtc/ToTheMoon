using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShip : MonoBehaviour
{
    private int maxHealth = 100;
    private int maxBoost = 250;
    private int ultGoal = 150;
    private int health=0;
    private int boost=0;
    private int ult=0;
    private bool paused = false;
    private bool win = false;
    private bool lose = false;

    private void Start()
    {
        health = maxHealth;
        boost = maxBoost;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public int GetMaxBoost()
    {
        return maxBoost;
    }

    public int GetHealth()
    {
        return health;
    }

    public int GetBoost()
    {
        return boost;
    }

    public int GetUltCharge()
    {
        if (ult >= ultGoal)
            return 100;
        float dec = (float)ult / (float)ultGoal;
        return (int)(dec * 100);
    }

    public void ButtonReturn()
    {
        paused = !paused;
    }

    public bool IsPaused()
    {
        return paused;
    }


    public bool getWin()
    {
        return win;
    }

    public bool getLose()
    {
        return lose;
    }

    // Update is called once per frame
    void Update()
    {
        if (!paused && !win && !lose)
        {
            if (health <= 0)
            {
                lose = true;
            }
            if (Input.GetKey(KeyCode.Alpha1) && health >=0)
            {
                health -= 1;
            }
            if (Input.GetKey(KeyCode.Alpha2) && boost >= 0)
            {
                boost -= 1;
            }
            if (!Input.GetKey(KeyCode.Alpha2) && boost != maxBoost)
            {
                boost += 1;
            }
            if (Input.GetKey(KeyCode.Alpha3))
            {
                ult += 1;
            }
            if (Input.GetKey(KeyCode.Alpha4))
            {
                win = true;
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                paused = !paused;
            }
        }
    }
}
