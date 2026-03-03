using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_LevelSettings : MonoBehaviour
{

    [SerializeField] GameManager gameManager;

    [Header("Multiplayer Settings")]
    [SerializeField] GameObject asteroidlvl1;
    [SerializeField] int boundsRadiilvl1;
    [SerializeField] int numOfAsteriodslvl1;
    [SerializeField] int numOfWaveslvl1;
    [SerializeField] List<int> numOfEnemiesLvl1;



    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void SetLevelData(int num)
    {
        if (gameManager != null)
        {
            switch (num)
            {
                case 1:
                    //gameManager.setLevel(asteroidlvl1, boundsRadiilvl1, numOfAsteriodslvl1, numOfWaveslvl1, numOfEnemiesLvl1);
                    break;
                
            }
        }
    }
}


