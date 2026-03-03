using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level Settings/New Level")]
public class LevelInfo : ScriptableObject
{
    public enum levelDifficulty
    {
        Easy = 0,
        Medium,
        Hard,
        Extreme
    }
    public enum levelBackground
    {
        DarkSpace = 0,
        EarthSpace,
        GreenGalaxies,
        NaturalGalaxies,
        OrangeBlackHole,
        PinkGalaxies,
        PurpleSaturn,
        COUNT,
        Default
    }
    public enum largeObject
    {
        Warship = 0,
        Firefly,
        Venator,
        VictoryFleet,
        COUNT,
        None,
        Random,
        Default
    }

    public struct LevelSettings
    {
        //Map Settings
        public levelDifficulty difficulty;
        public int mapRadius;
        //Asteroid Settings
        public int asteroidCount;
        public int asteroidMinScale;
        public int asteroidMaxScale;
        public int asteroidMinGap;
        public bool fluffyAsteroidBounds;
        public int fluffyFactor;
        public bool spawnOutsideAsteroids;
        //Background Settings
        public levelBackground backgroundSettings;
        public largeObject backgroundObject;
        //Enemy Settings
        public int numOfWaves;
        //Completion Values
        public int initialReward;
        public int secondaryReward;
    }

    [Header("Map Settings")]
    [SerializeField] public levelDifficulty difficulty;
    [SerializeField] public int mapRadius;
    [Header("Asteroid Settings")]
    [SerializeField] public int asteroidCount;
    [SerializeField] public int asteroidMinScale;
    [SerializeField] public int asteroidMaxScale;
    [SerializeField] public int asteroidMinGap;
    [SerializeField] public bool fluffyAsteroidBounds;
    [SerializeField] public int fluffyFactor;
    [SerializeField] public bool spawnOutsideAsteroids;
    [Header("Background Settings")]
    [SerializeField] public levelBackground backgroundSettings;
    [SerializeField] public largeObject backgroundObject;
    [Header("Enemy Settings")]
    [SerializeField] public int numOfWaves;
    [Header("Completion Values")]
    [SerializeField] public int initialReward;
    [SerializeField] public int secondaryReward;
}
