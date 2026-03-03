using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class LevelSettings : MonoBehaviour
{
    [SerializeField] List<LevelInfo> levelList;

    [Header("Custom Settings")]
    //Map Settings
    [SerializeField] public LevelInfo.levelDifficulty difficultyCustom;
    [SerializeField] public int mapRadiusCustom;
    //Asteroid Settinpublic gs
    [SerializeField] public int asteroidCountCustom;
    [SerializeField] public int asteroidMinScaleCustom;
    [SerializeField] public int asteroidMaxScaleCustom;
    [SerializeField] public int asteroidMinGapCustom;
    [SerializeField] public bool fluffyAsteroidBoundsCustom;
    [SerializeField] public int fluffyFactorCustom;
    [SerializeField] public bool spawnOutsideAsteroidsCustom;
    //Backgroudn Settings
    [SerializeField] public LevelInfo.levelBackground backgroundSettingsCustom;
    [SerializeField] public LevelInfo.largeObject backgroundObjectCustom;
    //Enemy Settings
    [SerializeField] public int numOfWavesCustom;
    //Completion Values
    [SerializeField] public int initialRewardCustom;
    [SerializeField] public int secondaryRewardCustom;


    [Header("Default Settings")]
    //Map Settings
    [SerializeField] LevelInfo.levelDifficulty difficultyDefault;
    [SerializeField] int mapRadiusDefault;
    //Asteroid Settings
    [SerializeField] int asteroidCountDefault;
    [SerializeField] int asteroidMinScaleDefault;
    [SerializeField] int asteroidMaxScaleDefault;
    [SerializeField] int asteroidMinGapDefault;
    [SerializeField] bool fluffyAsteroidBoundsDefault;
    [SerializeField] int fluffyFactorDefault;
    [SerializeField] bool spawnOutsideAsteroidsDefault;
    //Backgroudn Settings
    [SerializeField] LevelInfo.levelBackground backgroundSettingsDefault;
    [SerializeField] LevelInfo.largeObject backgroundObjectDefault;
    //Enemy Settings
    [SerializeField] int numOfWavesDefault;
    //Completion Values
    [SerializeField] int initialRewardDefault;
    [SerializeField] int secondaryRewardDefault;

    public void SetLevelData(int num)
    {
        LevelInfo.LevelSettings newLevel = new LevelInfo.LevelSettings();

        if (num-1 < levelList.Count)
        {
            num -= 1;
            //Map Settings
            newLevel.difficulty = levelList[num].difficulty;
            newLevel.mapRadius = levelList[num].mapRadius;
            //Asteroid Settings
            newLevel.asteroidCount = levelList[num].asteroidCount;
            newLevel.asteroidMinScale = levelList[num].asteroidMinScale;
            newLevel.asteroidMaxScale = levelList[num].asteroidMaxScale;
            newLevel.asteroidMinGap = levelList[num].asteroidMinGap;
            newLevel.fluffyAsteroidBounds = levelList[num].fluffyAsteroidBounds;
            newLevel.fluffyFactor = levelList[num].fluffyFactor;
            newLevel.spawnOutsideAsteroids = levelList[num].spawnOutsideAsteroids;
            //Background Settings
            newLevel.backgroundSettings = levelList[num].backgroundSettings;
            newLevel.backgroundObject = levelList[num].backgroundObject;
            //Enemy Settings
            newLevel.numOfWaves = levelList[num].numOfWaves;
            //Completion Values
            newLevel.initialReward = levelList[num].initialReward;
            newLevel.secondaryReward = levelList[num].secondaryReward;
        }
        else if (num == 6)
        {
            //Map Settings
            newLevel.difficulty = difficultyCustom;
            newLevel.mapRadius = mapRadiusCustom;
            //Asteroid Settings
            newLevel.asteroidCount = asteroidCountCustom;
            newLevel.asteroidMinScale = asteroidMinScaleCustom;
            newLevel.asteroidMaxScale = asteroidMaxScaleCustom;
            newLevel.asteroidMinGap = asteroidMinGapCustom;
            newLevel.fluffyAsteroidBounds = fluffyAsteroidBoundsCustom;
            newLevel.fluffyFactor = fluffyFactorCustom;
            newLevel.spawnOutsideAsteroids = spawnOutsideAsteroidsCustom;
            //Background Settings
            newLevel.backgroundSettings = backgroundSettingsCustom;
            newLevel.backgroundObject = backgroundObjectCustom;
            //Enemy Settings
            newLevel.numOfWaves = numOfWavesCustom;
            //Completion Values
            newLevel.initialReward = initialRewardCustom;
            newLevel.secondaryReward = secondaryRewardCustom;
        }
        else
        {
            //Map Settings
            newLevel.difficulty = difficultyDefault;
            newLevel.mapRadius = mapRadiusDefault;
            //Asteroid Settings
            newLevel.asteroidCount = asteroidCountDefault;
            newLevel.asteroidMinScale = asteroidMinScaleDefault;
            newLevel.asteroidMaxScale = asteroidMaxScaleDefault;
            newLevel.asteroidMinGap = asteroidMinGapDefault;
            newLevel.fluffyAsteroidBounds = fluffyAsteroidBoundsDefault;
            newLevel.fluffyFactor = fluffyFactorDefault;
            newLevel.spawnOutsideAsteroids = spawnOutsideAsteroidsDefault;
            //Background Settings
            newLevel.backgroundSettings = backgroundSettingsDefault;
            newLevel.backgroundObject = backgroundObjectDefault;
            //Enemy Settings
            newLevel.numOfWaves = numOfWavesDefault;
            //Completion Values
            newLevel.initialReward = initialRewardDefault;
            newLevel.secondaryReward = secondaryRewardDefault;
        }
        
        GameManagerBase.Instance.setLevel(newLevel);
    }
}
